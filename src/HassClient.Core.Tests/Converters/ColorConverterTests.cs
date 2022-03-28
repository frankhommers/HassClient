using System;
using System.Collections.Generic;
using System.IO;
using HassClient.Core.Models.Color;
using HassClient.Core.Serialization.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HassClient.Core.Tests.Converters;

[TestFixture(TestOf = typeof(ColorConverter))]
public class ColorConverterTests
{
  private readonly ColorConverter converter = new();

  [Test]
  [TestCase(typeof(Color))]
  [TestCase(typeof(HsColor))]
  [TestCase(typeof(KelvinTemperatureColor))]
  [TestCase(typeof(MiredsTemperatureColor))]
  [TestCase(typeof(NameColor))]
  [TestCase(typeof(RgbColor))]
  [TestCase(typeof(RgbwColor))]
  [TestCase(typeof(XyColor))]
  public void CanConvertColors(Type colorType)
  {
    bool canConvert = converter.CanConvert(colorType);

    Assert.True(canConvert);
  }

  public static IEnumerable<TestCaseData> WriteReadJsonTestCases()
  {
    Func<Color, TestCaseData> createData = (Color color) =>
      new TestCaseData(color).SetName($"{{m}}{color.GetType().Name}");

    yield return createData(new RgbColor(10, 20, 30));
    yield return createData(new RgbwColor(10, 20, 30, 255));
    yield return createData(new RgbwwColor(10, 20, 30, 128, 255));
    yield return createData(new HsColor(10, 20));
    yield return createData(new XyColor(0.2f, 0.6f));
    yield return createData(new NameColor("test_color"));
    yield return createData(new KelvinTemperatureColor(1337));
    yield return createData(new MiredsTemperatureColor(256));
  }

  [Test]
  [TestCaseSource(nameof(WriteReadJsonTestCases))]
  public void WriteJson(Color color)
  {
    StringWriter textWriter = new StringWriter();
    JsonTextWriter jsonWriter = new JsonTextWriter(textWriter);
    JsonSerializer serializer = JsonSerializer.Create();
    converter.WriteJson(jsonWriter, color, serializer);

    Assert.AreEqual(GetJsonRepresentation(color), textWriter.ToString());
  }

  [Test]
  [TestCaseSource(nameof(WriteReadJsonTestCases))]
  public void ReadJson(Color color)
  {
    StringReader textReader = new StringReader(GetJsonRepresentation(color));
    JsonTextReader jsonReader = new JsonTextReader(textReader);
    JsonSerializer serializer = JsonSerializer.Create();
    object result = converter.ReadJson(jsonReader, color.GetType(), null, serializer);

    Assert.NotNull(result);
    Assert.AreNotEqual(color, result);
    Assert.AreEqual(color.ToString(), result.ToString());
  }

  public static IEnumerable<TestCaseData> ReadJsonWithExisingValueTestCases()
  {
    Func<Color, Color, TestCaseData> createData = (Color existing, Color color) =>
      new TestCaseData(existing, color).SetName($"{{m}}{color.GetType().Name}");

    yield return createData(new RgbColor(10, 20, 30), new RgbColor(40, 50, 60));
    yield return createData(new RgbwColor(10, 20, 30, 255), new RgbwColor(40, 50, 60, 128));
    yield return createData(new RgbwwColor(10, 20, 30, 128, 255), new RgbwwColor(40, 50, 60, 64, 128));
    yield return createData(new HsColor(10, 20), new HsColor(30, 40));
    yield return createData(new XyColor(0.2f, 0.6f), new XyColor(0.4f, 0.8f));
    yield return createData(new NameColor("test_color"), new NameColor("new_color"));
    yield return createData(new KelvinTemperatureColor(1337), new KelvinTemperatureColor(2001));
    yield return createData(new MiredsTemperatureColor(256), new MiredsTemperatureColor(106));
  }

  [Test]
  [TestCaseSource(nameof(ReadJsonWithExisingValueTestCases))]
  public void ReadJsonWithExisingValue(Color existing, Color color)
  {
    StringReader textReader = new StringReader(GetJsonRepresentation(color));
    JsonTextReader jsonReader = new JsonTextReader(textReader);
    JsonSerializer serializer = JsonSerializer.Create();
    object result = converter.ReadJson(jsonReader, color.GetType(), existing, serializer);

    Assert.NotNull(result);
    Assert.AreEqual(existing, result);
    Assert.AreEqual(color.ToString(), result.ToString());
  }

  private string GetJsonRepresentation(Color color)
  {
    if (color is NameColor)
      return $"\"{color}\"";
    else if (color is KelvinTemperatureColor kelvinColor)
      return kelvinColor.Kelvins.ToString();
    else if (color is MiredsTemperatureColor miredsColor)
      return miredsColor.Mireds.ToString();
    else
      return color.ToString().Replace(" ", string.Empty);
  }
}