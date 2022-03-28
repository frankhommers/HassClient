using System;
using System.Globalization;

namespace HassClient.Core.Models.Color
{
  /// <summary>
  ///   Represents a CIE 1931 XY coordinate pair.
  /// </summary>
  public class XyColor : Color
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="XyColor" /> class.
    /// </summary>
    /// <param name="x">The horizontal coordinate in the range [0, 1].</param>
    /// <param name="y">The vertical coordinate in the range [0, 1].</param>
    public XyColor(float x, float y)
    {
      if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x), x, "X value must be in the range [0.0, 1.0]");

      if (y < 0 || y > 1)
        throw new ArgumentOutOfRangeException(nameof(y), y, "Y value must be in the range [0.0, 1.0]");

      X = x;
      Y = y;
    }

    /// <summary>
    ///   Gets the horizontal coordinate.
    /// </summary>
    public float X { get; internal set; }

    /// <summary>
    ///   Gets the vertical coordinate.
    /// </summary>
    public float Y { get; internal set; }

    /// <inheritdoc />
    public override string ToString()
    {
      FormattableString line = $"[{X}, {Y}]";
      return line.ToString(CultureInfo.InvariantCulture);
    }
  }
}