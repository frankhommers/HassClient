using System;
using HassClient.Core.Models.RegistryEntries.Modifiable;
using Newtonsoft.Json;

namespace HassClient.Core.Serialization.Converters
{
  /// <summary>
  ///   Converter for <see cref="ModifiableProperty{T}" />.
  /// </summary>
  public class ModifiablePropertyConverter : JsonConverter
  {
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      Type type = value.GetType();
      value = type.GetProperty(nameof(ModifiableProperty<object>.Value))
        ?.GetValue(value, null);
      serializer.Serialize(writer, value);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      Type argType = objectType.GetGenericArguments()[0];
      object value = serializer.Deserialize(reader, argType);

      if (existingValue != null)
        objectType.GetProperty(nameof(ModifiableProperty<object>.Value))
          ?.SetValue(existingValue, value);
      else
        existingValue = Activator.CreateInstance(objectType, value);

      return existingValue;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type type)
    {
      return type.IsGenericType &&
             type.GetGenericTypeDefinition() == typeof(ModifiableProperty<>);
    }
  }
}