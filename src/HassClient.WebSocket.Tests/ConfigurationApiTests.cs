using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class ConfigurationApiTests : BaseHassWsApiTest
{
  private ConfigurationModel configuration;

  [OneTimeSetUp]
  [Test]
  public async Task GetConfiguration()
  {
    if (configuration != null) return;

    configuration = await HassWsApi.GetConfigurationAsync();

    Assert.IsNotNull(configuration);
  }

  [Test]
  public void ConfigurationHasAllowedExternalDirs()
  {
    Assert.NotNull(configuration.AllowedExternalDirs);
    Assert.IsNotEmpty(configuration.AllowedExternalDirs);
  }

  [Test]
  public void ConfigurationHasAllowedExternalUrls()
  {
    Assert.NotNull(configuration.AllowedExternalUrls);
  }

  [Test]
  public void ConfigurationHasComponents()
  {
    Assert.NotNull(configuration.Components);
    Assert.IsNotEmpty(configuration.Components);
  }

  [Test]
  public void ConfigurationHasConfigDirectory()
  {
    Assert.NotNull(configuration.ConfigDirectory);
  }

  [Test]
  public void ConfigurationHasConfigSource()
  {
    Assert.NotNull(configuration.ConfigSource);
  }

  [Test]
  public void ConfigurationHasLocation()
  {
    Assert.NotNull(configuration.LocationName);
    Assert.NotZero(configuration.Latitude);
    Assert.NotZero(configuration.Longitude);
  }

  [Test]
  public void ConfigurationHasState()
  {
    Assert.NotNull(configuration.State);
  }

  [Test]
  public void ConfigurationHasTimeZone()
  {
    Assert.NotNull(configuration.TimeZone);
  }

  [Test]
  public void ConfigurationHasUnitSystem()
  {
    Assert.NotNull(configuration.UnitSystem);
    Assert.NotNull(configuration.UnitSystem.Length);
    Assert.NotNull(configuration.UnitSystem.Mass);
    Assert.NotNull(configuration.UnitSystem.Pressure);
    Assert.NotNull(configuration.UnitSystem.Temperature);
    Assert.NotNull(configuration.UnitSystem.Volume);
  }

  [Test]
  public void ConfigurationHasVersion()
  {
    Assert.NotNull(configuration.Version);
  }
}