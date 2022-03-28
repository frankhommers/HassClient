using System.Collections.Generic;
using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class GetStatesCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is GetStatesMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    List<StateModel> states = MockHassModelFactory.StateModelFaker.Generate(30);
    JRaw resultObject = new(HassSerializer.SerializeObject(states));
    return CreateResultMessageWithResult(resultObject);
  }
}