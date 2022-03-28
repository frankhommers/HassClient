using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages
{
  /// <summary>
  ///   Represents an identifiable message used by Web Socket API.
  /// </summary>
  public abstract class BaseIdentifiableMessage : BaseMessage
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="BaseIdentifiableMessage" /> class.
    /// </summary>
    /// <param name="type">
    ///   <inheritdoc />
    /// </param>
    public BaseIdentifiableMessage(string type)
      : base(type)
    {
    }

    /// <summary>
    ///   Gets the message identifier.
    /// </summary>
    [JsonProperty("id", Required = Required.Always)]
    public uint Id { get; internal set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"{base.ToString()} Id:{Id}";
    }
  }
}