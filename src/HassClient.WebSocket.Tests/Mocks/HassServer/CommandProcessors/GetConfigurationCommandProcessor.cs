using HassClient.Core.Models;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class GetConfigurationCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is GetConfigMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    ConfigurationModel configuration = MockHassModelFactory.ConfigurationFaker.Generate();
    JRaw resultObject = new(HassSerializer.SerializeObject(configuration));
    return CreateResultMessageWithResult(resultObject);
  }
}