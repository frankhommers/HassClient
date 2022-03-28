namespace HassClient.WebSocket.Messages.Authentication
{
  /// <summary>
  ///   Represents an authentication message used by Web Socket API.
  /// </summary>
  internal class AuthenticationMessage : BaseMessage
  {
    public AuthenticationMessage()
      : base("auth")
    {
    }

    public string AccessToken { get; set; }
  }
}