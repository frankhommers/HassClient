﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HassClient.Core.Serialization.Converters
{
  /// <summary>
  ///   Converter to convert Set[Tuple[str, str]] to <see cref="Dictionary{TKey,TValue}" />.
  /// </summary>
  public class TupleSetToDictionaryConverter : JsonConverter<Dictionary<string, string>>
  {
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, Dictionary<string, string> value, JsonSerializer serializer)
    {
      Dictionary<string, string> valueDic = value;
      string[][] array = valueDic.Select(x => new[] { x.Key, x.Value }).ToArray();
      serializer.Serialize(writer, array);
    }

    /// <inheritdoc />
    public override Dictionary<string, string> ReadJson(JsonReader reader, Type objectType,
      Dictionary<string, string> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
      string[][] array = serializer.Deserialize<string[][]>(reader);

      if (array == null ||
          array.Length == 0)
        return new Dictionary<string, string>();

      return array.ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : null);
    }
  }
}