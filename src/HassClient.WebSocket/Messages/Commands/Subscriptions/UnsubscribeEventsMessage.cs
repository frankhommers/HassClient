using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages.Commands.Subscriptions
{
  internal class UnsubscribeEventsMessage : BaseOutgoingMessage
  {
    public UnsubscribeEventsMessage()
      : base("unsubscribe_events")
    {
    }

    [JsonProperty(Required = Required.Always)]
    public uint SubscriptionId { get; set; }
  }
}