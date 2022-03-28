using System.Collections.Generic;
using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class PanelsCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is GetPanelsMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    IDictionary<string, PanelInfo> objs = MockHassModelFactory.PanelInfoFaker.Generate(10)
      .ToDistinctDictionary(x => x.ComponentName);
    JRaw resultObject = new(HassSerializer.SerializeObject(objs));
    return CreateResultMessageWithResult(resultObject);
  }
}