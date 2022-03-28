using HassClient.Core.Serialization;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Messages.Response
{
  internal class EventResultMessage : BaseIncomingMessage
  {
    public EventResultMessage()
      : base("event")
    {
    }

    public JRaw Event { get; set; }

    public T DeserializeEvent<T>()
    {
      return HassSerializer.DeserializeObject<T>(Event);
    }
  }
}