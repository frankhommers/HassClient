using System;

namespace HassClient.Core.Models.Color
{
  /// <summary>
  ///   Represents a temperature color expressed in mireds.
  /// </summary>
  public class MiredsTemperatureColor : Color
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="MiredsTemperatureColor" /> class.
    /// </summary>
    /// <param name="mireds">
    ///   A value representing the color temperature in mireds in the range [153, 500].
    /// </param>
    public MiredsTemperatureColor(uint mireds)
    {
      Mireds = Math.Min(Math.Max(mireds, 153), 500);
    }

    /// <summary>
    ///   Gets a value representing the color temperature in mireds.
    /// </summary>
    public uint Mireds { get; internal set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return Mireds.ToString();
    }
  }
}