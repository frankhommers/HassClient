using System.Linq;
using System.Runtime.Serialization;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using HassClient.WebSocket.Messages.Commands.RegistryEntryCollections;
using HassClient.WebSocket.Messages.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

internal class EntityRegistryStorageCollectionCommandProcessor
  : RegistryEntryCollectionCommandProcessor<EntityRegistryMessagesFactory, EntityRegistryEntry>
{
  protected override bool IsValidCommandType(string commandType)
  {
    return commandType.EndsWith("create") ||
           commandType.EndsWith("list") ||
           commandType.EndsWith("update") ||
           commandType.EndsWith("get") ||
           commandType.EndsWith("remove");
  }

  protected override object ProccessListCommand(MockHassServerRequestContext context, JToken merged)
  {
    return context.HassDb.GetAllEntityEntries()
      .Select(x => x as EntityRegistryEntry ?? EntityRegistryEntry.CreateFromEntry(x));
  }

  protected override object ProccessUpdateCommand(MockHassServerRequestContext context, JToken merged)
  {
    JToken newEntityIdProperty =
      merged.FirstOrDefault(x => x is JProperty property && property.Name == "new_entity_id");
    string newEntityId = (string)newEntityIdProperty;
    newEntityIdProperty?.Remove();

    JToken entityIdProperty = merged.FirstOrDefault(x => x is JProperty property && property.Name == "entity_id");
    string entityId = (string)entityIdProperty;
    MockRegistryEntity result = FindRegistryEntry(context, entityId, true);
    if (result != null)
    {
      if (newEntityId != null)
      {
        context.HassDb.DeleteObject(result.Entry);
        ((JProperty)entityIdProperty).Value = newEntityId;
      }

      PopulateModel(merged, result);

      if (newEntityId != null) context.HassDb.CreateObject(result.Entry);
    }

    return new EntityEntryResponse { EntityEntryRaw = new JRaw(HassSerializer.SerializeObject(result)) };
  }

  protected override object ProccessUnknownCommand(string commandType, MockHassServerRequestContext context,
    JToken merged)
  {
    string entityId = merged.Value<string>("entity_id");
    if (string.IsNullOrEmpty(entityId)) return ErrorCode.InvalidFormat;

    if (commandType.EndsWith("get")) return FindRegistryEntry(context, entityId, true);

    if (commandType.EndsWith("remove"))
    {
      MockRegistryEntity mockEntry = FindRegistryEntry(context, entityId, false);
      if (mockEntry == null) return ErrorCode.NotFound;

      context.HassDb.DeleteObject(mockEntry);
      bool result = context.HassDb.DeleteObject(mockEntry.Entry);
      return result ? null : ErrorCode.NotFound;
    }

    return base.ProccessUnknownCommand(commandType, context, merged);
  }

  private MockRegistryEntity FindRegistryEntry(MockHassServerRequestContext context, string entityId,
    bool createIfNotFound)
  {
    MockHassDb hassDB = context.HassDb;
    MockRegistryEntity result = hassDB.GetObjects<MockRegistryEntity>()?.FirstOrDefault(x => x.EntityId == entityId);
    if (result != null) return result;

    EntityRegistryEntryBase entry = hassDB.FindEntityEntry(entityId);
    if (entry == null) return null;

    result = new MockRegistryEntity(entry);

    if (createIfNotFound) hassDB.CreateObject(result);

    return result;
  }

  protected override void PrepareHassContext(MockHassServerRequestContext context)
  {
    base.PrepareHassContext(context);
    MockHassDb hassDB = context.HassDb;
    hassDB.CreateObject(new MockRegistryEntity("light.bed_light", "Bed Light")
    {
      UniqueId = Faker.RandomUUID(),
      ConfigEntryId = Faker.RandomUUID()
    });

    hassDB.CreateObject(
      new MockRegistryEntity("switch.fake_switch", "Fake Switch", "mdi: switch", DisabledByEnum.Integration)
      {
        UniqueId = Faker.RandomUUID(),
        ConfigEntryId = Faker.RandomUUID()
      });
  }

  private class MockRegistryEntity : EntityRegistryEntry
  {
    [JsonIgnore] public readonly EntityRegistryEntryBase Entry;

    public MockRegistryEntity(string entityId, string originalName, string originalIcon = null,
      DisabledByEnum disabledBy = DisabledByEnum.None)
      : base(entityId, null, null, disabledBy)
    {
      OriginalName = originalName;
      OriginalIcon = originalIcon;
    }

    public MockRegistryEntity(EntityRegistryEntryBase entry, DisabledByEnum disabledBy = DisabledByEnum.None)
      : this(entry.EntityId, entry.Name, entry.Icon, disabledBy)
    {
      Entry = entry;
      UniqueId = entry.UniqueId;
      Name = entry.Name;
      Icon = entry.Icon;
    }

    [OnDeserialized]
    private void OnDeserializedMock(StreamingContext context)
    {
      Entry.Name = Name;
      Entry.Icon = Icon;
      //this.Entry.UniqueId = this.EntityId.SplitEntityId()[1];
    }
  }
}