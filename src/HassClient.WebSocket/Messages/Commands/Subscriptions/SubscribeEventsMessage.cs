using HassClient.Core.Models.Events;
using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages.Commands.Subscriptions
{
  internal class SubscribeEventsMessage : BaseOutgoingMessage
  {
    public SubscribeEventsMessage()
      : base("subscribe_events")
    {
    }

    public SubscribeEventsMessage(string eventType)
      : this()
    {
      EventType = eventType;
    }

    [JsonProperty] public string EventType { get; set; }

    private bool ShouldSerializeEventType()
    {
      return EventType != Event.AnyEventFilter && EventType != null;
    }
  }
}