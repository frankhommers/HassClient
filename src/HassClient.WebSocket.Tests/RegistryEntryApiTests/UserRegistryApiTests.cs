using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests.RegistryEntryApiTests;

public class UserRegistryApiTests : BaseHassWsApiTest
{
  private User testUser;

  [OneTimeSetUp]
  [Test]
  [Order(1)]
  public async Task CreateUser()
  {
    if (testUser == null)
    {
      testUser = new User(MockHelpers.GetRandomTestName());
      bool result = await HassWsApi.CreateUserAsync(testUser);

      Assert.IsTrue(result, "SetUp failed");
      return;
    }

    Assert.NotNull(testUser.Id);
    Assert.NotNull(testUser.Name);
    Assert.IsTrue(testUser.IsActive);
    Assert.IsFalse(testUser.IsLocalOnly);
    Assert.IsFalse(testUser.IsOwner);
    Assert.IsFalse(testUser.IsAdministrator);
    Assert.IsFalse(testUser.HasPendingChanges);
    Assert.IsTrue(testUser.IsTracked);
  }

  [Test]
  [Order(2)]
  public async Task GetUsers()
  {
    IEnumerable<User> users = await HassWsApi.GetUsersAsync();

    Assert.NotNull(users);
    Assert.IsNotEmpty(users);
    Assert.IsTrue(users.Contains(testUser));
    Assert.IsTrue(users.Any(u => u.IsOwner));
    Assert.IsTrue(users.Any(u => u.IsAdministrator));
  }

  [Test]
  [Order(3)]
  public async Task UpdateUserName()
  {
    string updatedName = MockHelpers.GetRandomTestName();
    testUser.Name = updatedName;
    bool result = await HassWsApi.UpdateUserAsync(testUser);

    Assert.IsTrue(result);
    Assert.AreEqual(updatedName, testUser.Name);
  }

  [Test]
  [Order(3)]
  public async Task UpdateUserIsActive()
  {
    testUser.IsActive = false;
    bool result = await HassWsApi.UpdateUserAsync(testUser);

    Assert.IsTrue(result);
    Assert.IsFalse(testUser.IsActive);
  }

  [Test]
  [Order(3)]
  public async Task UpdateUserIsLocalOnly()
  {
    testUser.IsLocalOnly = true;
    bool result = await HassWsApi.UpdateUserAsync(testUser);

    Assert.IsTrue(result);
    Assert.IsTrue(testUser.IsLocalOnly);
  }

  [Test]
  [Order(3)]
  public async Task UpdateUserIsAdministrator()
  {
    testUser.IsAdministrator = true;
    bool result = await HassWsApi.UpdateUserAsync(testUser);

    Assert.IsTrue(result);
    Assert.IsTrue(testUser.IsAdministrator);
  }

  [Test]
  [Order(4)]
  public async Task UpdateWithForce()
  {
    string initialName = testUser.Name;
    ICollection<string> initialGroupIds = testUser.GroupIds;
    bool initialIsActive = testUser.IsActive;
    bool initialIsLocalOnly = testUser.IsLocalOnly;
    User clonedEntry = testUser.Clone();
    clonedEntry.Name = $"{initialName}_cloned";
    clonedEntry.IsAdministrator = !testUser.IsAdministrator;
    clonedEntry.IsActive = !initialIsActive;
    clonedEntry.IsLocalOnly = !initialIsLocalOnly;
    bool result = await HassWsApi.UpdateUserAsync(clonedEntry);
    Assert.IsTrue(result, "SetUp failed");
    Assert.False(testUser.HasPendingChanges, "SetUp failed");

    result = await HassWsApi.UpdateUserAsync(testUser, true);
    Assert.IsTrue(result);
    Assert.AreEqual(initialName, testUser.Name);
    Assert.AreEqual(initialGroupIds, testUser.GroupIds);
    Assert.AreEqual(initialIsActive, testUser.IsActive);
    Assert.AreEqual(initialIsLocalOnly, testUser.IsLocalOnly);
  }

  [OneTimeTearDown]
  [Test]
  [Order(5)]
  public async Task DeleteUser()
  {
    if (testUser == null) return;

    bool result = await HassWsApi.DeleteUserAsync(testUser);
    User deletedUser = testUser;
    testUser = null;

    Assert.IsTrue(result);
    Assert.IsFalse(deletedUser.IsTracked);
  }
}