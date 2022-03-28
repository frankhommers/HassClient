namespace HassClient.Core.Models.Color
{
  /// <summary>
  ///   Represents an RGBW (red, green, blue, white) color.
  /// </summary>
  public class RgbwColor : RgbColor
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="RgbwColor" /> class.
    /// </summary>
    /// <param name="red">The red color component value.</param>
    /// <param name="green">The green color component value.</param>
    /// <param name="blue">The blue color component value.</param>
    /// <param name="white">The white color component value.</param>
    public RgbwColor(byte red, byte green, byte blue, byte white)
      : base(red, green, blue)
    {
      W = white;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="RgbwColor" /> class.
    /// </summary>
    /// <param name="color">A <see cref="System.Drawing.Color" /> color.</param>
    public RgbwColor(System.Drawing.Color color)
      : this(color.R, color.G, color.B, color.A)
    {
    }

    /// <summary>
    ///   Gets the white color component value.
    /// </summary>
    public byte W { get; internal set; }

    public static implicit operator RgbwColor(System.Drawing.Color x)
    {
      return new RgbwColor(x);
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"[{R}, {G}, {B}, {W}]";
    }
  }
}