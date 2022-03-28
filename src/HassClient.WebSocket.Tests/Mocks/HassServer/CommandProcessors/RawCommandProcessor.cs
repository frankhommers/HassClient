using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using HassClient.WebSocket.Messages.Response;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class RawCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is RawCommandMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    RawCommandMessage rawCommand = (RawCommandMessage)receivedCommand;
    string messageType = rawCommand.Type;
    return new ResultMessage { Success = true };
  }
}