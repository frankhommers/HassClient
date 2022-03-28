using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Messages.Commands.RegistryEntryCollections
{
  internal class DeviceRegistryMessagesFactory : RegistryEntryCollectionMessagesFactory<Device>
  {
    public static readonly DeviceRegistryMessagesFactory Instance = new DeviceRegistryMessagesFactory();

    public DeviceRegistryMessagesFactory()
      : base("config/device_registry", "device")
    {
    }

    public BaseOutgoingMessage CreateUpdateMessage(Device device, bool? disable, bool forceUpdate)
    {
      JObject model = CreateDefaultUpdateObject(device, forceUpdate);

      if (disable.HasValue)
      {
        JObject merged = HassSerializer.CreateJObject(new
          { DisabledBy = disable.Value ? DisabledByEnum.User : (DisabledByEnum?)null });
        model.Merge(merged);
      }

      return CreateUpdateMessage(device.Id, model);
    }
  }
}