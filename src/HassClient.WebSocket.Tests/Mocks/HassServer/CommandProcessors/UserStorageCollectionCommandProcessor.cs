using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages.Commands.RegistryEntryCollections;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

internal class UserStorageCollectionCommandProcessor
  : RegistryEntryCollectionCommandProcessor<UserMessagesFactory, User>
{
  protected override object ProccessCreateCommand(MockHassServerRequestContext context, JToken merged)
  {
    User user = (User)base.ProccessCreateCommand(context, merged);
    user.SetIsActive(true);
    return new UserResponse { UserRaw = new JRaw(HassSerializer.SerializeObject(user)) };
  }

  protected override object ProccessUpdateCommand(MockHassServerRequestContext context, JToken merged)
  {
    object user = base.ProccessUpdateCommand(context, merged);
    return new UserResponse { UserRaw = new JRaw(HassSerializer.SerializeObject(user)) };
  }

  protected override object ProccessListCommand(MockHassServerRequestContext context, JToken merged)
  {
    return base.ProccessListCommand(context, merged);
  }

  protected override void PrepareHassContext(MockHassServerRequestContext context)
  {
    base.PrepareHassContext(context);
    User ownerUser = User.CreateUnmodified(Faker.RandomUUID(), "owner", true);
    context.HassDb.CreateObject(ownerUser);
  }
}