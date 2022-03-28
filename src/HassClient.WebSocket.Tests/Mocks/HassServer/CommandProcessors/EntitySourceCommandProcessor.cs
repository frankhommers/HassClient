using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class EntitySourceCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is EntitySourceMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    EntitySourceMessage commandEntitySource = receivedCommand as EntitySourceMessage;
    IEnumerable<EntitySource> objs;
    if (commandEntitySource.EntityIds?.Count() > 0)
      objs = MockHassModelFactory.EntitySourceFaker.GenerateWithEntityIds(commandEntitySource.EntityIds);
    else
      objs = MockHassModelFactory.EntitySourceFaker.Generate(10);

    JRaw resultObject = new(HassSerializer.SerializeObject(objs.ToDistinctDictionary(x => x.EntityId)));
    return CreateResultMessageWithResult(resultObject);
  }
}