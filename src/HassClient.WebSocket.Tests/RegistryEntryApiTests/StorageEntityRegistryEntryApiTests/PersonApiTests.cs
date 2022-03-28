using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests.RegistryEntryApiTests.StorageEntityRegistryEntryApiTests;

public class PersonApiTests : BaseHassWsApiTest
{
  private Person _testPerson;

  [OneTimeSetUp]
  [Test]
  [Order(1)]
  public async Task CreatePerson()
  {
    if (_testPerson == null)
    {
      User testUser = new(MockHelpers.GetRandomTestName(), false);
      bool result = await HassWsApi.CreateUserAsync(testUser);
      Assert.IsTrue(result, "SetUp failed");

      _testPerson = new Person(testUser.Name, testUser);
      result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testPerson);
      Assert.IsTrue(result, "SetUp failed");
      return;
    }

    Assert.NotNull(_testPerson);
    Assert.NotNull(_testPerson.UniqueId);
    Assert.NotNull(_testPerson.Name);
    Assert.IsFalse(_testPerson.HasPendingChanges);
    Assert.IsTrue(_testPerson.IsTracked);
  }

  [Test]
  [Order(2)]
  public async Task GetPersons()
  {
    IEnumerable<Person> result = await HassWsApi.GetStorageEntityRegistryEntriesAsync<Person>();

    Assert.NotNull(result);
    Assert.IsNotEmpty(result);
    Assert.IsTrue(result.Contains(_testPerson));
    Assert.IsTrue(result.All(x => x.Id != null));
    Assert.IsTrue(result.All(x => x.UniqueId != null));
    Assert.IsTrue(result.All(x => x.EntityId.StartsWith("person.")));
    Assert.IsTrue(result.Any(x => x.Name != null));
    Assert.IsFalse(_testPerson.HasPendingChanges);
  }

  [Test]
  [Order(3)]
  public async Task UpdatePersonName()
  {
    _testPerson.Name = $"{nameof(PersonApiTests)}_{DateTime.Now.Ticks}";
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

    Assert.IsTrue(result);
    Assert.IsFalse(_testPerson.HasPendingChanges);
  }

  [Test]
  [Order(3)]
  public async Task UpdatePersonPicture()
  {
    _testPerson.Picture = "test/Picture.png";
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

    Assert.IsTrue(result);
    Assert.IsFalse(_testPerson.HasPendingChanges);
  }

  [Test]
  [Order(3)]
  public async Task UpdatePersonDeviceTrackers()
  {
    _testPerson.DeviceTrackers.Add($"device_tracker.{MockHelpers.GetRandomTestName()}");
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

    Assert.IsTrue(result);
    Assert.IsFalse(_testPerson.HasPendingChanges);
  }

  [Test]
  [Order(3)]
  public async Task UpdatePersonUserId()
  {
    User testUser = new(MockHelpers.GetRandomTestName(), false);
    bool result = await HassWsApi.CreateUserAsync(testUser);
    Assert.IsTrue(result, "SetUp failed");

    _testPerson.ChangeUser(testUser);
    result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson);

    Assert.IsTrue(result);
    Assert.IsFalse(_testPerson.HasPendingChanges);
  }

  [Test]
  [Order(4)]
  public async Task UpdateWithForce()
  {
    string initialName = _testPerson.Name;
    Person clonedEntry = _testPerson.Clone();
    clonedEntry.Name = $"{initialName}_cloned";
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
    Assert.IsTrue(result, "SetUp failed");
    Assert.False(_testPerson.HasPendingChanges, "SetUp failed");

    result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testPerson, true);
    Assert.IsTrue(result);
    Assert.AreEqual(initialName, _testPerson.Name);
    Assert.IsFalse(_testPerson.HasPendingChanges);
  }

  [OneTimeTearDown]
  [Test]
  [Order(5)]
  public async Task DeletePerson()
  {
    if (_testPerson == null) return;

    bool result = await HassWsApi.DeleteStorageEntityRegistryEntryAsync(_testPerson);
    Person deletedPerson = _testPerson;
    _testPerson = null;

    Assert.IsTrue(result);
    Assert.IsFalse(deletedPerson.IsTracked);
  }
}