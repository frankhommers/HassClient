using System;

namespace HassClient.Core.Models.Color
{
  /// <summary>
  ///   Represents a HSV color by hue and saturation.
  /// </summary>
  public class HsColor : Color
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="HsColor" /> class.
    /// </summary>
    /// <param name="hue">The hue value in the range [0, 360].</param>
    /// <param name="saturation">The saturation value in the range [0, 100].</param>
    public HsColor(uint hue, uint saturation)
    {
      if (hue > 360) throw new ArgumentOutOfRangeException(nameof(hue), hue, "Hue value must be in the range [0, 360]");

      if (saturation > 100)
        throw new ArgumentOutOfRangeException(nameof(saturation), saturation,
          "Saturation value must be in the range [0, 100]");

      Hue = hue;
      Saturation = saturation;
    }

    /// <summary>
    ///   Gets the hue value.
    /// </summary>
    public uint Hue { get; internal set; }

    /// <summary>
    ///   Gets the saturation value.
    /// </summary>
    public uint Saturation { get; internal set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"[{Hue}, {Saturation}]";
    }
  }
}