using Chevron9.Shared.Graphics;

namespace Chevron9.Shared.Extensions;

/// <summary>
///     Extension methods for working with colors, including gradients, blending, and utility functions.
/// </summary>
public static class ColorExtensions
{
    #region Color Blending and Interpolation

    /// <summary>
    ///     Linearly interpolates between two colors.
    /// </summary>
    /// <param name="from">The starting color.</param>
    /// <param name="to">The ending color.</param>
    /// <param name="t">The interpolation factor (0.0 to 1.0).</param>
    /// <returns>The interpolated color.</returns>
    public static Color Lerp(this Color from, Color to, float t)
    {
        t = Math.Clamp(t, 0.0f, 1.0f);

        var r = (byte)Math.Round(from.R + (to.R - from.R) * t);
        var g = (byte)Math.Round(from.G + (to.G - from.G) * t);
        var b = (byte)Math.Round(from.B + (to.B - from.B) * t);
        var a = (byte)Math.Round(from.A + (to.A - from.A) * t);

        return new Color(r, g, b, a);
    }

    /// <summary>
    ///     Blends two colors using alpha blending.
    /// </summary>
    /// <param name="background">The background color.</param>
    /// <param name="foreground">The foreground color to blend over the background.</param>
    /// <returns>The blended color.</returns>
    public static Color AlphaBlend(this Color background, Color foreground)
    {
        var alpha = foreground.AlphaFloat;
        var invAlpha = 1.0f - alpha;

        var r = (byte)Math.Round(foreground.R * alpha + background.R * invAlpha);
        var g = (byte)Math.Round(foreground.G * alpha + background.G * invAlpha);
        var b = (byte)Math.Round(foreground.B * alpha + background.B * invAlpha);
        var a = (byte)Math.Round(255 * (alpha + background.AlphaFloat * invAlpha));

        return new Color(r, g, b, a);
    }

    /// <summary>
    ///     Blends multiple colors with equal weight.
    /// </summary>
    /// <param name="colors">The colors to blend.</param>
    /// <returns>The blended color.</returns>
    /// <exception cref="ArgumentException">Thrown when colors array is null or empty.</exception>
    public static Color BlendColors(this IEnumerable<Color> colors)
    {
        var colorArray = colors?.ToArray() ?? throw new ArgumentException("Colors cannot be null.", nameof(colors));

        if (colorArray.Length == 0)
        {
            throw new ArgumentException("Colors array cannot be empty.", nameof(colors));
        }

        if (colorArray.Length == 1)
        {
            return colorArray[0];
        }

        var totalR = 0f;
        var totalG = 0f;
        var totalB = 0f;
        var totalA = 0f;

        foreach (var color in colorArray)
        {
            totalR += color.R;
            totalG += color.G;
            totalB += color.B;
            totalA += color.A;
        }

        var count = colorArray.Length;
        return new Color(
            (byte)Math.Round(totalR / count),
            (byte)Math.Round(totalG / count),
            (byte)Math.Round(totalB / count),
            (byte)Math.Round(totalA / count)
        );
    }

    #endregion

    #region Gradient Generation

    /// <summary>
    ///     Creates a linear gradient between two colors.
    /// </summary>
    /// <param name="from">The starting color.</param>
    /// <param name="to">The ending color.</param>
    /// <param name="steps">The number of steps in the gradient.</param>
    /// <returns>An array of colors representing the gradient.</returns>
    /// <exception cref="ArgumentException">Thrown when steps is less than 2.</exception>
    public static Color[] CreateLinearGradient(this Color from, Color to, int steps)
    {
        if (steps < 2)
        {
            throw new ArgumentException("Steps must be at least 2.", nameof(steps));
        }

        var gradient = new Color[steps];

        for (var i = 0; i < steps; i++)
        {
            var t = (float)i / (steps - 1);
            gradient[i] = from.Lerp(to, t);
        }

        return gradient;
    }

