using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using HassClient.WebSocket.Tests.Extensions;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

[SetUpFixture]
public class EnvironmentSetup
{
  private TestcontainersContainer hassContainer;

  [OneTimeSetUp]
  public async Task GlobalSetupAsync()
  {
    string instanceBaseUrl = Environment.GetEnvironmentVariable(BaseHassWsApiTest.TestsInstanceBaseUrlVar);

    if (instanceBaseUrl == null)
    {
      // Create temporary directory with tests resources
      string tmpDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
      Directory.CreateDirectory(tmpDirectory);
      DirectoryExtensions.CopyFilesRecursively("./resources", tmpDirectory);

      const int HassPort = 8123;
      const string HassVersion = "latest";
      const string tokenFilename = "TOKEN";
      ITestcontainersBuilder<TestcontainersContainer> testcontainersBuilder =
        new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage($"homeassistant/home-assistant:{HassVersion}")
          .WithPortBinding(HassPort, true)
          .WithExposedPort(HassPort)
          .WithBindMount(Path.Combine(tmpDirectory, "config"), "/config")
          .WithBindMount(Path.Combine(tmpDirectory, "scripts"), "/app")
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(HassPort))
          .WithEntrypoint("/bin/bash", "-c")
          .WithCommand($"python3 /app/create_token.py >/app/{tokenFilename} && /init");

      hassContainer = testcontainersBuilder.Build();
      await hassContainer.StartAsync();

      ushort mappedPort = hassContainer.GetMappedPublicPort(HassPort);
      string hostTokenPath = Path.Combine(tmpDirectory, "scripts", tokenFilename);
      string accessToken = File.ReadLines(hostTokenPath).First();

      Environment.SetEnvironmentVariable(BaseHassWsApiTest.TestsInstanceBaseUrlVar, $"http://localhost:{mappedPort}");
      Environment.SetEnvironmentVariable(BaseHassWsApiTest.TestsAccessTokenVar, accessToken);
    }
  }

  [OneTimeTearDown]
  public async Task GlobalTeardown()
  {
    if (hassContainer != null) await hassContainer.DisposeAsync();
  }
}