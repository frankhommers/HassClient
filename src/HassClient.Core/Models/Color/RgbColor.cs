namespace HassClient.Core.Models.Color
{
  /// <summary>
  ///   Represents an RGB (red, green, blue) color.
  /// </summary>
  public class RgbColor : Color
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="RgbColor" /> class.
    /// </summary>
    /// <param name="red">The red color component value.</param>
    /// <param name="green">The green color component value.</param>
    /// <param name="blue">The blue color component value.</param>
    public RgbColor(byte red, byte green, byte blue)
    {
      R = red;
      G = green;
      B = blue;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="RgbColor" /> class.
    /// </summary>
    /// <param name="color">A <see cref="System.Drawing.Color" /> color.</param>
    public RgbColor(System.Drawing.Color color)
      : this(color.R, color.G, color.B)
    {
    }

    /// <summary>
    ///   Gets the red color component value.
    /// </summary>
    public byte R { get; internal set; }

    /// <summary>
    ///   Gets the green color component value.
    /// </summary>
    public byte G { get; internal set; }

    /// <summary>
    ///   Gets the blue color component value.
    /// </summary>
    public byte B { get; internal set; }

    public static implicit operator RgbColor(System.Drawing.Color x)
    {
      return new RgbColor(x);
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"[{R}, {G}, {B}]";
    }
  }
}