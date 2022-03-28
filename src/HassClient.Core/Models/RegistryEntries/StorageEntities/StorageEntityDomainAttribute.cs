using System;
using HassClient.Core.Models.KnownEnums;

namespace HassClient.Core.Models.RegistryEntries.StorageEntities
{
  /// <summary>
  ///   Attribute used to specify the domain of a <see cref="StorageEntityRegistryEntryBase" />.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class StorageEntityDomainAttribute : Attribute
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="StorageEntityDomainAttribute" /> class.
    /// </summary>
    /// <param name="domain">The storage entity registry entry domain.</param>
    public StorageEntityDomainAttribute(KnownDomains domain)
    {
      Domain = domain;
    }

    /// <summary>
    ///   Gets or sets the storage entity registry entry domain.
    /// </summary>
    public KnownDomains Domain { get; set; }
  }
}