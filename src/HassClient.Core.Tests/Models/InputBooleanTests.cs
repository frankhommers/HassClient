using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(InputBoolean))]
public class InputBooleanTests
{
  [Test]
  public void HasPublicConstructorWithParameters()
  {
    ConstructorInfo constructor = typeof(InputBoolean).GetConstructors()
      .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
    Assert.NotNull(constructor);
  }

  [Test]
  public void NewInputBooleanHasPendingChanges()
  {
    InputBoolean testEntry = new(MockHelpers.GetRandomTestName());
    Assert.IsTrue(testEntry.HasPendingChanges);
  }

  [Test]
  public void NewInputBooleanIsUntracked()
  {
    InputBoolean testEntry = new(MockHelpers.GetRandomTestName());
    Assert.False(testEntry.IsTracked);
  }

  private static IEnumerable<string> NullOrWhiteSpaceStringValues()
  {
    return RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void NewInputBooleanWithNullOrWhiteSpaceNameThrows(string value)
  {
    Assert.Throws<ArgumentException>(() => new InputBoolean(value));
  }

  [Test]
  public void SetNewNameMakesHasPendingChangesTrue()
  {
    InputBoolean testEntry = CreateTestEntry(out _, out string initialName, out _, out _);

    testEntry.Name = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Name = initialName;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewIconMakesHasPendingChangesTrue()
  {
    InputBoolean testEntry = CreateTestEntry(out _, out _, out string initialIcon, out _);

    testEntry.Icon = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Icon = initialIcon;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewInitialMakesHasPendingChangesTrue()
  {
    InputBoolean testEntry = CreateTestEntry(out _, out _, out _, out bool initial);

    testEntry.Initial = !initial;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Initial = initial;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void DiscardPendingChanges()
  {
    InputBoolean testEntry = CreateTestEntry(out _, out string initialName, out string initialIcon, out bool initial);

    testEntry.Name = MockHelpers.GetRandomTestName();
    testEntry.Icon = MockHelpers.GetRandomTestName();
    testEntry.Initial = !initial;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.DiscardPendingChanges();
    Assert.False(testEntry.HasPendingChanges);
    Assert.AreEqual(initialName, testEntry.Name);
    Assert.AreEqual(initialIcon, testEntry.Icon);
    Assert.AreEqual(initial, testEntry.Initial);
  }

  private InputBoolean CreateTestEntry(out string entityId, out string name, out string icon, out bool initial)
  {
    entityId = MockHelpers.GetRandomEntityId(KnownDomains.InputBoolean);
    name = MockHelpers.GetRandomTestName();
    icon = "mdi:fan";
    initial = true;
    return InputBoolean.CreateUnmodified(entityId, name, icon, initial);
  }
}