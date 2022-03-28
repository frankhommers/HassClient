using HassClient.Core.Models.RegistryEntries;

namespace HassClient.WebSocket.Messages.Commands.RegistryEntryCollections
{
  internal class AreaRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Area>
  {
    public static readonly AreaRegistryMessagesFactory Instance = new AreaRegistryMessagesFactory();

    public AreaRegistryMessagesFactory()
      : base("config/area_registry", "area")
    {
    }

    public new BaseOutgoingMessage CreateCreateMessage(Area area)
    {
      return base.CreateCreateMessage(area);
    }

    public new BaseOutgoingMessage CreateUpdateMessage(Area area, bool forceUpdate)
    {
      return base.CreateUpdateMessage(area, forceUpdate);
    }

    public new BaseOutgoingMessage CreateDeleteMessage(Area area)
    {
      return base.CreateDeleteMessage(area);
    }
  }
}