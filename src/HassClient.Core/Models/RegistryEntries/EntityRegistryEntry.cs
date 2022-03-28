using System.Collections.Generic;
using System.Linq;
using HassClient.Core.Helpers;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Models.RegistryEntries
{
  /// <summary>
  ///   The Entity Registry keeps a registry of entities. Entities are uniquely identified by their domain, platform and
  ///   an unique id provided by that platform.
  /// </summary>
  public class EntityRegistryEntry : EntityRegistryEntryBase
  {
    [JsonProperty("disabled_by")] 
    private readonly ModifiableProperty<DisabledByEnum?> _disabledBy =
      new ModifiableProperty<DisabledByEnum?>(nameof(_disabledBy));

    private string _deviceClass;

    [JsonProperty("entity_id", Required = Required.Always)]
    private string _entityId;

    [JsonConstructor]
    private EntityRegistryEntry()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="EntityRegistryEntry" /> class.
    ///   <para>
    ///     Used for testing purposes.
    ///   </para>
    /// </summary>
    /// <param name="entityId">The entity id.</param>
    /// <param name="name">The original name.</param>
    /// <param name="icon">The original icon.</param>
    /// <param name="disabledBy">The original disable.</param>
    protected internal EntityRegistryEntry(string entityId, string name, string icon,
      DisabledByEnum disabledBy = DisabledByEnum.None)
      : base(name, icon)
    {
      this._entityId = entityId;
      Platform = entityId.GetDomain();
      this._disabledBy.Value = disabledBy;

      SaveChanges();
    }

    /// <inheritdoc />
    [JsonProperty]
    protected internal override string UniqueId { get; set; }

    /// <inheritdoc />
    public override string Name
    {
      get => base.Name ?? OriginalName;
      set => base.Name = value != OriginalName ? value : null;
    }

    /// <inheritdoc />
    public override string Icon
    {
      get => base.Icon ?? OriginalIcon;
      set => base.Icon = value != OriginalIcon ? value : null;
    }

    /// <inheritdoc />
    protected override bool AcceptsNullOrWhiteSpaceName => true;

    /// <inheritdoc />
    public override string EntityId => _entityId;

    /// <summary>
    ///   Gets the original friendly name of this entity.
    /// </summary>
    [JsonProperty]
    public string OriginalName { get; protected set; }

    /// <summary>
    ///   Gets the original icon to display in front of the entity in the front-end.
    /// </summary>
    [JsonProperty]
    public string OriginalIcon { get; protected set; }

    /// <summary>
    ///   Gets the original device class.
    /// </summary>
    [JsonProperty]
    public string OriginalDeviceClass { get; protected set; }

    /// <summary>
    ///   Gets the class of the device. This affects the state and default icon representation
    ///   of the entity.
    /// </summary>
    [JsonProperty]
    public string DeviceClass
    {
      get => _deviceClass ?? OriginalDeviceClass;
      set => _deviceClass = value != OriginalDeviceClass ? value : null;
    }

    /// <summary>
    ///   Gets the platform associated with this entity registry.
    /// </summary>
    [JsonProperty]
    public string Platform { get; private set; }

    /// <summary>
    ///   Gets the device id associated with this entity registry.
    /// </summary>
    [JsonProperty]
    public string DeviceId { get; private set; }

    /// <summary>
    ///   Gets the area id associated with this entity registry.
    /// </summary>
    [JsonProperty]
    public string AreaId { get; private set; }

    /// <summary>
    ///   Gets the configuration entry id associated with this entity registry.
    /// </summary>
    [JsonProperty]
    public string ConfigEntryId { get; internal set; }

    /// <summary>
    ///   Gets a value indicating the disabling source, if any.
    /// </summary>
    [JsonIgnore]
    public DisabledByEnum DisabledBy => _disabledBy.Value ?? DisabledByEnum.None;

    /// <summary>
    ///   Gets a value indicating whether the entity is disabled.
    /// </summary>
    [JsonIgnore]
    public bool IsDisabled => DisabledBy != DisabledByEnum.None;

    /// <summary>
    ///   Gets the capabilities of the entity.
    /// </summary>
    [JsonProperty]
    public Dictionary<string, JRaw> Capabilities { get; private set; }

    /// <summary>
    ///   Gets a the supported features of this entity, if any.
    /// </summary>
    [JsonProperty]
    public int SupportedFeatures { get; private set; }

    /// <summary>
    ///   Gets the units of measurement, if any. This will also influence the graphical presentation
    ///   in the history visualization as continuous value.
    ///   Sensors with missing <see cref="UnitOfMeasurement" /> are showing as discrete values.
    /// </summary>
    [JsonProperty]
    public string UnitOfMeasurement { get; private set; }

    /// <summary>
    ///   Gets a value indicating the classification for non-primary entities.
    ///   <para>
    ///     Primary entity's category will be <see cref="RegistryEntries.EntityCategory.None" />.
    ///   </para>
    /// </summary>
    [JsonProperty]
    public string EntityCategory { get; private set; }

    /// <summary>
    ///   Gets the domain of the entity.
    /// </summary>
    [JsonIgnore]
    public string Domain => EntityId.GetDomain();

    // Used for testing purposes.
    internal static EntityRegistryEntry CreateUnmodified(string entityId, string name, string icon = null,
      DisabledByEnum disabledBy = DisabledByEnum.None)
    {
      return new EntityRegistryEntry(entityId, name, icon, disabledBy);
    }

    // Used for testing purposes.
    internal static EntityRegistryEntry CreateFromEntry(EntityRegistryEntryBase entry,
      DisabledByEnum disabledBy = DisabledByEnum.None)
    {
      return new EntityRegistryEntry(entry.EntityId, entry.Name, entry.Icon, disabledBy);
    }

    /// <inheritdoc />
    protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
    {
      return base.GetModifiableProperties().Append(_disabledBy);
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"{nameof(EntityRegistryEntry)}: {EntityId}";
    }

    // Used for testing purposes.
    internal EntityRegistryEntry Clone()
    {
      EntityRegistryEntry result = CreateUnmodified(EntityId, Name, Icon, DisabledBy);
      result.UniqueId = UniqueId;
      result._entityId = _entityId;
      result.AreaId = AreaId;
      result.Capabilities = Capabilities;
      result.ConfigEntryId = ConfigEntryId;
      result.DeviceClass = DeviceClass;
      result.DeviceId = DeviceId;
      result.OriginalName = OriginalName;
      result.OriginalIcon = OriginalIcon;
      result.Platform = Platform;
      result.SupportedFeatures = SupportedFeatures;
      result.UnitOfMeasurement = UnitOfMeasurement;
      return result;
    }
  }
}