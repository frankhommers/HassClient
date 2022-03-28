using System.IO;
using System.Reflection;
using HassClient.WebSocket.Messages;
using HassClient.WebSocket.Messages.Response;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Tests.Mocks.HassServer.CommandProcessors;

public abstract class BaseCommandProcessor
{
  public abstract bool CanProcess(BaseIdentifiableMessage receivedCommand);

  public abstract BaseIdentifiableMessage ProcessCommand(MockHassServerRequestContext context,
    BaseIdentifiableMessage receivedCommand);

  protected BaseIdentifiableMessage CreateResultMessageWithError(ErrorInfo errorInfo)
  {
    return new ResultMessage { Error = errorInfo };
  }

  protected BaseIdentifiableMessage CreateResultMessageWithResult(JRaw result)
  {
    return new ResultMessage { Success = true, Result = result };
  }

  protected Stream GetResourceStream(string filename)
  {
    Assembly assembly = typeof(BaseCommandProcessor).Assembly;
    string assemblyNamepace = Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name);
    return assembly.GetManifestResourceStream($"{assemblyNamepace}.Mocks.Data.{filename}");
  }
}