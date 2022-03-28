using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using HassClient.WebSocket.Messages.Response;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class PingCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is PingMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    return new PongMessage();
  }
}