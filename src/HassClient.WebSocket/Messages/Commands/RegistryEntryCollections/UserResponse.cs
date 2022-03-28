using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HassClient.WebSocket.Messages.Commands.RegistryEntryCollections
{
  internal class UserResponse
  {
    [JsonProperty("user")] public JRaw UserRaw { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"{UserRaw}";
    }
  }
}