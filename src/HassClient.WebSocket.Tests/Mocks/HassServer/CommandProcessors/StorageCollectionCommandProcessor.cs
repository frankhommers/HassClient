using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.WebSocket.Messages.Commands.RegistryEntryCollections;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public class StorageCollectionCommandProcessor<TModel> :
  RegistryEntryCollectionCommandProcessor<RegistryEntryCollectionMessagesFactory<TModel>, TModel>
  where TModel : StorageEntityRegistryEntryBase
{
  public StorageCollectionCommandProcessor()
    : base(StorageCollectionMessagesFactory<TModel>.Create())
  {
  }

  protected override object ProccessListCommand(MockHassServerRequestContext context, JToken merged)
  {
    object result = base.ProccessListCommand(context, merged);
    if (typeof(TModel) == typeof(Person))
    {
      IEnumerable<Person> persons = (IEnumerable<Person>)result;
      return new PersonResponse
      {
        Storage = persons.Where(p => p.IsStorageEntry).ToArray(),
        Config = persons.Where(p => !p.IsStorageEntry).ToArray()
      };
    }

    return result;
  }
}