using System;
using System.Linq;
using System.Threading.Tasks;
using HassClient.WebSocket.Messages.Commands.Search;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class SearchApiTests : BaseHassWsApiTest
{
  public static ItemType[] GetItemTypes()
  {
    return Enum.GetValues<ItemType>().Except(new[] {ItemType.Entity}).ToArray();
  }

  [TestCaseSource(nameof(GetItemTypes))]
  public async Task SearchRelatedUnknown(ItemType itemType)
  {
    SearchRelatedResponse result = await HassWsApi.SearchRelatedAsync(itemType, $"Unknown_{DateTime.Now.Ticks}");

    Assert.NotNull(result);
    Assert.IsNull(result.AreaIds);
    Assert.IsNull(result.AutomationIds);
    Assert.IsNull(result.ConfigEntryIds);
    Assert.IsNull(result.DeviceIds);
    Assert.IsNull(result.EntityIds);
  }

  [Test]
  public async Task SearchRelatedKnownEntity()
  {
    SearchRelatedResponse result = await HassWsApi.SearchRelatedAsync(ItemType.Entity, "light.bed_light");

    Assert.NotNull(result);
    Assert.NotNull(result.ConfigEntryIds);
    Assert.NotNull(result.DeviceIds);
    Assert.NotZero(result.ConfigEntryIds.Length);
    Assert.NotZero(result.DeviceIds.Length);
  }
}