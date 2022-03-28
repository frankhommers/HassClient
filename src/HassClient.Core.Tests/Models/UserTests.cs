using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(User))]
public class UserTests
{
  [Test]
  public void HasPublicConstructorWithParameters()
  {
    ConstructorInfo constructor = typeof(User).GetConstructors()
      .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
    Assert.NotNull(constructor);
  }

  [Test]
  public void NewUserHasPendingChanges()
  {
    User testEntry = new(MockHelpers.GetRandomTestName());
    Assert.IsTrue(testEntry.HasPendingChanges);
  }

  [Test]
  public void NewUserIsUntracked()
  {
    User testEntry = new(MockHelpers.GetRandomTestName());
    Assert.IsFalse(testEntry.IsTracked);
  }

  private static IEnumerable<string> NullOrWhiteSpaceStringValues()
  {
    return RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void NewUserWithNullOrWhiteSpaceNameThrows(string value)
  {
    Assert.Throws<ArgumentException>(() => new User(value));
  }

  [Test]
  public void SetNewNameMakesHasPendingChangesTrue()
  {
    User testEntry = CreateTestEntry(out string initialName);

    testEntry.Name = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Name = initialName;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewIsAdministratorMakesHasPendingChangesTrue()
  {
    User testEntry = CreateTestEntry(out _);

    testEntry.IsAdministrator = true;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.IsAdministrator = false;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void AddNewGroupIdMakesHasPendingChangesTrue()
  {
    string testGroupId = "TestGroupId";
    User testEntry = CreateTestEntry(out _);

    testEntry.GroupIds.Add(testGroupId);
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.GroupIds.Remove(testGroupId);
    Assert.False(testEntry.HasPendingChanges);
  }

  private User CreateTestEntry(out string name)
  {
    name = MockHelpers.GetRandomTestName();
    return User.CreateUnmodified("testId", name, false);
  }
}