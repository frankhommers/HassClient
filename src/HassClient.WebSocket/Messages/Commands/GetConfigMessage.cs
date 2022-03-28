namespace HassClient.WebSocket.Messages.Commands
{
  internal class GetConfigMessage : BaseOutgoingMessage
  {
    public GetConfigMessage()
      : base("get_config")
    {
    }
  }
}