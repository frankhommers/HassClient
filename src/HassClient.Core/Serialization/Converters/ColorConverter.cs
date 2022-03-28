using System;
using HassClient.Core.Models.Color;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.Core.Serialization.Converters
{
  /// <summary>
  ///   Converter for <see cref="Color" />.
  /// </summary>
  public class ColorConverter : JsonConverter<Color>
  {
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
      if (value is NameColor)
        serializer.Serialize(writer, value.ToString());
      else if (value is KelvinTemperatureColor kelvinColor)
        serializer.Serialize(writer, kelvinColor.Kelvins);
      else if (value is MiredsTemperatureColor miredsColor)
        serializer.Serialize(writer, miredsColor.Mireds);
      else
        serializer.Serialize(writer, JArray.Parse(value.ToString()));
    }

    /// <inheritdoc />
    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      if (objectType == typeof(RgbwColor))
      {
        JArray values = serializer.Deserialize<JArray>(reader);
        if (hasExistingValue)
        {
          RgbwColor rgbwColor = existingValue as RgbwColor;
          rgbwColor.R = (byte)values[0];
          rgbwColor.G = (byte)values[1];
          rgbwColor.B = (byte)values[2];
          rgbwColor.W = (byte)values[3];
          return rgbwColor;
        }

        return Color.FromRgbw((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3]);
      }

      if (objectType == typeof(RgbwwColor))
      {
        JArray values = serializer.Deserialize<JArray>(reader);
        if (hasExistingValue)
        {
          RgbwwColor rgbwwColor = existingValue as RgbwwColor;
          rgbwwColor.R = (byte)values[0];
          rgbwwColor.G = (byte)values[1];
          rgbwwColor.B = (byte)values[2];
          rgbwwColor.CW = (byte)values[3];
          rgbwwColor.WW = (byte)values[4];
          return rgbwwColor;
        }

        return Color.FromRgbww((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3], (byte)values[4]);
      }

      if (objectType == typeof(RgbColor))
      {
        JArray values = serializer.Deserialize<JArray>(reader);
        if (hasExistingValue)
        {
          RgbColor rgbColor = existingValue as RgbColor;
          rgbColor.R = (byte)values[0];
          rgbColor.G = (byte)values[1];
          rgbColor.B = (byte)values[2];
          return rgbColor;
        }

        return Color.FromRgb((byte)values[0], (byte)values[1], (byte)values[2]);
      }

      if (objectType == typeof(HsColor))
      {
        JArray values = serializer.Deserialize<JArray>(reader);
        if (hasExistingValue)
        {
          HsColor hsColor = existingValue as HsColor;
          hsColor.Hue = (uint)values[0];
          hsColor.Saturation = (uint)values[1];
          return hsColor;
        }

        return Color.FromHs((uint)values[0], (uint)values[1]);
      }

      if (objectType == typeof(XyColor))
      {
        JArray values = serializer.Deserialize<JArray>(reader);
        if (hasExistingValue)
        {
          XyColor xyColor = existingValue as XyColor;
          xyColor.X = (float)values[0];
          xyColor.Y = (float)values[1];
          return xyColor;
        }

        return Color.FromXy((float)values[0], (float)values[1]);
      }

      if (objectType == typeof(NameColor))
      {
        string colorName = serializer.Deserialize<string>(reader);
        if (hasExistingValue)
        {
          NameColor nameColor = existingValue as NameColor;
          nameColor.Name = colorName;
          return nameColor;
        }

        return new NameColor(colorName);
      }

      if (objectType == typeof(MiredsTemperatureColor))
      {
        uint mireds = serializer.Deserialize<uint>(reader);
        if (hasExistingValue)
        {
          MiredsTemperatureColor color = existingValue as MiredsTemperatureColor;
          color.Mireds = mireds;
          return color;
        }

        return Color.FromMireds(mireds);
      }

      if (objectType == typeof(KelvinTemperatureColor))
      {
        uint kelvins = serializer.Deserialize<uint>(reader);
        if (hasExistingValue)
        {
          KelvinTemperatureColor color = existingValue as KelvinTemperatureColor;
          color.Kelvins = kelvins;
          return color;
        }

        return Color.FromKelvinTemperature(kelvins);
      }

      return null;
    }
  }
}