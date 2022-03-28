namespace HassClient.Core.Models.Color
{
  /// <summary>
  ///   Represents an RGBWW (red, green, blue, cold white, warm white) color.
  /// </summary>
  public class RgbwwColor : RgbColor
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="RgbwwColor" /> class.
    /// </summary>
    /// <param name="red">The red color component value.</param>
    /// <param name="green">The green color component value.</param>
    /// <param name="blue">The blue color component value.</param>
    /// <param name="coldWhite">The cold white color component value.</param>
    /// <param name="warmWhite">The warm white color component value.</param>
    public RgbwwColor(byte red, byte green, byte blue, byte coldWhite, byte warmWhite)
      : base(red, green, blue)
    {
      CW = coldWhite;
      WW = warmWhite;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="RgbwwColor" /> class.
    /// </summary>
    /// <param name="color">A <see cref="System.Drawing.Color" /> color.</param>
    public RgbwwColor(System.Drawing.Color color)
      : this(color.R, color.G, color.B, color.A, color.A)
    {
    }

    /// <summary>
    ///   Gets the cold white color component value.
    /// </summary>
    public byte CW { get; internal set; }

    /// <summary>
    ///   Gets the warm white color component value.
    /// </summary>
    public byte WW { get; internal set; }

    public static implicit operator RgbwwColor(System.Drawing.Color x)
    {
      return new RgbwwColor(x);
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"[{R}, {G}, {B}, {CW}, {WW}]";
    }
  }
}