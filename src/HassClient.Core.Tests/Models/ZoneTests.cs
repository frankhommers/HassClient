using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(Zone))]
public class ZoneTests
{
  [Test]
  public void HasPublicConstructorWithParameters()
  {
    ConstructorInfo constructor = typeof(Zone).GetConstructors()
      .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
    Assert.NotNull(constructor);
  }

  [Test]
  public void NewZoneHasPendingChanges()
  {
    Zone testEntry = new(MockHelpers.GetRandomTestName(), 20, 30, 5);
    Assert.IsTrue(testEntry.HasPendingChanges);
  }

  [Test]
  public void NewZoneIsUntracked()
  {
    Zone testEntry = new(MockHelpers.GetRandomTestName(), 20, 30, 5);
    Assert.IsFalse(testEntry.IsTracked);
  }

  private static IEnumerable<string> NullOrWhiteSpaceStringValues()
  {
    return RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void NewZoneWithNullOrWhiteSpaceNameThrows(string value)
  {
    Assert.Throws<ArgumentException>(() => new Zone(value, 20, 30, 5));
  }

  [Test]
  public void SetNewNameMakesHasPendingChangesTrue()
  {
    Zone testEntry = CreateTestEntry(out string initialName, out _, out _, out _, out _, out _);

    testEntry.Name = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Name = initialName;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewIconMakesHasPendingChangesTrue()
  {
    Zone testEntry = CreateTestEntry(out _, out string initialIcon, out _, out _, out _, out _);

    testEntry.Icon = "mdi:test";
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Icon = initialIcon;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewLongitudeMakesHasPendingChangesTrue()
  {
    Zone testEntry = CreateTestEntry(out _, out _, out float initialLongitude, out _, out _, out _);

    testEntry.Longitude += 10;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Longitude = initialLongitude;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewLatitudeMakesHasPendingChangesTrue()
  {
    Zone testEntry = CreateTestEntry(out _, out _, out _, out float initialLatitude, out _, out _);

    testEntry.Latitude += 10;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Latitude = initialLatitude;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewRadiusMakesHasPendingChangesTrue()
  {
    Zone testEntry = CreateTestEntry(out _, out _, out _, out _, out float initialRadius, out _);

    testEntry.Radius += 10;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Radius = initialRadius;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewIsPassiveMakesHasPendingChangesTrue()
  {
    Zone testEntry = CreateTestEntry(out _, out _, out _, out _, out _, out bool initialIsPassive);

    testEntry.IsPassive = !initialIsPassive;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.IsPassive = initialIsPassive;
    Assert.False(testEntry.HasPendingChanges);
  }

  private Zone CreateTestEntry(out string name, out string icon, out float longitude, out float latitude,
    out float radius, out bool isPassive)
  {
    name = MockHelpers.GetRandomTestName();
    icon = "mdi:zone";
    longitude = 20;
    latitude = 30;
    radius = 5;
    isPassive = false;
    return Zone.CreateUnmodified("testId", name, longitude, latitude, radius, icon, isPassive);
  }
}