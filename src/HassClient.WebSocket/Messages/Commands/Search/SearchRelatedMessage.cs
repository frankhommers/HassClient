using Newtonsoft.Json;

namespace HassClient.WebSocket.Messages.Commands.Search
{
  internal class SearchRelatedMessage : BaseOutgoingMessage
  {
    public SearchRelatedMessage()
      : base("search/related")
    {
    }

    public SearchRelatedMessage(ItemType itemType, string itemId)
      : this()
    {
      ItemType = itemType;
      ItemId = itemId;
    }

    [JsonProperty(Required = Required.Always)]
    public ItemType ItemType { get; set; }

    [JsonProperty(Required = Required.Always)]
    public string ItemId { get; set; }
  }
}