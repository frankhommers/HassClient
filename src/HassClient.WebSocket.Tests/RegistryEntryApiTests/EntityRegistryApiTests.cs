using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests.RegistryEntryApiTests;

public class EntityRegistryApiTests : BaseHassWsApiTest
{
  private string testEntityId;
  private InputBoolean testInputBoolean;

  protected override async Task OneTimeSetUp()
  {
    await base.OneTimeSetUp();
    testInputBoolean = new InputBoolean(MockHelpers.GetRandomTestName(), "mdi:switch");
    bool result = await HassWsApi.CreateStorageEntityRegistryEntryAsync(testInputBoolean);
    testEntityId = testInputBoolean.EntityId;

    Assert.IsTrue(result, "SetUp failed");
  }

  protected override async Task OneTimeTearDown()
  {
    await base.OneTimeTearDown();
    await HassWsApi.DeleteStorageEntityRegistryEntryAsync(testInputBoolean);
  }

  [Test]
  public async Task GetEntities()
  {
    IEnumerable<EntityRegistryEntry> entities = await HassWsApi.GetEntitiesAsync();

    Assert.IsNotNull(entities);
    Assert.IsNotEmpty(entities);
    Assert.IsTrue(entities.All(e => e.EntityId != null));
    Assert.IsTrue(entities.All(e => e.Platform != null), entities.FirstOrDefault(e => e.Platform == null)?.EntityId);
    Assert.IsTrue(entities.Any(e => e.ConfigEntryId != null));
  }

  [Test]
  public void GetEntityWithNullEntityIdThrows()
  {
    Assert.ThrowsAsync<ArgumentException>(() => HassWsApi.GetEntityAsync(null));
  }

  [Test]
  public void UpdateEntityWithSameEntityIdThrows()
  {
    EntityRegistryEntry testEntity = new("switch.TestEntity", null, null);

    Assert.ThrowsAsync<ArgumentException>(() => HassWsApi.UpdateEntityAsync(testEntity, testEntity.EntityId));
  }

  [Test]
  public async Task GetEntity()
  {
    string entityId = "light.bed_light";
    EntityRegistryEntry entity = await HassWsApi.GetEntityAsync(entityId);

    Assert.IsNotNull(entity);
    Assert.IsNotNull(entity.ConfigEntryId);
    Assert.IsNotNull(entity.OriginalName);
    Assert.IsNotNull(entity.Name);
    Assert.AreEqual(entityId, entity.EntityId);
  }

  [Test]
  [Order(1)]
  [NonParallelizable]
  public async Task GetCreatedEntity()
  {
    EntityRegistryEntry entity = await HassWsApi.GetEntityAsync(testEntityId);

    Assert.IsNotNull(entity);
    Assert.IsNotNull(entity.OriginalName);
    Assert.IsNotNull(entity.OriginalIcon);
    Assert.IsNotNull(entity.Name);
    Assert.IsNotNull(entity.Icon);
    Assert.AreEqual(testEntityId, entity.EntityId);
  }

  [Order(1)]
  [NonParallelizable]
  [TestCase(true)]
  [TestCase(false)]
  public async Task UpdateEntityDisable(bool disable)
  {
    EntityRegistryEntry testEntity = await HassWsApi.GetEntityAsync(testEntityId);

    bool result = await HassWsApi.UpdateEntityAsync(testEntity, disable: disable);

    Assert.IsTrue(result);
    Assert.AreEqual(testEntityId, testEntity.EntityId);
    Assert.AreEqual(disable, testEntity.IsDisabled);
  }

  [Test]
  [Order(1)]
  [NonParallelizable]
  public async Task UpdateEntityName()
  {
    string newName = MockHelpers.GetRandomTestName();
    EntityRegistryEntry testEntity = await HassWsApi.GetEntityAsync(testEntityId);

    testEntity.Name = newName;
    bool result = await HassWsApi.UpdateEntityAsync(testEntity);

    Assert.IsTrue(result);
    Assert.AreEqual(testEntityId, testEntity.EntityId);
    Assert.AreEqual(newName, testEntity.Name);
    Assert.AreNotEqual(newName, testEntity.OriginalName);
  }

