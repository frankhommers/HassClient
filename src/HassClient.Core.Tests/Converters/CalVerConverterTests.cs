using System.IO;
using HassClient.Core.Models;
using HassClient.Core.Serialization.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HassClient.Core.Tests.Converters;

[TestFixture(TestOf = typeof(CalVerConverter))]
public class CalVerConverterTests
{
  private readonly CalVerConverter converter = new();

  private readonly CalVer testVersion = CalVer.Create("2022.02.4b3");

  [Test]
  public void CanConvertCalVer()
  {
    bool canConvert = converter.CanConvert(typeof(CalVer));

    Assert.True(canConvert);
  }

  [Test]
  public void WriteJson()
  {
    StringWriter textWriter = new();
    JsonTextWriter jsonWriter = new(textWriter);
    JsonSerializer serializer = JsonSerializer.Create();

    converter.WriteJson(jsonWriter, testVersion, serializer);

    Assert.AreEqual($"\"{testVersion}\"", textWriter.ToString());
  }

  [Test]
  public void ReadJson()
  {
    StringReader textReader = new($"\"{testVersion}\"");
    JsonTextReader jsonReader = new(textReader);
    JsonSerializer serializer = JsonSerializer.Create();
    object result = converter.ReadJson(jsonReader, testVersion.GetType(), null, serializer);

    Assert.NotNull(result);
    Assert.AreNotEqual(testVersion, result);
    Assert.AreEqual(testVersion.ToString(), result.ToString());
  }

  public void ReadJsonWithExisingValue()
  {
    CalVer existingVersion = CalVer.Create("2021.05.7b1");

    StringReader textReader = new(testVersion.ToString());
    JsonTextReader jsonReader = new(textReader);
    JsonSerializer serializer = JsonSerializer.Create();
    object result = converter.ReadJson(jsonReader, testVersion.GetType(), existingVersion, serializer);

    Assert.NotNull(result);
    Assert.AreEqual(existingVersion, result);
    Assert.AreEqual(testVersion.ToString(), result.ToString());
  }
}