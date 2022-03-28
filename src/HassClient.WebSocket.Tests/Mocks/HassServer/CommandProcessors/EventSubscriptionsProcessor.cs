using System.Collections.Generic;
using HassClient.Core.Helpers;
using HassClient.Core.Models.Events;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands.Subscriptions;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class EventSubscriptionsProcessor : BaseCommandProcessor
{
  private readonly Dictionary<string, List<uint>> subscribersByEventType = new();

  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is SubscribeEventsMessage || receivedCommand is UnsubscribeEventsMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    if (receivedCommand is SubscribeEventsMessage subscribeMessage)
    {
      string eventType = subscribeMessage.EventType ?? KnownEventTypes.Any.ToEventTypeString();
      if (!subscribersByEventType.TryGetValue(eventType, out List<uint> subcribers))
      {
        subcribers = new List<uint>();
        subscribersByEventType.Add(eventType, subcribers);
      }

      subcribers.Add(subscribeMessage.Id);
      return CreateResultMessageWithResult(null);
    }

    if (receivedCommand is UnsubscribeEventsMessage unsubscribeMessage)
      foreach (List<uint> item in subscribersByEventType.Values)
        if (item.Remove(unsubscribeMessage.SubscriptionId))
          //success = true;
          break;

    return CreateResultMessageWithResult(null);
  }

  public bool TryGetSubscribers(KnownEventTypes eventType, out List<uint> subscribers)
  {
    subscribers = new List<uint>();
    if (eventType != KnownEventTypes.Any &&
        subscribersByEventType.TryGetValue(KnownEventTypes.Any.ToEventTypeString(), out List<uint> anySubscribers))
      subscribers.AddRange(anySubscribers);

    if (subscribersByEventType.TryGetValue(eventType.ToEventTypeString(), out List<uint> typeSubscribers))
      subscribers.AddRange(typeSubscribers);

    return subscribers.Count > 0;
  }

  public void ClearSubscriptions()
  {
    subscribersByEventType.Clear();
  }
}