namespace HassClient.WebSocket.Messages.Authentication
{
  /// <summary>
  ///   Represents an authentication message used by Web Socket API.
  /// </summary>
  internal class AuthenticationInvalidMessage : BaseMessage
  {
    public AuthenticationInvalidMessage()
      : base("auth_invalid")
    {
    }

    public string Message { get; set; }
  }
}