using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests.RegistryEntryApiTests.StorageEntityRegistryEntryApiTests;

public class ZoneApiTests : BaseHassWsApiTest
{
  private Zone _testZone;

  [OneTimeSetUp]
  [Test]
  [Order(1)]
  public async Task CreateZone()
  {
    if (_testZone == null)
    {
      _testZone = new Zone(MockHelpers.GetRandomTestName(), 20.1f, 34.6f, 10.5f, "mdi:fan", true);
      bool result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testZone);

      Assert.IsTrue(result, "SetUp failed");
      return;
    }

    Assert.NotNull(_testZone);
    Assert.NotNull(_testZone.UniqueId);
    Assert.NotNull(_testZone.Name);
    Assert.IsFalse(_testZone.HasPendingChanges);
    Assert.IsTrue(_testZone.IsTracked);
  }

  [Test]
  [Order(2)]
  public async Task GetZones()
  {
    IEnumerable<Zone> result = await HassWsApi.GetStorageEntityRegistryEntriesAsync<Zone>();

    Assert.NotNull(result);
    Assert.IsNotEmpty(result);
    Assert.IsTrue(result.Contains(_testZone));
    Assert.IsTrue(result.All(x => x.Id != null));
    Assert.IsTrue(result.All(x => x.UniqueId != null));
    Assert.IsTrue(result.All(x => x.EntityId.StartsWith("zone.")));
    Assert.IsTrue(result.Any(x => x.Name != null));
    Assert.IsTrue(result.Any(x => x.Longitude > 0));
    Assert.IsTrue(result.Any(x => x.Latitude > 0));
    Assert.IsTrue(result.Any(x => x.Longitude != x.Latitude));
    Assert.IsTrue(result.Any(x => x.Radius > 0));
    Assert.IsTrue(result.Any(x => x.IsPassive));
    Assert.IsTrue(result.Any(x => x.Icon != null));
  }

  [Test]
  [Order(3)]
  public async Task UpdateZoneName()
  {
    _testZone.Name = $"{nameof(ZoneApiTests)}_{DateTime.Now.Ticks}";
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone);

    Assert.IsTrue(result);
    Assert.IsFalse(_testZone.HasPendingChanges);
  }

  [Test]
  [Order(4)]
  public async Task UpdateZoneInitial()
  {
    _testZone.IsPassive = false;
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone);

    Assert.IsTrue(result);
    Assert.IsFalse(_testZone.HasPendingChanges);
  }

  [Test]
  [Order(5)]
  public async Task UpdateZoneIcon()
  {
    _testZone.Icon = "mdi:lightbulb";
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone);

    Assert.IsTrue(result);
    Assert.IsFalse(_testZone.HasPendingChanges);
  }

  [Test]
  [Order(6)]
  public async Task UpdateWithForce()
  {
    string initialName = _testZone.Name;
    string initialIcon = _testZone.Icon;
    float initialLongitude = _testZone.Longitude;
    float initialLatitude = _testZone.Latitude;
    float initialRadius = _testZone.Radius;
    bool initialIsPassive = _testZone.IsPassive;
    Zone clonedEntry = _testZone.Clone();
    clonedEntry.Name = $"{initialName}_cloned";
    clonedEntry.Icon = $"{initialIcon}_cloned";
    clonedEntry.Longitude = initialLongitude + 15f;
    clonedEntry.Latitude = initialLatitude + 15f;
    clonedEntry.Radius = initialRadius + 15f;
    clonedEntry.IsPassive = !initialIsPassive;
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
    Assert.IsTrue(result, "SetUp failed");
    Assert.False(_testZone.HasPendingChanges, "SetUp failed");

    result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testZone, true);
    Assert.IsTrue(result);
    Assert.AreEqual(initialName, _testZone.Name);
    Assert.AreEqual(initialIcon, _testZone.Icon);
    Assert.AreEqual(initialLongitude, _testZone.Longitude);
    Assert.AreEqual(initialLatitude, _testZone.Latitude);
    Assert.AreEqual(initialRadius, _testZone.Radius);
    Assert.AreEqual(initialIsPassive, _testZone.IsPassive);
    Assert.IsFalse(_testZone.HasPendingChanges);
  }

  [OneTimeTearDown]
  [Test]
  [Order(7)]
  public async Task DeleteZone()
  {
    if (_testZone == null) return;

    bool result = await HassWsApi.DeleteStorageEntityRegistryEntryAsync(_testZone);
    Zone deletedZone = _testZone;
    _testZone = null;

    Assert.IsTrue(result);
    Assert.IsFalse(deletedZone.IsTracked);
  }
}