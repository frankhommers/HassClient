using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests.RegistryEntryApiTests.StorageEntityRegistryEntryApiTests;

public class InputBooleanApiTests : BaseHassWsApiTest
{
  private InputBoolean _testInputBoolean;

  [OneTimeSetUp]
  [Test]
  [Order(1)]
  public async Task CreateInputBoolean()
  {
    if (_testInputBoolean == null)
    {
      _testInputBoolean = new InputBoolean(MockHelpers.GetRandomTestName(), "mdi:fan", true);
      bool result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(_testInputBoolean);

      Assert.IsTrue(result, "SetUp failed");
      return;
    }

    Assert.NotNull(_testInputBoolean);
    Assert.NotNull(_testInputBoolean.UniqueId);
    Assert.NotNull(_testInputBoolean.Name);
    Assert.IsFalse(_testInputBoolean.HasPendingChanges);
    Assert.IsTrue(_testInputBoolean.IsTracked);
  }

  [Test]
  [Order(2)]
  public async Task GetInputBooleans()
  {
    IEnumerable<InputBoolean> result = await HassWsApi.GetStorageEntityRegistryEntriesAsync<InputBoolean>();

    Assert.NotNull(result);
    Assert.IsNotEmpty(result);
    Assert.IsTrue(result.Contains(_testInputBoolean));
    Assert.IsTrue(result.All(x => x.Id != null));
    Assert.IsTrue(result.All(x => x.UniqueId != null));
    Assert.IsTrue(result.All(x => x.EntityId.StartsWith("input_boolean.")));
    Assert.IsTrue(result.Any(x => x.Name != null));
    Assert.IsTrue(result.Any(x => x.Initial));
    Assert.IsTrue(result.Any(x => x.Icon != null));
    Assert.IsFalse(_testInputBoolean.HasPendingChanges);
  }

  [Test]
  [Order(3)]
  public async Task UpdateInputBooleanName()
  {
    _testInputBoolean.Name = $"{nameof(InputBooleanApiTests)}_{DateTime.Now.Ticks}";
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean);

    Assert.IsTrue(result);
    Assert.IsFalse(_testInputBoolean.HasPendingChanges);
  }

  [Test]
  [Order(4)]
  public async Task UpdateInputBooleanInitial()
  {
    _testInputBoolean.Initial = false;
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean);

    Assert.IsTrue(result);
    Assert.IsFalse(_testInputBoolean.HasPendingChanges);
  }

  [Test]
  [Order(5)]
  public async Task UpdateInputBooleanIcon()
  {
    _testInputBoolean.Icon = "mdi:lightbulb";
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean);

    Assert.IsTrue(result);
    Assert.IsFalse(_testInputBoolean.HasPendingChanges);
  }

  [Test]
  [Order(6)]
  public async Task UpdateWithForce()
  {
    string initialName = _testInputBoolean.Name;
    string initialIcon = _testInputBoolean.Icon;
    bool initialInitial = _testInputBoolean.Initial;
    InputBoolean clonedEntry = _testInputBoolean.Clone();
    clonedEntry.Name = $"{initialName}_cloned";
    clonedEntry.Icon = $"{initialIcon}_cloned";
    clonedEntry.Initial = !initialInitial;
    bool result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(clonedEntry);
    Assert.IsTrue(result, "SetUp failed");
    Assert.False(_testInputBoolean.HasPendingChanges, "SetUp failed");

    result = await HassWsApi.UpdateStorageEntityRegistryEntryAsync(_testInputBoolean, true);
    Assert.IsTrue(result);
    Assert.AreEqual(initialName, _testInputBoolean.Name);
    Assert.AreEqual(initialIcon, _testInputBoolean.Icon);
    Assert.AreEqual(initialInitial, _testInputBoolean.Initial);
    Assert.IsFalse(_testInputBoolean.HasPendingChanges);
  }

  [OneTimeTearDown]
  [Test]
  [Order(7)]
  public async Task DeleteInputBoolean()
  {
    if (_testInputBoolean == null) return;

    bool result = await HassWsApi.DeleteStorageEntityRegistryEntryAsync(_testInputBoolean);
    InputBoolean deletedInputBoolean = _testInputBoolean;
    _testInputBoolean = null;

    Assert.IsTrue(result);
    Assert.IsFalse(deletedInputBoolean.IsTracked);
  }
}