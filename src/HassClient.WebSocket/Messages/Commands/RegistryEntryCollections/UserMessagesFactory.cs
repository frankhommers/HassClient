using HassClient.Core.Models.RegistryEntries;

namespace HassClient.WebSocket.Messages.Commands.RegistryEntryCollections
{
  internal class UserMessagesFactory : RegistryEntryCollectionMessagesFactory<User>
  {
    public static UserMessagesFactory Instance = new UserMessagesFactory();

    public UserMessagesFactory()
      : base("config/auth", "user")
    {
    }

    public new BaseOutgoingMessage CreateCreateMessage(User user)
    {
      return base.CreateCreateMessage(user);
    }

    public new BaseOutgoingMessage CreateUpdateMessage(User user, bool forceUpdate)
    {
      return base.CreateUpdateMessage(user, forceUpdate);
    }

    public new BaseOutgoingMessage CreateDeleteMessage(User user)
    {
      return base.CreateDeleteMessage(user);
    }
  }
}