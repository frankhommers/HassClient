namespace HassClient.WebSocket.Messages.Authentication
{
  /// <summary>
  ///   Represents an authentication message used by Web Socket API.
  /// </summary>
  internal class AuthenticationOkMessage : BaseMessage
  {
    public AuthenticationOkMessage()
      : base("auth_ok")
    {
    }

    public string HAVersion { get; set; }
  }
}