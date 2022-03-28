using System;
using HassClient.Core.Models;
using Newtonsoft.Json;

namespace HassClient.Core.Serialization.Converters
{
  /// <summary>
  ///   Converter for <see cref="CalVer" />.
  /// </summary>
  public class CalVerConverter : JsonConverter<CalVer>
  {
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, CalVer value, JsonSerializer serializer)
    {
      serializer.Serialize(writer, value.ToString());
    }

    /// <inheritdoc />
    public override CalVer ReadJson(JsonReader reader, Type objectType, CalVer existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      string versionStr = serializer.Deserialize<string>(reader);
      existingValue = existingValue ?? new CalVer();
      existingValue.Parse(versionStr);
      return existingValue;
    }
  }
}