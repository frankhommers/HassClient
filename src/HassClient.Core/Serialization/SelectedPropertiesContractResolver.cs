﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HassClient.Core.Serialization
{
  /// <summary>
  ///   Contract resolver used to filter only selected properties during object serialization.
  /// </summary>
  public class SelectedPropertiesContractResolver : DefaultContractResolver
  {
    private HashSet<string> _selectedProperties;

    /// <summary>
    ///   White-list containing the named of the properties to be included in the serialization.
    /// </summary>
    public IEnumerable<string> SelectedProperties
    {
      get => _selectedProperties;
      set => _selectedProperties = new HashSet<string>(value);
    }

    /// <inheritdoc />
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      IList<JsonProperty> allProps = base.CreateProperties(type, memberSerialization);
      return allProps.Where(p => _selectedProperties.Contains(p.UnderlyingName)).ToList();
    }
  }
}