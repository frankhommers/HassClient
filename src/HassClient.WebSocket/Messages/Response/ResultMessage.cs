using System.Diagnostics;
using HassClient.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Messages.Response
{
  internal class ResultMessage : BaseIncomingMessage
  {
    public ResultMessage()
      : base("result")
    {
    }

    [JsonProperty("success", Required = Required.Always)]
    public bool Success { get; set; }

    [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
    public JRaw Result { get; set; }

    [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
    public ErrorInfo Error { get; set; }

    public T DeserializeResult<T>()
    {
      Trace.WriteLine($"Deserializing result: {Result}");
      return HassSerializer.DeserializeObject<T>(Result);
    }

    public void PopulateResult(object target)
    {
      HassSerializer.PopulateObject(Result, target);
    }
  }
}