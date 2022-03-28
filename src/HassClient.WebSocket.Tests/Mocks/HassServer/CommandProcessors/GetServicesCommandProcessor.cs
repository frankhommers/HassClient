using System.IO;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class GetServicesCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is GetServicesMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    using (Stream stream = GetResourceStream("GetServicesResponse.json"))
    using (StreamReader sr = new(stream))
    using (JsonTextReader reader = new(sr))
    {
      JRaw resultObject = JRaw.Create(reader);
      return CreateResultMessageWithResult(resultObject);
    }
  }
}