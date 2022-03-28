namespace HassClient.WebSocket.Messages.Commands
{
  internal class GetServicesMessage : BaseOutgoingMessage
  {
    public GetServicesMessage()
      : base("get_services")
    {
    }
  }
}