using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Authentication;
using HassClient.WebSocket.Messages.Commands;
using HassClient.WebSocket.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer;

public class MockHassServerWebSocket : MockServerWebSocket
{
  private readonly MockHassDb hassDB = new();

  private MockHassServerRequestContext activeRequestContext;

  public MockHassServerWebSocket()
  {
    ConnectionParameters = ConnectionParameters.CreateFromInstanceBaseUrl(
      $"http://{ServerUri.Host}:{ServerUri.Port}",
      GenerateRandomToken());
  }

  public CalVer HAVersion => CalVer.Create("2022.1.0");

  public ConnectionParameters ConnectionParameters { get; }

  public bool IgnoreAuthenticationMessages { get; set; } = false;

  public TimeSpan ResponseSimulatedDelay { get; set; } = TimeSpan.Zero;

  public Task<bool> RaiseStateChangedEventAsync(string entityId)
  {
    StateChangedEvent data = MockHassModelFactory.StateChangedEventFaker
      .GenerateWithEntityId(entityId);

    EventResultInfo eventResult = new()
    {
      EventType = KnownEventTypes.StateChanged.ToEventTypeString(),
      Origin = "mock_server",
      TimeFired = DateTimeOffset.Now,
      Data = new JRaw(HassSerializer.SerializeObject(data)),
      Context = data.OldState.Context
    };

    JRaw eventResultObject = new(HassSerializer.SerializeObject(eventResult));
    return RaiseEventAsync(KnownEventTypes.StateChanged, eventResultObject);
  }

  public async Task<bool> RaiseEventAsync(KnownEventTypes eventType, JRaw eventResultObject)
  {
    MockHassServerRequestContext context = activeRequestContext;
    if (context.EventSubscriptionsProcessor.TryGetSubscribers(eventType, out List<uint> subscribers))
    {
      foreach (uint id in subscribers)
        await context.SendMessageAsync(new EventResultMessage { Event = eventResultObject, Id = id }, default);

      return true;
    }

    return false;
  }

  private string GenerateRandomToken()
  {
    return Guid.NewGuid().ToString("N");
  }

  protected override async Task RespondToWebSocketRequestAsync(System.Net.WebSockets.WebSocket webSocket, CancellationToken cancellationToken)
  {
    MockHassServerRequestContext context = new(hassDB, webSocket);

    await context.SendMessageAsync(new AuthenticationRequiredMessage { HAVersion = HAVersion.ToString() },
      cancellationToken);

    try
    {
      while (true)
        if (context.IsAuthenticating)
        {
          BaseMessage incomingMessage = await context.ReceiveMessageAsync<BaseMessage>(cancellationToken);
          await Task.Delay(ResponseSimulatedDelay);

          if (!IgnoreAuthenticationMessages &&
              incomingMessage is AuthenticationMessage authMessage)
          {
            if (authMessage.AccessToken == ConnectionParameters.AccessToken)
            {
              await context.SendMessageAsync(new AuthenticationOkMessage { HAVersion = HAVersion.ToString() },
                cancellationToken);
              context.IsAuthenticating = false;
              activeRequestContext = context;
            }
            else
            {
              await context.SendMessageAsync(new AuthenticationInvalidMessage(), cancellationToken);
              break;
            }
          }
        }
        else
        {
          BaseOutgoingMessage receivedMessage =
            await context.ReceiveMessageAsync<BaseOutgoingMessage>(cancellationToken);
          uint receivedMessageId = receivedMessage.Id;

          await Task.Delay(ResponseSimulatedDelay);

          BaseIdentifiableMessage response;
          if (context.LastReceivedID >= receivedMessageId)
          {
            response = new ResultMessage { Error = new ErrorInfo(ErrorCodes.IdReuse) };
          }
          else
          {
            context.LastReceivedID = receivedMessageId;

            if (receivedMessage is PingMessage)
              response = new PongMessage();
            else if (!context.TryProccesMessage(receivedMessage, out response))
              response = new ResultMessage { Error = new ErrorInfo(ErrorCodes.UnknownCommand) };
          }

          response.Id = receivedMessageId;
          await context.SendMessageAsync(response, cancellationToken);
        }
    }
    catch
    {
      Trace.WriteLine("A problem occured while attending client. Closing connection.");
      await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, string.Empty, default);
    }
  }
}