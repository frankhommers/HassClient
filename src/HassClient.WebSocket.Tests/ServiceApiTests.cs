using System.Collections.Generic;
using System.Threading.Tasks;
using HassClient.Core.Models;
using HassClient.Core.Models.KnownEnums;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class ServiceApiTests : BaseHassWsApiTest
{
  [Test]
  public async Task GetServices()
  {
    IEnumerable<ServiceDomain> services = await HassWsApi.GetServicesAsync();

    Assert.NotNull(services);
    Assert.IsNotEmpty(services);
  }

  [Test]
  public async Task CallService()
  {
    Context result = await HassWsApi.CallServiceAsync("homeassistant", "check_config");

    Assert.NotNull(result);
  }

  [Test]
  public async Task CallServiceForEntities()
  {
    bool result = await HassWsApi.CallServiceForEntitiesAsync("homeassistant", "update_entity", "sun.sun");

    Assert.NotNull(result);
  }

  [Test]
  public async Task CallServiceWithKnwonDomain()
  {
    bool result = await HassWsApi.CallServiceAsync(KnownDomains.Homeassistant, KnownServices.CheckConfig);

    Assert.IsTrue(result);
  }

  [Test]
  public async Task CallServiceForEntitiesWithKnwonDomain()
  {
    bool result =
      await HassWsApi.CallServiceForEntitiesAsync(KnownDomains.Homeassistant, KnownServices.UpdateEntity, "sun.sun");

    Assert.IsTrue(result);
  }
}