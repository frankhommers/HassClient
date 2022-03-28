using System.Threading.Tasks;
using HassClient.WebSocket.Messages.Response;
using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages.Commands
{
  internal class RenderTemplateMessage : BaseOutgoingMessage
  {
    private readonly TaskCompletionSource<string> templateEventReceivedTCS;

    public RenderTemplateMessage()
      : base("render_template")
    {
      templateEventReceivedTCS = new TaskCompletionSource<string>();
    }

    [JsonIgnore] public Task<string> WaitResponseTask => templateEventReceivedTCS.Task;

    [JsonProperty(Required = Required.Always)]
    public string Template { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string[] EntitiesIds { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string[] Variables { get; set; }

    public void ProcessEventReceivedMessage(EventResultMessage eventResultMessage)
    {
      TemplateEventInfo templateEventInfo = eventResultMessage.DeserializeEvent<TemplateEventInfo>();
      templateEventReceivedTCS.SetResult(templateEventInfo.Result);
    }
  }
}