using System;
using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages.Commands
{
  [Obsolete(
    "This web-socket message was depreciated in Home Assistant Core 0.107 and will be removed in a future release. Until then it will result in a WARNING entry in the user's log.")]
  internal class GetMediaPlayerThumbnailMessage : BaseOutgoingMessage
  {
    public GetMediaPlayerThumbnailMessage()
      : base("media_player_thumbnail")
    {
    }

    [JsonProperty(Required = Required.Always)]
    public string EntityId { get; set; }
  }
}