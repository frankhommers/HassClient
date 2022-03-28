namespace HassClient.WebSocket.Messages.Commands
{
  internal class PingMessage : BaseOutgoingMessage
  {
    public PingMessage()
      : base("ping")
    {
    }
  }
}