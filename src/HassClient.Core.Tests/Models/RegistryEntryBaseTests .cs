using System;
using System.Collections.Generic;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Serialization;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(EntityRegistryEntryBase))]
public class RegistryEntryBaseTests
{
  private class TestRegistryEntry : EntityRegistryEntryBase
  {
    public TestRegistryEntry(string name, string icon = null)
      : base(name, icon)
    {
    }

    public TestRegistryEntry()
      : this(MockHelpers.GetRandomTestName(), "mdi:camera")
    {
    }

    public override string EntityId => MockHelpers.GetRandomEntityId(KnownDomains.Camera);

    protected internal override string UniqueId { get; set; }

    public static TestRegistryEntry CreateUnmodified(out string initialName, out string initialIcon)
    {
      TestRegistryEntry result = new();
      initialName = result.Name;
      initialIcon = result.Icon;
      result.SaveChanges();
      return result;
    }
  }

  private class NullNameTestRegistryEntry : TestRegistryEntry
  {
    public NullNameTestRegistryEntry(string name, string icon = null)
      : base(name, icon)
    {
    }

    protected override bool AcceptsNullOrWhiteSpaceName => true;
  }

  [Test]
  public void DeserializedEntityRegistryEntryHasNoPendingChanges()
  {
    TestRegistryEntry testEntry = HassSerializer.DeserializeObject<TestRegistryEntry>("{}");
    Assert.IsFalse(testEntry.HasPendingChanges);
  }

  [Test]
  public void NewEntityRegistryEntryHasPendingChanges()
  {
    TestRegistryEntry testEntry = new();
    Assert.IsTrue(testEntry.HasPendingChanges);
  }

  [Test]
  public void NewEntityRegistryEntryIsUntracked()
  {
    TestRegistryEntry testEntry = new();
    Assert.False(testEntry.IsTracked);
  }

  public static IEnumerable<string> NullOrWhiteSpaceStringValues()
  {
    yield return null;
    yield return string.Empty;
    yield return " ";
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void NewEntityRegistryEntryWithNullOrWhiteSpaceNameWhenNotAcceptedThrows(string value)
  {
    Assert.Throws<ArgumentException>(() => new TestRegistryEntry(value));
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void NewEntityRegistryEntryWithNullOrWhiteSpaceNameWhenAcceptedDoesNotThrows(string value)
  {
    Assert.DoesNotThrow(() => new NullNameTestRegistryEntry(value));
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void SetNullOrWhiteSpaceNameWhenNotAcceptedThrows(string value)
  {
    TestRegistryEntry testEntry = new();
    Assert.Throws<InvalidOperationException>(() => testEntry.Name = value);
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void SetNullOrWhiteSpaceNameWhenAcceptedDoesNotThrows(string value)
  {
    NullNameTestRegistryEntry testEntry = new(MockHelpers.GetRandomTestName());
    Assert.DoesNotThrow(() => testEntry.Name = value);
  }

  [Test]
  public void SetNewNameMakesHasPendingChangesTrue()
  {
    TestRegistryEntry testEntry = TestRegistryEntry.CreateUnmodified(out string initialName, out _);

    testEntry.Name = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Name = initialName;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewIconMakesHasPendingChangesTrue()
  {
    TestRegistryEntry testEntry = TestRegistryEntry.CreateUnmodified(out _, out string initialIcon);

    testEntry.Icon = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Icon = initialIcon;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void DiscardPendingChanges()
  {
    TestRegistryEntry testEntry = TestRegistryEntry.CreateUnmodified(out string initialName, out string initialIcon);

    testEntry.Name = MockHelpers.GetRandomTestName();
    testEntry.Icon = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.DiscardPendingChanges();
    Assert.False(testEntry.HasPendingChanges);
    Assert.AreEqual(initialName, testEntry.Name);
    Assert.AreEqual(initialIcon, testEntry.Icon);
  }
}