  [Test]
  [Order(1)]
  [NonParallelizable]
  public async Task UpdateEntityIcon()
  {
    EntityRegistryEntry testEntity = await HassWsApi.GetEntityAsync(testEntityId);

    string newIcon = "mdi:fan";
    testEntity.Icon = newIcon;
    bool result = await HassWsApi.UpdateEntityAsync(testEntity);

    Assert.IsTrue(result);
    Assert.AreEqual(testEntityId, testEntity.EntityId);
    Assert.AreEqual(newIcon, testEntity.Icon);
    Assert.AreNotEqual(newIcon, testEntity.OriginalIcon);
  }

  [Test]
  [Order(1)]
  public async Task RefreshEntity()
  {
    EntityRegistryEntry testEntity = await HassWsApi.GetEntityAsync(testEntityId);
    EntityRegistryEntry clonedEntity = testEntity.Clone();
    clonedEntity.Name = MockHelpers.GetRandomTestName();
    bool result = await HassWsApi.UpdateEntityAsync(clonedEntity);
    Assert.IsTrue(result, "SetUp failed");
    Assert.False(testEntity.HasPendingChanges, "SetUp failed");

    result = await HassWsApi.RefreshEntityAsync(testEntity);
    Assert.IsTrue(result);
    Assert.AreEqual(clonedEntity.Name, testEntity.Name);
  }

  [Test]
  [Order(2)]
  [NonParallelizable]
  public async Task UpdateEntityId()
  {
    EntityRegistryEntry testEntity = await HassWsApi.GetEntityAsync(testEntityId);
    string newEntityId = testEntityId + 1;

    bool result = await HassWsApi.UpdateEntityAsync(testEntity, newEntityId);

    Assert.IsTrue(result);
    Assert.AreEqual(newEntityId, testEntity.EntityId);
    Assert.AreNotEqual(testEntityId, newEntityId);

    testEntityId = newEntityId; // This is needed for DeleteEntityTest
  }

  [Test]
  [Order(3)]
  public async Task UpdateWithForce()
  {
    EntityRegistryEntry testEntity = await HassWsApi.GetEntityAsync(testEntityId);
    string initialName = testEntity.Name;
    string initialIcon = testEntity.Icon;
    DisabledByEnum initialDisabledBy = testEntity.DisabledBy;
    EntityRegistryEntry clonedEntry = testEntity.Clone();
    clonedEntry.Name = $"{initialName}_cloned";
    clonedEntry.Icon = $"{initialIcon}_cloned";
    bool result = await HassWsApi.UpdateEntityAsync(clonedEntry, disable: true);
    Assert.IsTrue(result, "SetUp failed");
    Assert.False(testEntity.HasPendingChanges, "SetUp failed");

    result = await HassWsApi.UpdateEntityAsync(testEntity, disable: false, forceUpdate: true);
    Assert.IsTrue(result);
    Assert.AreEqual(initialName, testEntity.Name);
    Assert.AreEqual(initialIcon, testEntity.Icon);
    Assert.AreEqual(initialDisabledBy, testEntity.DisabledBy);
  }

  [Test]
  [Order(4)]
  [NonParallelizable]
  public async Task DeleteEntity()
  {
    EntityRegistryEntry testEntity = await HassWsApi.GetEntityAsync(testEntityId);
    bool result = await HassWsApi.DeleteEntityAsync(testEntity);
    EntityRegistryEntry testEntity1 = await HassWsApi.GetEntityAsync(testEntityId);

    Assert.IsTrue(result);
    Assert.IsNull(testEntity1);
    Assert.IsFalse(testEntity.IsTracked);
  }
}