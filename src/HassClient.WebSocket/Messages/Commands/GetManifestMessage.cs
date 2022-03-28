namespace HassClient.WebSocket.Messages.Commands
{
  internal class GetManifestMessage : BaseOutgoingMessage
  {
    public GetManifestMessage()
      : base("manifest/get")
    {
    }

    /// <summary>
    ///   Gets or sets the name of the integration to query.
    /// </summary>
    public string Integration { get; set; }
  }
}