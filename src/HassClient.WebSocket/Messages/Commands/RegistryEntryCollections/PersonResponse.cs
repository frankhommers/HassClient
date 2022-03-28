using HassClient.Core.Models.RegistryEntries.StorageEntities;

namespace HassClient.WebSocket.Messages.Commands.RegistryEntryCollections
{
  internal class PersonResponse
  {
    public Person[] Storage { get; set; }

    public Person[] Config { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"{nameof(Storage)}: {Storage?.Length ?? 0}\t{nameof(Config)}: {Config?.Length ?? 0}";
    }
  }
}