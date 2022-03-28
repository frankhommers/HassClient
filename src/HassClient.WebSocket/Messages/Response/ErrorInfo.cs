namespace HassClient.WebSocket.Messages.Response
{
  /// <summary>
  ///   Provides information about the error occurred.
  /// </summary>
  public class ErrorInfo
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="ErrorInfo" /> class.
    /// </summary>
    public ErrorInfo()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="ErrorInfo" /> class.
    /// </summary>
    internal ErrorInfo(ErrorCodes code)
    {
      Code = code;
      Message = code.ToString();
    }

    /// <summary>
    ///   The error code.
    /// </summary>
    public ErrorCodes Code { get; set; }

    /// <summary>
    ///   A message provided by the server with detailed information about the error.
    /// </summary>
    public string Message { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"{Code}: {Message}";
    }
  }
}