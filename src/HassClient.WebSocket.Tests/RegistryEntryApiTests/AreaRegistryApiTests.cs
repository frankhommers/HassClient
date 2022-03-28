using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using HassClient.Core.Tests;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests.RegistryEntryApiTests;

public class AreaRegistryApiTests : BaseHassWsApiTest
{
  private Area _testArea;

  [OneTimeSetUp]
  [Test]
  [Order(1)]
  public async Task CreateArea()
  {
    if (_testArea == null)
    {
      _testArea = new Area(MockHelpers.GetRandomTestName());
      bool result = await HassWsApi.CreateAreaAsync(_testArea);
      Assert.IsTrue(result, "SetUp failed");
      return;
    }

    Assert.NotNull(_testArea.Id);
    Assert.NotNull(_testArea.Name);
    Assert.IsFalse(_testArea.HasPendingChanges);
    Assert.IsTrue(_testArea.IsTracked);
  }

  [Test]
  [Order(2)]
  public async Task GetAreas()
  {
    IEnumerable<Area> areas = await HassWsApi.GetAreasAsync();

    Assert.NotNull(areas);
    Assert.IsNotEmpty(areas);
    Assert.IsTrue(areas.Contains(_testArea));
  }

  [Test]
  [Order(3)]
  public async Task UpdateArea()
  {
    _testArea.Name = MockHelpers.GetRandomTestName();
    bool result = await HassWsApi.UpdateAreaAsync(_testArea);

    Assert.IsTrue(result);
    Assert.False(_testArea.HasPendingChanges);
  }

  [Test]
  [Order(4)]
  public async Task UpdateWithForce()
  {
    string initialName = _testArea.Name;
    Area clonedArea = _testArea.Clone();
    clonedArea.Name = $"{initialName}_cloned";
    bool result = await HassWsApi.UpdateAreaAsync(clonedArea);
    Assert.IsTrue(result, "SetUp failed");
    Assert.False(_testArea.HasPendingChanges, "SetUp failed");

    result = await HassWsApi.UpdateAreaAsync(_testArea, true);
    Assert.IsTrue(result);
    Assert.AreEqual(initialName, _testArea.Name);
  }

  [OneTimeTearDown]
  [Test]
  [Order(5)]
  public async Task DeleteArea()
  {
    if (_testArea == null) return;

    bool result = await HassWsApi.DeleteAreaAsync(_testArea);
    Area deletedArea = _testArea;
    _testArea = null;

    Assert.IsTrue(result);
    Assert.IsFalse(deletedArea.IsTracked);
  }
}