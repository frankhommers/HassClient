using System.Threading.Tasks;
using HassClient.WebSocket.Messages.Commands;
using HassClient.WebSocket.Messages.Response;
using NUnit.Framework;

namespace HassClient.WebSocket.Tests;

public class RawCommandApiTests : BaseHassWsApiTest
{
  [Test]
  public async Task SendRawCommandWithResult()
  {
    RawCommandResult result = await HassWsApi.SendRawCommandWithResultAsync(new RawCommandMessage("get_config"));

    Assert.IsNotNull(result);
    Assert.IsTrue(result.Success);
    Assert.IsNotNull(result.Result);
  }

  [Test]
  public async Task SendRawCommandWithSuccess()
  {
    bool result = await HassWsApi.SendRawCommandWithSuccessAsync(new RawCommandMessage("get_config"));

    Assert.IsTrue(result);
  }

  [Test]
  public async Task SendInvalidRawCommandWithResultReturnsUnknownCommandError()
  {
    RawCommandResult result = await HassWsApi.SendRawCommandWithResultAsync(new RawCommandMessage("invalid_command"));

    Assert.IsNotNull(result);
    Assert.IsFalse(result.Success);
    Assert.IsNull(result.Result);
    Assert.IsNotNull(result.Error);
    Assert.AreEqual(ErrorCodes.UnknownCommand, result.Error.Code);
  }

  [Test]
  public async Task PingPongCommandResponseIsEncapsulatedAsResultMessage()
  {
    RawCommandResult result = await HassWsApi.SendRawCommandWithResultAsync(new RawCommandMessage("ping"));

    Assert.IsNotNull(result);
    Assert.IsTrue(result.Success);
  }
}