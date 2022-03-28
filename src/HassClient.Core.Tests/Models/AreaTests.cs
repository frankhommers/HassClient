using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(Area))]
public class AreaTests
{
  [Test]
  public void HasPublicConstructorWithParameters()
  {
    ConstructorInfo constructor = typeof(Area).GetConstructors()
      .FirstOrDefault(x => x.IsPublic && x.GetParameters().Length > 0);
    Assert.NotNull(constructor);
  }

  [Test]
  public void NewAreaHasPendingChanges()
  {
    Area testEntry = new(MockHelpers.GetRandomTestName());
    Assert.IsTrue(testEntry.HasPendingChanges);
  }

  [Test]
  public void NewAreaIsUntracked()
  {
    Area testEntry = new(MockHelpers.GetRandomTestName());
    Assert.False(testEntry.IsTracked);
  }

  private static IEnumerable<string> NullOrWhiteSpaceStringValues()
  {
    return RegistryEntryBaseTests.NullOrWhiteSpaceStringValues();
  }

  [Test]
  [TestCaseSource(nameof(NullOrWhiteSpaceStringValues))]
  public void NewAreaWithNullOrWhiteSpaceNameThrows(string value)
  {
    Assert.Throws<ArgumentException>(() => new Area(value));
  }

  [Test]
  public void SetNewNameMakesHasPendingChangesTrue()
  {
    Area testEntry = CreateTestEntry(out string initialName, out _);

    testEntry.Name = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Name = initialName;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewPictureMakesHasPendingChangesTrue()
  {
    Area testEntry = CreateTestEntry(out _, out string picture);

    testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Picture = picture;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void DiscardPendingChanges()
  {
    Area testEntry = CreateTestEntry(out string initialName, out string initialPicture);

    testEntry.Name = MockHelpers.GetRandomTestName();
    testEntry.Picture = $"/test/{MockHelpers.GetRandomTestName()}.png";
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.DiscardPendingChanges();
    Assert.False(testEntry.HasPendingChanges);
    Assert.AreEqual(initialName, testEntry.Name);
    Assert.AreEqual(initialPicture, testEntry.Picture);
  }

  private Area CreateTestEntry(out string name, out string picture)
  {
    name = MockHelpers.GetRandomTestName();
    picture = "/test/Picture.png";
    return Area.CreateUnmodified(name, picture);
  }
}