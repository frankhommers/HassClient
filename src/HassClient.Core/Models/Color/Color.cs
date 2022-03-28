namespace HassClient.Core.Models.Color
{
  /// <summary>
  ///   Represents a Home Assistant color.
  /// </summary>
  public abstract class Color
  {
    /// <summary>
    ///   Creates a <see cref="RgbColor" /> with the given values.
    /// </summary>
    /// <param name="red">The red color component value.</param>
    /// <param name="green">The green color component value.</param>
    /// <param name="blue">The blue color component value.</param>
    /// <returns>A <see cref="RgbColor" /> with the given values.</returns>
    public static RgbColor FromRgb(byte red, byte green, byte blue)
    {
      return new RgbColor(red, green, blue);
    }

    /// <summary>
    ///   Creates a <see cref="RgbwColor" /> with the given values.
    /// </summary>
    /// <param name="red">The red color component value.</param>
    /// <param name="green">The green color component value.</param>
    /// <param name="blue">The blue color component value.</param>
    /// <param name="white">The white color component value.</param>
    /// <returns>A <see cref="RgbwColor" /> with the given values.</returns>
    public static RgbwColor FromRgbw(byte red, byte green, byte blue, byte white)
    {
      return new RgbwColor(red, green, blue, white);
    }

    /// <summary>
    ///   Creates a <see cref="RgbwwColor" /> with the given values.
    /// </summary>
    /// <param name="red">The red color component value.</param>
    /// <param name="green">The green color component value.</param>
    /// <param name="blue">The blue color component value.</param>
    /// <param name="coldWhite">The cold white color component value.</param>
    /// <param name="warmWhite">The warm white color component value.</param>
    /// <returns>A <see cref="RgbwwColor" /> with the given values.</returns>
    public static RgbwwColor FromRgbww(byte red, byte green, byte blue, byte coldWhite, byte warmWhite)
    {
      return new RgbwwColor(red, green, blue, coldWhite, warmWhite);
    }

    /// <summary>
    ///   Creates a <see cref="HsColor" /> with the given values.
    /// </summary>
    /// <param name="hue">The hue value in the range [0, 360].</param>
    /// <param name="saturation">The saturation value in the range [0, 100].</param>
    /// <returns>A <see cref="HsColor" /> with the given values.</returns>
    public static HsColor FromHs(uint hue, uint saturation)
    {
      return new HsColor(hue, saturation);
    }

    /// <summary>
    ///   Creates a <see cref="XyColor" /> with the given values.
    /// </summary>
    /// <param name="x">The horizontal coordinate in the range [0, 1].</param>
    /// <param name="y">The vertical coordinate in the range [0, 1].</param>
    /// <returns>A <see cref="XyColor" /> with the given values.</returns>
    public static XyColor FromXy(float x, float y)
    {
      return new XyColor(x, y);
    }

    /// <summary>
    ///   Creates a <see cref="KelvinTemperatureColor" /> with the given temperature.
    /// </summary>
    /// <param name="kelvins">
    ///   A value representing the color temperature in kelvins in the range [1000, 40000].
    /// </param>
    /// <returns>A <see cref="KelvinTemperatureColor" /> with the given temperature.</returns>
    public static KelvinTemperatureColor FromKelvinTemperature(uint kelvins)
    {
      return new KelvinTemperatureColor(kelvins);
    }

    /// <summary>
    ///   Creates a <see cref="MiredsTemperatureColor" /> with the given temperature.
    /// </summary>
    /// <param name="mireds">
    ///   A value representing the color temperature in mireds in the range [153, 500].
    /// </param>
    /// <returns>A <see cref="MiredsTemperatureColor" /> with the given temperature.</returns>
    public static MiredsTemperatureColor FromMireds(uint mireds)
    {
      return new MiredsTemperatureColor(mireds);
    }
  }
}