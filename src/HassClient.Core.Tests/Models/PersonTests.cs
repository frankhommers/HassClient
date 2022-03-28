using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Models.RegistryEntries.StorageEntities;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(Person))]
public class PersonTests
{
  private readonly User testUser = User.CreateUnmodified("test", MockHelpers.GetRandomTestName(), false);

  [Test]
  public void HasPublicConstructorWithParameters()
  {
    ConstructorInfo constructor = typeof(Person).GetConstructors()
      .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
    Assert.NotNull(constructor);
  }

  [Test]
  public void NewPersonHasPendingChanges()
  {
    Person testEntry = new(MockHelpers.GetRandomTestName(), testUser);
    Assert.IsTrue(testEntry.HasPendingChanges);
  }

  [Test]
  public void NewPersonIsStorageEntry()
  {
    Person testEntry = new(MockHelpers.GetRandomTestName(), testUser);
    Assert.IsTrue(testEntry.IsStorageEntry);
  }

  [Test]
  public void NewPersonIsUntracked()
  {
    Person testEntry = new(MockHelpers.GetRandomTestName(), testUser);
    Assert.False(testEntry.IsTracked);
  }

  [Test]
  public void NewPersonHasUserId()
  {
    Person testEntry = new(MockHelpers.GetRandomTestName(), testUser);
    Assert.AreEqual(testEntry.UserId, testUser.Id);
  }

  private static IEnumerable<string> NullOrWhiteSpaceStringValues()
  {
    return RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void NewPersonWithNullOrWhiteSpaceNameThrows(string value)
  {
    Assert.Throws<ArgumentException>(() => new Person(value, testUser));
  }

  [Test]
  public void SetNewIconThrows()
  {
    Person testEntry = CreateTestEntry(out _, out _, out User _, out _, out _);

    Assert.Throws<InvalidOperationException>(() => testEntry.Icon = MockHelpers.GetRandomTestName());
  }

  [Test]
  public void SetNewInvalidDeviceTrackerThrows()
  {
    Person testEntry = CreateTestEntry(out _, out _, out User _, out _, out _);

    Assert.Throws<InvalidOperationException>(
      () => testEntry.DeviceTrackers.Add(MockHelpers.GetRandomEntityId(KnownDomains.Camera)));
  }

  [Test]
  public void SetNewNameMakesHasPendingChangesTrue()
  {
    Person testEntry = CreateTestEntry(out _, out string initialName, out _, out _, out _);

    testEntry.Name = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Name = initialName;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewPictureMakesHasPendingChangesTrue()
  {
    Person testEntry = CreateTestEntry(out _, out _, out _, out string picture, out _);

    testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Picture = picture;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewUserMakesHasPendingChangesTrue()
  {
    Person testEntry = CreateTestEntry(out _, out _, out User user, out _, out _);

    testEntry.ChangeUser(testUser);
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.ChangeUser(user);
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewDeviceTrackerMakesHasPendingChangesTrue()
  {
    string testDeviceTracker = MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker);
    Person testEntry = CreateTestEntry(out _, out _, out _, out _, out IEnumerable<string> deviceTrackers);

    testEntry.DeviceTrackers.Add(testDeviceTracker);
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.DeviceTrackers.Remove(testDeviceTracker);
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void DiscardPendingChanges()
  {
    Person testEntry = CreateTestEntry(out _, out string initialName, out User initialUser, out string initialPicture,
      out IEnumerable<string> initialDeviceTrackers);

    testEntry.Name = MockHelpers.GetRandomTestName();
    testEntry.ChangeUser(testUser);
    testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
    testEntry.DeviceTrackers.Add(MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker));
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.DiscardPendingChanges();
    Assert.False(testEntry.HasPendingChanges);
    Assert.AreEqual(initialName, testEntry.Name);
    Assert.AreEqual(initialUser.Id, testEntry.UserId);
    Assert.AreEqual(initialPicture, testEntry.Picture);
    Assert.AreEqual(initialDeviceTrackers, testEntry.DeviceTrackers);
  }

  private Person CreateTestEntry(out string entityId, out string name, out User user, out string picture,
    out IEnumerable<string> deviceTrackers)
  {
    entityId = MockHelpers.GetRandomEntityId(KnownDomains.Person);
    name = MockHelpers.GetRandomTestName();
    user = User.CreateUnmodified(MockHelpers.GetRandomTestName(), name, false);
    picture = "/test/Picture.png";
    deviceTrackers = new[]
    {
      MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker),
      MockHelpers.GetRandomEntityId(KnownDomains.DeviceTracker)
    };
    return Person.CreateUnmodified(entityId, name, user.Id, picture, deviceTrackers);
  }
}