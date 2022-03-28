using System;
using HassClient.Core.Helpers;
using HassClient.Core.Models;
using HassClient.Core.Models.Events;
using HassClient.Core.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Messages.Response
{
  /// <summary>
  ///   Information of a fired Home Assistant event.
  /// </summary>
  public class EventResultInfo
  {
    /// <summary>
    ///   Gets or sets the event type.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string EventType { get; set; }

    /// <summary>
    ///   Gets the event type as <see cref="KnownEventTypes" />.
    /// </summary>
    [JsonIgnore]
    public KnownEventTypes KnownEventType => EventType.AsKnownEventType();

    /// <summary>
    ///   Gets or sets the time at which the event was fired.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public DateTimeOffset TimeFired { get; set; }

    /// <summary>
    ///   Gets or sets the origin that fired the event.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string Origin { get; set; }

    /// <summary>
    ///   Gets or sets the data associated with the fired event.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public JRaw Data { get; set; }

    /// <summary>
    ///   Gets or sets the context associated with the fired event.
    /// </summary>
    public Context Context { get; set; }

    /// <summary>
    ///   Deserializes the event <see cref="Data" /> to the specified .NET type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <returns>The deserialized data object.</returns>
    public T DeserializeData<T>()
    {
      return HassSerializer.DeserializeObject<T>(Data);
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"Event {EventType} fired at {TimeFired} from {Origin}";
    }
  }
}