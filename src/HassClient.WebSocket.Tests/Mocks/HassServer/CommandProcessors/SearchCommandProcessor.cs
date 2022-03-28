using Bogus;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Commands.Search;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class SearchCommandProcessor : BaseCommandProcessor
{
  public override bool CanProcess(BaseIdentifiableMessage receivedCommand)
  {
    return receivedCommand is SearchRelatedMessage;
  }

  public override BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand)
  {
    SearchRelatedMessage searchMessage = receivedCommand as SearchRelatedMessage;
    SearchRelatedResponse resultResponse = new();

    if (searchMessage.ItemType == ItemType.Entity &&
        searchMessage.ItemId == "light.bed_light")
    {
      Faker faker = new();
      resultResponse.AreaIds = new[] { faker.RandomUUID() };
      resultResponse.AutomationIds = new[] { faker.RandomUUID() };
      resultResponse.ConfigEntryIds = new[] { faker.RandomUUID() };
      resultResponse.DeviceIds = new[] { faker.RandomUUID() };
      resultResponse.EntityIds = new[] { faker.RandomEntityId() };
    }

    JRaw resultObject = new(HassSerializer.SerializeObject(resultResponse));
    return CreateResultMessageWithResult(resultObject);
  }
}