using System.Collections.Generic;
using System.Threading.Tasks;
using HassClient.Core.Models;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class EntitySourcesApiTests : BaseHassWsApiTest
{
  [Test]
  public async Task GetEntitySources()
  {
    IEnumerable<EntitySource> entities = await HassWsApi.GetEntitySourcesAsync();

    Assert.IsNotNull(entities);
    Assert.IsNotEmpty(entities);
  }

  [Test]
  public async Task GetEntitySourceWithFilterAsync()
  {
    string entityId = "zone.home";
    EntitySource result = await HassWsApi.GetEntitySourceAsync(entityId);

    Assert.AreEqual(result.EntityId, entityId);
    Assert.AreEqual(result.Domain, entityId.Split('.')[0]);
    Assert.NotNull(result.Source);
  }
}