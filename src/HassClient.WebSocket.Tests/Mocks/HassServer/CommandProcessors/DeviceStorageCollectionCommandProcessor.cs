using HassClient.Core.Models.RegistryEntries;
using HassClient.WebSocket.Messages.Commands.RegistryEntryCollections;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

internal class DeviceStorageCollectionCommandProcessor
  : RegistryEntryCollectionCommandProcessor<DeviceRegistryMessagesFactory, Device>
{
  protected override void PrepareHassContext(MockHassServerRequestContext context)
  {
    base.PrepareHassContext(context);
    context.HassDb.CreateObject(MockHassModelFactory.DeviceFaker.Generate());
  }
}