    /// <summary>
    ///     Creates a multi-color gradient.
    /// </summary>
    /// <param name="colors">The colors to create a gradient from.</param>
    /// <param name="steps">The number of steps in the gradient.</param>
    /// <returns>An array of colors representing the gradient.</returns>
    /// <exception cref="ArgumentException">Thrown when colors array is null, empty, or has less than 2 colors.</exception>
    public static Color[] CreateMultiColorGradient(this IEnumerable<Color> colors, int steps)
    {
        var colorArray = colors?.ToArray() ?? throw new ArgumentException("Colors cannot be null.", nameof(colors));

        if (colorArray.Length < 2)
        {
            throw new ArgumentException("At least 2 colors are required.", nameof(colors));
        }

        if (steps < 2)
        {
            throw new ArgumentException("Steps must be at least 2.", nameof(steps));
        }

        var gradient = new Color[steps];
        var segmentLength = (float)(steps - 1) / (colorArray.Length - 1);

        for (var i = 0; i < steps; i++)
        {
            var position = (float)i;
            var segmentIndex = (int)(position / segmentLength);

            if (segmentIndex >= colorArray.Length - 1)
            {
                gradient[i] = colorArray[^1];
            }
            else
            {
                var t = (position - segmentIndex * segmentLength) / segmentLength;
                gradient[i] = colorArray[segmentIndex].Lerp(colorArray[segmentIndex + 1], t);
            }
        }

        return gradient;
    }

