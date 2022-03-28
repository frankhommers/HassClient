using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class CallServiceCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is CallServiceMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    CallServiceMessage callServiceMsg = receivedCommand as CallServiceMessage;
    StateModel state = new()
    {
      Context = MockHassModelFactory.ContextFaker.Generate()
    };
    JRaw resultObject = new(HassSerializer.SerializeObject(state));
    return CreateResultMessageWithResult(resultObject);
  }
}