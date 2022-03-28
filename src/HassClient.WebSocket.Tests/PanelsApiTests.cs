using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class PanelsApiTests : BaseHassWsApiTest
{
  private IEnumerable<PanelInfo> _panels;

  [OneTimeSetUp]
  [Test]
  public async Task GetPanels()
  {
    if (_panels != null) return;

    _panels = await HassWsApi.GetPanelsAsync();

    Assert.IsNotNull(_panels);
    Assert.IsNotEmpty(_panels);
  }

  [Test]
  public async Task GetPanel()
  {
    if (_panels != null) return;

    PanelInfo firstPanel = _panels?.FirstOrDefault();
    Assert.NotNull(firstPanel, "SetUp failed");

    PanelInfo result = await HassWsApi.GetPanelAsync(firstPanel.UrlPath);

    Assert.IsNotNull(result);
    Assert.AreEqual(firstPanel, result);
  }

  [Test]
  public void GetPanelWithNullUrlPathThrows()
  {
    Assert.ThrowsAsync<ArgumentException>(() => HassWsApi.GetPanelAsync(null));
  }

  [Test]
  public void GetPanelsHasComponentName()
  {
    Assert.IsTrue(_panels.All(x => x.ComponentName != default));
  }

  [Test]
  public void GetPanelsHasConfiguration()
  {
    Assert.IsTrue(_panels.All(x => x.Configuration != default));
  }

  [Test]
  public void GetPanelsHasIcon()
  {
    Assert.IsTrue(_panels.Any(x => x.Icon != default));
  }

  [Test]
  public void GetPanelsHasRequireAdmin()
  {
    Assert.IsTrue(_panels.Any(x => x.RequireAdmin));
  }

  [Test]
  public void GetPanelsHasTitle()
  {
    Assert.IsTrue(_panels.Any(x => x.Title != default));
  }

  [Test]
  public void GetPanelsHasUrlPath()
  {
    Assert.IsTrue(_panels.All(x => x.UrlPath != default));
  }
}