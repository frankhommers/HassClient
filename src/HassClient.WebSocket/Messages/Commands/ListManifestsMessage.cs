namespace HassClient.WebSocket.Messages.Commands
{
  internal class ListManifestsMessage : BaseOutgoingMessage
  {
    public ListManifestsMessage()
      : base("manifest/list")
    {
    }
  }
}