    /// <summary>
    ///     Creates a radial gradient pattern.
    /// </summary>
    /// <param name="center">The center color.</param>
    /// <param name="edge">The edge color.</param>
    /// <param name="radius">The radius of the gradient.</param>
    /// <param name="width">The width of the output array.</param>
    /// <param name="height">The height of the output array.</param>
    /// <returns>A 2D array of colors representing the radial gradient.</returns>
    public static Color[,] CreateRadialGradient(this Color center, Color edge, float radius, int width, int height)
    {
        var gradient = new Color[width, height];
        var centerX = width / 2f;
        var centerY = height / 2f;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var distance = MathF.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                var t = Math.Clamp(distance / radius, 0f, 1f);
                gradient[x, y] = center.Lerp(edge, t);
            }
        }

        return gradient;
    }

    #endregion

    #region Color Manipulation

    /// <summary>
    ///     Darkens a color by the specified amount.
    /// </summary>
    /// <param name="color">The color to darken.</param>
    /// <param name="amount">The amount to darken (0.0 to 1.0).</param>
    /// <returns>The darkened color.</returns>
    public static Color Darken(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        var factor = 1f - amount;

        return new Color(
            (byte)Math.Round(color.R * factor),
            (byte)Math.Round(color.G * factor),
            (byte)Math.Round(color.B * factor),
            color.A
        );
    }

    /// <summary>
    ///     Lightens a color by the specified amount.
    /// </summary>
    /// <param name="color">The color to lighten.</param>
    /// <param name="amount">The amount to lighten (0.0 to 1.0).</param>
    /// <returns>The lightened color.</returns>
    public static Color Lighten(this Color color, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);

        return new Color(
            (byte)Math.Round(color.R + (255 - color.R) * amount),
            (byte)Math.Round(color.G + (255 - color.G) * amount),
            (byte)Math.Round(color.B + (255 - color.B) * amount),
            color.A
        );
    }

    /// <summary>
    ///     Adjusts the saturation of a color.
    /// </summary>
    /// <param name="color">The color to adjust.</param>
    /// <param name="saturation">The saturation factor (0.0 = grayscale, 1.0 = original, >1.0 = more saturated).</param>
    /// <returns>The color with adjusted saturation.</returns>
    public static Color AdjustSaturation(this Color color, float saturation)
    {
        var gray = (byte)Math.Round(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);

        var r = (byte)Math.Clamp(gray + (color.R - gray) * saturation, 0, 255);
        var g = (byte)Math.Clamp(gray + (color.G - gray) * saturation, 0, 255);
        var b = (byte)Math.Clamp(gray + (color.B - gray) * saturation, 0, 255);

        return new Color(r, g, b, color.A);
    }

    /// <summary>
    ///     Converts a color to grayscale using luminance.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>The grayscale color.</returns>
    public static Color ToGrayscale(this Color color)
    {
        var gray = (byte)Math.Round(color.R * 0.299 + color.G * 0.587 + color.B * 0.114);
        return new Color(gray, gray, gray, color.A);
    }

    /// <summary>
    ///     Inverts a color.
    /// </summary>
    /// <param name="color">The color to invert.</param>
    /// <param name="invertAlpha">Whether to invert the alpha channel as well.</param>
    /// <returns>The inverted color.</returns>
    public static Color Invert(this Color color, bool invertAlpha = false)
    {
        return new Color(
            (byte)(255 - color.R),
            (byte)(255 - color.G),
            (byte)(255 - color.B),
            invertAlpha ? (byte)(255 - color.A) : color.A
        );
    }

    #endregion

    #region Color Analysis

    /// <summary>
    ///     Gets the luminance of a color (perceived brightness).
    /// </summary>
    /// <param name="color">The color to analyze.</param>
    /// <returns>The luminance value (0.0 to 1.0).</returns>
    public static float GetLuminance(this Color color)
    {
        return (color.R * 0.299f + color.G * 0.587f + color.B * 0.114f) / 255f;
    }

    /// <summary>
    ///     Determines if a color is considered "light" based on its luminance.
    /// </summary>
    /// <param name="color">The color to analyze.</param>
    /// <param name="threshold">The luminance threshold (default: 0.5).</param>
    /// <returns>True if the color is light, false otherwise.</returns>
    public static bool IsLight(this Color color, float threshold = 0.5f)
    {
        return color.GetLuminance() > threshold;
    }

    /// <summary>
    ///     Determines if a color is considered "dark" based on its luminance.
    /// </summary>
    /// <param name="color">The color to analyze.</param>
    /// <param name="threshold">The luminance threshold (default: 0.5).</param>
    /// <returns>True if the color is dark, false otherwise.</returns>
    public static bool IsDark(this Color color, float threshold = 0.5f)
    {
        return color.GetLuminance() <= threshold;
    }

    /// <summary>
    ///     Calculates the contrast ratio between two colors.
    /// </summary>
    /// <param name="color1">The first color.</param>
    /// <param name="color2">The second color.</param>
    /// <returns>The contrast ratio (1:1 to 21:1).</returns>
    public static float GetContrastRatio(this Color color1, Color color2)
    {
        var l1 = color1.GetLuminance();
        var l2 = color2.GetLuminance();

        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);

        return (lighter + 0.05f) / (darker + 0.05f);
    }

    /// <summary>
    ///     Gets the best contrasting color (black or white) for the given color.
    /// </summary>
    /// <param name="color">The color to find contrast for.</param>
    /// <returns>Either black or white, whichever provides better contrast.</returns>
    public static Color GetContrastingColor(this Color color)
    {
        return color.IsLight() ? Color.Black : Color.White;
    }

    #endregion

    #region HSV/HSL Conversion Utilities

    /// <summary>
    ///     Converts RGB to HSV (Hue, Saturation, Value).
    /// </summary>
    /// <param name="color">The RGB color to convert.</param>
    /// <returns>A tuple containing HSV values (H: 0-360, S: 0-1, V: 0-1).</returns>
    public static (float H, float S, float V) ToHsv(this Color color)
    {
        var r = color.RedFloat;
        var g = color.GreenFloat;
        var b = color.BlueFloat;

        var max = Math.Max(Math.Max(r, g), b);
        var min = Math.Min(Math.Min(r, g), b);
        var delta = max - min;

        var h = 0f;
        if (delta != 0)
        {
            if (max == r)
            {
                h = 60 * ((g - b) / delta % 6);
            }
            else if (max == g)
            {
                h = 60 * ((b - r) / delta + 2);
            }
            else
            {
                h = 60 * ((r - g) / delta + 4);
            }
        }

        if (h < 0)
        {
            h += 360;
        }

        var s = max == 0 ? 0 : delta / max;
        var v = max;

        return (h, s, v);
    }

    /// <summary>
    ///     Creates a color from HSV values.
    /// </summary>
    /// <param name="h">Hue (0-360 degrees).</param>
    /// <param name="s">Saturation (0.0-1.0).</param>
    /// <param name="v">Value (0.0-1.0).</param>
    /// <param name="a">Alpha (0.0-1.0), defaults to 1.0.</param>
    /// <returns>The RGB color.</returns>
    public static Color FromHsv(float h, float s, float v, float a = 1.0f)
    {
        h = h % 360;
        if (h < 0)
        {
            h += 360;
        }

        var c = v * s;
        var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        var m = v - c;

        float r, g, b;

        if (h < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (h < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (h < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (h < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (h < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else
        {
            r = c;
            g = 0;
            b = x;
        }

        return Color.FromFloat(r + m, g + m, b + m, a);
    }

    /// <summary>
    ///     Adjusts the hue of a color.
    /// </summary>
    /// <param name="color">The color to adjust.</param>
    /// <param name="hueShift">The amount to shift the hue (in degrees).</param>
    /// <returns>The color with adjusted hue.</returns>
    public static Color AdjustHue(this Color color, float hueShift)
    {
        var (h, s, v) = color.ToHsv();
        return FromHsv(h + hueShift, s, v, color.AlphaFloat);
    }

    #endregion
}
