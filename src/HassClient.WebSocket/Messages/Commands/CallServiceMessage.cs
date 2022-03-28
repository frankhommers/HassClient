using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages.Commands
{
  internal class CallServiceMessage : BaseOutgoingMessage
  {
    public CallServiceMessage()
      : base("call_service")
    {
    }

    public CallServiceMessage(string domain, string service, object serviceData)
      : this()
    {
      Domain = domain;
      Service = service;
      ServiceData = serviceData;
    }

    [JsonProperty(Required = Required.Always)]
    public string Domain { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string Service { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public object ServiceData { get; set; }
  }
}