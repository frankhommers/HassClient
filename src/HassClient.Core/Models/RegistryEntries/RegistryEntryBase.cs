using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using Newtonsoft.Json;

namespace HassClient.Core.Models.RegistryEntries
{
  /// <summary>
  ///   Defines a registry entry model that can be updated by the user using the API.
  /// </summary>
  public abstract class RegistryEntryBase
  {
    private readonly IModifiableProperty[] modifiableProperties;

    /// <summary>
    ///   Gets a value indicating whether the object has been deserialized.
    /// </summary>
    protected bool IsDeserialized;

    /// <summary>
    ///   Initializes a new instance of the <see cref="RegistryEntryBase" /> class.
    /// </summary>
    public RegistryEntryBase()
    {
      modifiableProperties = GetModifiableProperties().ToArray();
    }

    /// <summary>
    ///   Gets the unique identifier that represents this Registry Entry.
    /// </summary>
    protected internal abstract string UniqueId { get; set; }

    /// <summary>
    ///   Gets a value indicating that the registry entry already exists on the Home Assistant instance.
    /// </summary>
    [JsonIgnore]
    public bool IsTracked => IsDeserialized && UniqueId != null;

    /// <summary>
    ///   Gets a value indicating that the registry entry is marked as dirty and is pending to be updated.
    /// </summary>
    [JsonIgnore]
    public bool IsDirty { get; internal set; }

    /// <summary>
    ///   Gets a value indicating that the model has pending changes waiting to update.
    /// </summary>
    [JsonIgnore]
    public bool HasPendingChanges => modifiableProperties.Any(x => x.HasPendingChanges);

    /// <summary>
    ///   Gets all modifiable properties of the model.
    /// </summary>
    /// <returns>The modifiable properties of the model.</returns>
    protected abstract IEnumerable<IModifiableProperty> GetModifiableProperties();

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      IsDeserialized = true;
      SaveChanges();
    }

    /// <summary>
    ///   Clears the <see cref="HasPendingChanges" /> property.
    ///   <para>
    ///     Called internally when the model is deserialized or populated with updated values.
    ///   </para>
    /// </summary>
    protected void SaveChanges()
    {
      foreach (IModifiableProperty property in modifiableProperties) property.SaveChanges();
    }

    /// <summary>
    ///   Discard any pending changes made on the entity and clears the <see cref="HasPendingChanges" /> property.
    /// </summary>
    public void DiscardPendingChanges()
    {
      foreach (IModifiableProperty property in modifiableProperties) property.DiscardPendingChanges();
    }

    internal void Untrack()
    {
      UniqueId = null;
    }

    internal IEnumerable<string> GetModifiablePropertyNames()
    {
      return modifiableProperties
        .Select(x => x.Name);
    }

    internal IEnumerable<string> GetModifiedPropertyNames()
    {
      return modifiableProperties
        .Where(x => x.HasPendingChanges)
        .Select(x => x.Name);
    }
  }
}