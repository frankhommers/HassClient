namespace HassClient.WebSocket.Messages.Commands
{
  internal class GetPanelsMessage : BaseOutgoingMessage
  {
    public GetPanelsMessage()
      : base("get_panels")
    {
    }
  }
}