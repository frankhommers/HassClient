namespace HassClient.WebSocket.Messages.Authentication
{
  /// <summary>
  ///   Represents an authentication message used by Web Socket API.
  /// </summary>
  internal class AuthenticationRequiredMessage : BaseMessage
  {
    public AuthenticationRequiredMessage()
      : base("auth_required")
    {
    }

    public string HAVersion { get; set; }
  }
}