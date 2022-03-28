using System.Threading.Tasks;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages.Response;
using HassClient.WebSocket.Tests.Mocks;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class SubscriptionApiTests : BaseHassWsApiTest
{
  private const string TestEntityId = "light.ceiling_lights";

  private async Task<StateChangedEvent> ForceStateChangedAndGetEventData(MockEventSubscriber subscriber)
  {
    string domain = TestEntityId.GetDomain();
    bool update = await HassWsApi.CallServiceForEntitiesAsync(domain, "toggle", TestEntityId);
    Assert.NotNull(update, "SetUp failed");

    EventResultInfo eventResultInfo = await subscriber.WaitFirstEventArgWithTimeoutAsync<EventResultInfo>(
      x => HassSerializer.TryGetEnumFromSnakeCase(x.EventType, out KnownEventTypes knownEventType) &&
           knownEventType == KnownEventTypes.StateChanged,
      500);

    Assert.NotNull(eventResultInfo, "SetUp failed");

    return eventResultInfo.DeserializeData<StateChangedEvent>();
  }

  [Test]
  public async Task AddMultipleEventHandlerSubscriptionForAnyEvent()
  {
    MockEventHandler<EventResultInfo> testEventHandler1 = new();
    MockEventHandler<EventResultInfo> testEventHandler2 = new();
    MockEventSubscriber subscriber1 = new();
    MockEventSubscriber subscriber2 = new();
    testEventHandler1.Event += subscriber1.Handle;
    testEventHandler2.Event += subscriber2.Handle;
    bool result1 = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler1.EventHandler);
    bool result2 = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler2.EventHandler);

    Assert.IsTrue(result1);
    Assert.IsTrue(result2);

    StateChangedEvent eventData = await ForceStateChangedAndGetEventData(subscriber1);

    Assert.NotZero(subscriber1.HitCount);
    Assert.AreEqual(subscriber1.HitCount, subscriber2.HitCount);
    Assert.IsTrue(eventData.EntityId == TestEntityId);
  }

  [Test]
  public async Task AddEventHandlerSubscriptionForAnyEvent()
  {
    MockEventHandler<EventResultInfo> testEventHandler = new();
    MockEventSubscriber subscriber = new();
    testEventHandler.Event += subscriber.Handle;
    bool result = await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler);

    Assert.IsTrue(result);

    await ForceStateChangedAndGetEventData(subscriber);

    Assert.NotZero(subscriber.HitCount);
  }

  [Test]
  public async Task AddEventHandlerSubscriptionForStateChangedEvents()
  {
    MockEventHandler<EventResultInfo> testEventHandler = new();
    MockEventSubscriber subscriber = new();
    testEventHandler.Event += subscriber.Handle;
    bool result =
      await HassWsApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler, KnownEventTypes.StateChanged);

    Assert.IsTrue(result);

    StateChangedEvent eventData = await ForceStateChangedAndGetEventData(subscriber);

    Assert.NotZero(subscriber.HitCount);
    Assert.IsTrue(eventData.EntityId == TestEntityId);
    Assert.NotNull(eventData.NewState.State);
  }
}