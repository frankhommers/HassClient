using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HassClient.Core.Helpers;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using Newtonsoft.Json;

namespace HassClient.Core.Models.RegistryEntries.StorageEntities
{
  /// <summary>
  ///   Represents a person.
  /// </summary>
  [StorageEntityDomain(KnownDomains.Person)]
  public class Person : StorageEntityRegistryEntryBase
  {
    private readonly ModifiablePropertyCollection<string> _deviceTrackers =
      new ModifiablePropertyCollection<string>(
        nameof(DeviceTrackers),
        v => v.IsValidDomainEntityId(KnownDomains.DeviceTracker));

    private readonly ModifiableProperty<string> _picture = new ModifiableProperty<string>(nameof(Picture));
    private readonly ModifiableProperty<string> _userId = new ModifiableProperty<string>(nameof(UserId));

    [JsonConstructor]
    private Person()
    {
    }

    private Person(string name, string userId)
      : base(name, null)
    {
      UserId = userId;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="Person" /> class.
    /// </summary>
    /// <param name="name">The entity name.</param>
    /// <param name="user">The user account of the Home Assistant associated to this person.</param>
    public Person(string name, User user)
      : this(name, user?.Id)
    {
      if (user is null) throw new ArgumentNullException(nameof(user));

      IsStorageEntry = true;
    }
    //// "device_trackers":["device_tracker.demo_anne_therese","device_tracker.demo_paulus"],
    //// "picture":"/api/image/serve/f986543a0ea7b88ebffcd1213aeffb32/512x512"}

    /// <summary>
    ///   Gets a value indicating whether the <see cref="Person" /> entry is defined within the storage
    ///   and, therefore, is editable.
    /// </summary>
    [JsonIgnore]
    public bool IsStorageEntry { get; internal set; }

    /// <summary>
    ///   Gets the user ID of the Home Assistant user account for the person.
    ///   <para>To set this property use the method <see cref="ChangeUser(User)" />.</para>
    /// </summary>
    [JsonProperty]
    public string UserId
    {
      get => _userId.Value;
      private set => _userId.Value = value;
    }

    /// <summary>
    ///   Gets or sets a list of device trackers entities associated to this person entity.
    /// </summary>
    [JsonProperty]
    public ICollection<string> DeviceTrackers => _deviceTrackers.Value;

    /// <summary>
    ///   Gets or sets a URL (relative or absolute) to a picture for the person entity.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Picture
    {
      get => _picture.Value;
      set
      {
        CheckSupportModification();
        _picture.Value = value;
      }
    }

    /// <summary>
    ///   <see cref="Icon" /> property is inherited but is not available for <see cref="Person" /> entities.
    ///   <para>
    ///     Trying to set this property to a value different to <see langword="null" /> will throw
    ///     an <see cref="InvalidOperationException" />.
    ///   </para>
    /// </summary>
    public override string Icon
    {
      get => null;
      set
      {
        if (value != null)
          throw new InvalidOperationException("Persons has no icon and, therefore, cannot be modified.");
      }
    }

    /// <summary>
    ///   Changes the user account of the Home Assistant associated to this person entity.
    ///   <para>This method affects the <see cref="UserId" /> property.</para>
    /// </summary>
    /// <param name="user">The user account of the Home Assistant associated to this person entity.</param>
    public void ChangeUser(User user)
    {
      if (user is null) throw new ArgumentNullException(nameof(user));

      CheckSupportModification(nameof(UserId));

      UserId = user.Id;
    }

    private void CheckSupportModification([CallerMemberName] string propertyName = null)
    {
      if (IsTracked && !IsStorageEntry)
        throw new NotSupportedException(
          $"The property {propertyName} can only be modified on {nameof(Person)} storage entries.");
    }

    // Used for testing purposes.
    internal static Person CreateUnmodified(string uniqueId, string name, string userId, string picture = null,
      IEnumerable<string> deviceTrackers = null)
    {
      Person result = new Person(name, userId)
      {
        Id = uniqueId,
        Picture = picture
      };

      if (deviceTrackers != null)
        foreach (string item in deviceTrackers)
          result.DeviceTrackers.Add(item);

      result.SaveChanges();
      return result;
    }

    /// <inheritdoc />
    protected override IEnumerable<IModifiableProperty> GetModifiableProperties()
    {
      return base.GetModifiableProperties()
        .Append(_userId)
        .Append(_picture)
        .Append(_deviceTrackers)
        .Where(x => x.Name != nameof(Icon));
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"{nameof(Person)}: {Name}";
    }

    // Used for testing purposes.
    internal Person Clone()
    {
      Person result = CreateUnmodified(UniqueId, Name, UserId, Picture, DeviceTrackers);
      return result;
    }
  }
}