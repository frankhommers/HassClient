using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HassClient.Core.Models.RegistryEntries;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests.RegistryEntryApiTests;

public class DeviceRegistryApiTests : BaseHassWsApiTest
{
  [Test]
  public async Task GetDevices()
  {
    IEnumerable<Device> devices = await HassWsApi.GetDevicesAsync();

    Assert.NotNull(devices);
    Assert.IsNotEmpty(devices);
  }

  [Test]
  public async Task UpdateNameDevice()
  {
    IEnumerable<Device> devices = await HassWsApi.GetDevicesAsync();
    Device testDevice = devices.FirstOrDefault();
    Assert.NotNull(testDevice, "SetUp failed");

    string newName = $"TestDevice_{DateTime.Now.Ticks}";
    testDevice.Name = newName;
    bool result = await HassWsApi.UpdateDeviceAsync(testDevice);

    Assert.IsTrue(result);
    Assert.IsFalse(testDevice.HasPendingChanges);
    Assert.AreEqual(newName, testDevice.Name);
  }

  [Test]
  public async Task UpdateAreaIdDevice()
  {
    IEnumerable<Device> devices = await HassWsApi.GetDevicesAsync();
    Device testDevice = devices.FirstOrDefault();
    Assert.NotNull(testDevice, "SetUp failed");

    string newAreaId = $"{DateTime.Now.Ticks}";
    testDevice.AreaId = newAreaId;
    bool result = await HassWsApi.UpdateDeviceAsync(testDevice);

    Assert.IsTrue(result);
    Assert.IsFalse(testDevice.HasPendingChanges);
    Assert.AreEqual(newAreaId, testDevice.AreaId);
  }
}