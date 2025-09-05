namespace Chevron9.Shared.Graphics;

/// <summary>
/// Represents a color with Red, Green, Blue, and Alpha (RGBA) components.
/// Each component is a byte value ranging from 0 to 255.
/// </summary>
public readonly partial struct Color : IEquatable<Color>
{
    /// <summary>
    /// The red component of the color (0-255).
    /// </summary>
    public byte R { get; }

    /// <summary>
    /// The green component of the color (0-255).
    /// </summary>
    public byte G { get; }

    /// <summary>
    /// The blue component of the color (0-255).
    /// </summary>
    public byte B { get; }

    /// <summary>
    /// The alpha (transparency) component of the color (0-255).
    /// 0 is fully transparent, 255 is fully opaque.
    /// </summary>
    public byte A { get; }

    /// <summary>
    /// Initializes a new instance of the Color struct with the specified RGBA values.
    /// </summary>
    /// <param name="r">The red component (0-255).</param>
    /// <param name="g">The green component (0-255).</param>
    /// <param name="b">The blue component (0-255).</param>
    /// <param name="a">The alpha component (0-255). Defaults to 255 (fully opaque).</param>
    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Initializes a new instance of the Color struct with the specified RGB values and full opacity.
    /// </summary>
    /// <param name="r">The red component (0-255).</param>
    /// <param name="g">The green component (0-255).</param>
    /// <param name="b">The blue component (0-255).</param>
    public Color(byte r, byte g, byte b) : this(r, g, b, 255)
    {
    }

    /// <summary>
    /// Creates a color from floating-point RGBA values (0.0 to 1.0).
    /// </summary>
    /// <param name="r">The red component (0.0-1.0).</param>
    /// <param name="g">The green component (0.0-1.0).</param>
    /// <param name="b">The blue component (0.0-1.0).</param>
    /// <param name="a">The alpha component (0.0-1.0). Defaults to 1.0 (fully opaque).</param>
    /// <returns>A new Color instance.</returns>
    public static Color FromFloat(float r, float g, float b, float a = 1.0f)
    {
        return new Color(
            (byte)Math.Clamp(r * 255, 0, 255),
            (byte)Math.Clamp(g * 255, 0, 255),
            (byte)Math.Clamp(b * 255, 0, 255),
            (byte)Math.Clamp(a * 255, 0, 255)
        );
    }

    /// <summary>
    /// Creates a color from a hexadecimal string representation.
    /// Supports formats: "#RGB", "#ARGB", "#RRGGBB", "#AARRGGBB"
    /// </summary>
    /// <param name="hex">The hexadecimal color string.</param>
    /// <returns>A new Color instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the hex string format is invalid.</exception>
    public static Color FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new ArgumentException("Hex string cannot be null or empty.", nameof(hex));

        hex = hex.TrimStart('#');

        return hex.Length switch
        {
            3 => ParseRgbHex(hex),
            4 => ParseArgbHex(hex),
            6 => ParseRrggbbHex(hex),
            8 => ParseAarrggbbHex(hex),
            _ => throw new ArgumentException($"Invalid hex color format: #{hex}", nameof(hex))
        };
    }

    private static Color ParseRgbHex(string hex)
    {
        try
        {
            var r = Convert.ToByte(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
            var g = Convert.ToByte(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
            var b = Convert.ToByte(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
            return new Color(r, g, b);
        }
        catch (FormatException)
        {
            throw new ArgumentException($"Invalid hex color format: #{hex}", nameof(hex));
        }
    }

    private static Color ParseArgbHex(string hex)
    {
        var a = Convert.ToByte(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
        var r = Convert.ToByte(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
        var g = Convert.ToByte(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
        var b = Convert.ToByte(hex.Substring(3, 1) + hex.Substring(3, 1), 16);
        return new Color(r, g, b, a);
    }

    private static Color ParseRrggbbHex(string hex)
    {
        var r = Convert.ToByte(hex.Substring(0, 2), 16);
        var g = Convert.ToByte(hex.Substring(2, 2), 16);
        var b = Convert.ToByte(hex.Substring(4, 2), 16);
        return new Color(r, g, b);
    }

    private static Color ParseAarrggbbHex(string hex)
    {
        var a = Convert.ToByte(hex.Substring(0, 2), 16);
        var r = Convert.ToByte(hex.Substring(2, 2), 16);
        var g = Convert.ToByte(hex.Substring(4, 2), 16);
        var b = Convert.ToByte(hex.Substring(6, 2), 16);
        return new Color(r, g, b, a);
    }

    /// <summary>
    /// Converts the color to a hexadecimal string representation.
    /// </summary>
    /// <param name="includeAlpha">Whether to include the alpha component in the output.</param>
    /// <returns>A hexadecimal color string (e.g., "#FF0000" or "#FFFF0000").</returns>
    public string ToHex(bool includeAlpha = false)
    {
        if (includeAlpha)
        {
            return $"#{A:X2}{R:X2}{G:X2}{B:X2}";
        }
        return $"#{R:X2}{G:X2}{B:X2}";
    }

    /// <summary>
    /// Gets the red component as a floating-point value (0.0 to 1.0).
    /// </summary>
    public float RedFloat => R / 255.0f;

    /// <summary>
    /// Gets the green component as a floating-point value (0.0 to 1.0).
    /// </summary>
    public float GreenFloat => G / 255.0f;

    /// <summary>
    /// Gets the blue component as a floating-point value (0.0 to 1.0).
    /// </summary>
    public float BlueFloat => B / 255.0f;

    /// <summary>
    /// Gets the alpha component as a floating-point value (0.0 to 1.0).
    /// </summary>
    public float AlphaFloat => A / 255.0f;

    /// <summary>
    /// Creates a new color with the specified alpha value.
    /// </summary>
    /// <param name="alpha">The new alpha value (0-255).</param>
    /// <returns>A new Color instance with the specified alpha.</returns>
    public Color WithAlpha(byte alpha)
    {
        return new Color(R, G, B, alpha);
    }

    /// <summary>
    /// Creates a new color with the specified alpha value as a float (0.0 to 1.0).
    /// </summary>
    /// <param name="alpha">The new alpha value (0.0-1.0).</param>
    /// <returns>A new Color instance with the specified alpha.</returns>
    public Color WithAlpha(float alpha)
    {
        return new Color(R, G, B, (byte)Math.Clamp(alpha * 255, 0, 255));
    }

    /// <summary>
    /// Determines whether the specified Color is equal to the current Color.
    /// </summary>
    /// <param name="other">The Color to compare with the current Color.</param>
    /// <returns>true if the specified Color is equal to the current Color; otherwise, false.</returns>
    public bool Equals(Color other)
    {
        return R == other.R && G == other.G && B == other.B && A == other.A;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Color.
    /// </summary>
    /// <param name="obj">The object to compare with the current Color.</param>
    /// <returns>true if the specified object is equal to the current Color; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this Color.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    /// <summary>
    /// Returns a string representation of this Color.
    /// </summary>
    /// <returns>A string that represents this Color.</returns>
    public override string ToString()
    {
        return $"Color(R:{R}, G:{G}, B:{B}, A:{A})";
    }

    /// <summary>
    /// Determines whether two Color instances are equal.
    /// </summary>
    /// <param name="left">The first Color to compare.</param>
    /// <param name="right">The second Color to compare.</param>
    /// <returns>true if the Colors are equal; otherwise, false.</returns>
    public static bool operator ==(Color left, Color right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two Color instances are not equal.
    /// </summary>
    /// <param name="left">The first Color to compare.</param>
    /// <param name="right">The second Color to compare.</param>
    /// <returns>true if the Colors are not equal; otherwise, false.</returns>
    public static bool operator !=(Color left, Color right)
    {
        return !left.Equals(right);
    }
}
