using System;
using System.Reflection;
using System.Runtime.Serialization;
using HassClient.Core.Models.Events;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Serialization;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace HassClient.Core.Tests;

[TestFixture(TestOf = typeof(HassSerializer))]
public class SerializerTests
{
  private const string expectedTestValueEnumResult = "test_value";
  private const string expectedEnumMemberTestValueEnumResult = "customized_name";
  private const string expectedTestPropertyResult = "test_property";
  private const string expectedTestFieldResult = "test_field";

  private enum TestEnum
  {
    DefaultValue = 0,

    TestValue,

    [EnumMember(Value = expectedEnumMemberTestValueEnumResult)]
    EnumMemberTestValue
  }

  private class TestClass
  {
    public string TestField;
    public string TestProperty { get; set; }
  }

  [Test]
  public void EnumToSnakeCase()
  {
    string result = TestEnum.TestValue.ToSnakeCase();

    Assert.NotNull(result);
    Assert.AreEqual(expectedTestValueEnumResult, result);
  }

  [Test]
  public void EnumToSnakeCasePriorizesEnumMemberAttribute()
  {
    TestEnum value = TestEnum.EnumMemberTestValue;
    MemberInfo[] memInfo = typeof(TestEnum).GetMember(value.ToString());
    string attribValue = memInfo[0].GetCustomAttribute<EnumMemberAttribute>().Value;
    string result = value.ToSnakeCase();

    Assert.NotNull(result);
    Assert.AreEqual(attribValue, result);
  }

  [Test]
  [TestCase(KnownDomains.Automation)]
  [TestCase(KnownEventTypes.AreaRegistryUpdated)]
  [TestCase(KnownServices.AddonRestart)]
  public void EnumToSnakeCaseWithKnownEnumThrows<T>(T value)
    where T : Enum
  {
    Assert.Throws<InvalidOperationException>(() => value.ToSnakeCase());
  }

  [Test]
  public void TryGetEnumFromSnakeCase()
  {
    bool success = HassSerializer.TryGetEnumFromSnakeCase(expectedTestValueEnumResult, out TestEnum result);

    Assert.IsTrue(success);
    Assert.AreEqual(TestEnum.TestValue, result);
  }

  [Test]
  public void TryGetEnumFromSnakeCaseWithInvalidValue()
  {
    bool success = HassSerializer.TryGetEnumFromSnakeCase("invalid_value", out TestEnum result);

    Assert.IsFalse(success);
    Assert.AreEqual(default(TestEnum), result);
  }

  [Test]
  public void EnumValuesAreConvertedToSnakeCase()
  {
    TestEnum value = TestEnum.TestValue;
    string result = HassSerializer.SerializeObject(value);

    Assert.NotNull(result);
    Assert.AreEqual($"\"{expectedTestValueEnumResult}\"", result);
  }

  [Test]
  public void EnumValuesAreConvertedToSnakeCasePriorizingEnumMemberAttribute()
  {
    TestEnum value = TestEnum.EnumMemberTestValue;
    MemberInfo[] memInfo = typeof(TestEnum).GetMember(value.ToString());
    string attribValue = memInfo[0].GetCustomAttribute<EnumMemberAttribute>().Value;
    string result = HassSerializer.SerializeObject(value);

    Assert.NotNull(result);
    Assert.AreEqual($"\"{attribValue}\"", result);
  }

  [Test]
  public void EnumValuesAreConvertedFromSnakeCase()
  {
    TestEnum result = HassSerializer.DeserializeObject<TestEnum>($"\"{expectedTestValueEnumResult}\"");

    Assert.NotNull(result);
    Assert.AreEqual(TestEnum.TestValue, result);
  }

  [Test]
  public void EnumValuesAreConvertedFromSnakeCasePriorizingEnumMemberAttribute()
  {
    TestEnum value = TestEnum.EnumMemberTestValue;
    MemberInfo[] memInfo = typeof(TestEnum).GetMember(value.ToString());
    string attribValue = memInfo[0].GetCustomAttribute<EnumMemberAttribute>().Value;
    TestEnum result = HassSerializer.DeserializeObject<TestEnum>($"\"{attribValue}\"");

    Assert.NotNull(result);
    Assert.AreEqual(value, result);
  }

  [Test]
  public void PropertiesAreConvertedToSnakeCase()
  {
    TestClass value = new() { TestProperty = nameof(TestClass.TestProperty) };
    string result = HassSerializer.SerializeObject(value);

    Assert.NotNull(result);
    Assert.IsTrue(result.Contains($"\"{expectedTestPropertyResult}\":\"{value.TestProperty}\""));
  }

  [Test]
  public void PropertiesAreConvertedFromSnakeCase()
  {
    TestClass result =
      HassSerializer.DeserializeObject<TestClass>(
        $"{{\"{expectedTestPropertyResult}\":\"{nameof(TestClass.TestProperty)}\"}}");

    Assert.NotNull(result);
    Assert.AreEqual(nameof(TestClass.TestProperty), result.TestProperty);
  }

  [Test]
  public void FieldsAreConvertedToSnakeCase()
  {
    TestClass value = new() { TestField = nameof(TestClass.TestField) };
    string result = HassSerializer.SerializeObject(value);

    Assert.NotNull(result);
    Assert.IsTrue(result.Contains($"\"{expectedTestFieldResult}\":\"{value.TestField}\""));
  }

  [Test]
  public void FieldsAreConvertedFromSnakeCase()
  {
    TestClass result =
      HassSerializer.DeserializeObject<TestClass>(
        $"{{\"{expectedTestFieldResult}\":\"{nameof(TestClass.TestField)}\"}}");

    Assert.NotNull(result);
    Assert.AreEqual(nameof(TestClass.TestField), result.TestField);
  }

  [Test]
  public void JObjectPropertiesAreConvertedToSnakeCase()
  {
    TestClass value = new() { TestProperty = nameof(TestClass.TestProperty) };
    JObject result = HassSerializer.CreateJObject(value);

    Assert.NotNull(result);
    Assert.AreEqual(value.TestProperty, result.GetValue(expectedTestPropertyResult).ToString());
  }

  [Test]
  public void JObjectFieldsAreConvertedToSnakeCase()
  {
    TestClass value = new() { TestField = nameof(TestClass.TestField) };
    JObject result = HassSerializer.CreateJObject(value);

    Assert.NotNull(result);
    Assert.AreEqual(value.TestField, result.GetValue(expectedTestFieldResult).ToString());
  }

  [Test]
  public void JObjectWithSelectedProperties()
  {
    string[] selectedProperties = { nameof(TestClass.TestProperty) };
    JObject result = HassSerializer.CreateJObject(new TestClass(), selectedProperties);

    Assert.NotNull(result);
    Assert.AreEqual(1, result.Count);
    Assert.IsTrue(result.ContainsKey(expectedTestPropertyResult));
    Assert.IsFalse(result.ContainsKey(expectedTestFieldResult));
  }

  [Test]
  public void JObjectFromNullThrows()
  {
    Assert.Throws<ArgumentNullException>(() => HassSerializer.CreateJObject(null));
  }
}