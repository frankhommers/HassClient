using System.Linq;
using System.Reflection;
using HassClient.Core.Models.KnownEnums;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.Core.Tests.Models;

[TestFixture(TestOf = typeof(Device))]
public class DeviceTests
{
  [Test]
  public void HasNoPublicConstructors()
  {
    ConstructorInfo constructor = typeof(Device).GetConstructors()
      .FirstOrDefault(x => x.IsPublic);
    Assert.Null(constructor);
  }

  [Test]
  public void SetNewNameMakesHasPendingChangesTrue()
  {
    Device testEntry = CreateTestEntry(out _, out string initialName, out _, out _);

    testEntry.Name = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.Name = initialName;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void SetNewAreaIdMakesHasPendingChangesTrue()
  {
    Device testEntry = CreateTestEntry(out _, out _, out string initialAreaId, out _);

    testEntry.AreaId = MockHelpers.GetRandomTestName();
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.AreaId = initialAreaId;
    Assert.False(testEntry.HasPendingChanges);
  }

  [Test]
  public void DiscardPendingChanges()
  {
    Device testEntry = CreateTestEntry(out _, out string initialName, out string initialAreaId,
      out DisabledByEnum initialDisabledBy);

    testEntry.Name = MockHelpers.GetRandomTestName();
    testEntry.AreaId = MockHelpers.GetRandomTestName();
    //testEntry.DisabledBy = DisabledByEnum.User;
    Assert.IsTrue(testEntry.HasPendingChanges);

    testEntry.DiscardPendingChanges();
    Assert.False(testEntry.HasPendingChanges);
    Assert.AreEqual(initialName, testEntry.Name);
    Assert.AreEqual(initialAreaId, testEntry.AreaId);
    Assert.AreEqual(initialDisabledBy, testEntry.DisabledBy);
  }

  [Test]
  public void NameIsNameByUserIfDefined()
  {
    Device testEntry = CreateTestEntry(out _, out _, out _, out _);

    Assert.AreEqual(testEntry.OriginalName, testEntry.Name);

    string testName = MockHelpers.GetRandomTestName();
    testEntry.Name = testName;
    Assert.AreEqual(testName, testEntry.Name);

    testEntry.Name = null;
    Assert.AreEqual(testEntry.OriginalName, testEntry.Name);
  }

  private Device CreateTestEntry(out string entityId, out string name, out string areaId, out DisabledByEnum disabledBy)
  {
    entityId = MockHelpers.GetRandomEntityId(KnownDomains.Esphome);
    name = MockHelpers.GetRandomTestName();
    areaId = MockHelpers.GetRandomTestName();
    disabledBy = DisabledByEnum.Integration;
    return Device.CreateUnmodified(entityId, name, areaId, disabledBy);
  }
}