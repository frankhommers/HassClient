using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages.Commands
{
  internal class EntitySourceMessage : BaseOutgoingMessage
  {
    public EntitySourceMessage()
      : base("entity/source")
    {
    }

    [JsonProperty("entity_id", NullValueHandling = NullValueHandling.Ignore)]
    public string[] EntityIds { get; set; }
  }
}