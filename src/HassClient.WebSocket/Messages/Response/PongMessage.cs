namespace HassClient.WebSocket.Messages.Response
{
  internal class PongMessage : BaseIncomingMessage
  {
    public PongMessage()
      : base("pong")
    {
    }
  }
}