using Chevron9.Shared.Graphics;

namespace Chevron9.Shared.Primitives;

/// <summary>
///     Represents a single logical cell in a grid or buffer.
///     A cell has a "visual" payload (generic) plus foreground/background colors.
/// </summary>
/// <typeparam name="TVisual">
///     The visual payload type. Examples:
///     - int (Unicode codepoint) for terminal
///     - SpriteRef (atlas rect) for tile/sprite rendering
///     - GlyphRef (font + glyph index) for bitmap fonts
/// </typeparam>
public readonly record struct Cell<TVisual> : IEquatable<Cell<TVisual>>
{
    public Cell(TVisual visual, Color fg, Color bg)
    {
        Visual = visual;
        Fg = fg;
        Bg = bg;
    }

    /// <summary> Gets the visual payload (glyph/sprite/etc). </summary>
    public TVisual Visual { get; }

    /// <summary> Gets the foreground color. </summary>
    public Color Fg { get; }

    /// <summary> Gets the background color. </summary>
    public Color Bg { get; }

    /// <summary>
    ///     Returns true if this cell is visually equal to another.
    ///     Equality is based on Visual, Fg, and Bg.
    /// </summary>
    public bool Equals(Cell<TVisual> other)
    {
        return EqualityComparer<TVisual>.Default.Equals(Visual, other.Visual) &&
               Fg.Equals(other.Fg) &&
               Bg.Equals(other.Bg);
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Visual, Fg, Bg);
    }

    public override string ToString()
    {
        return $"Cell({Visual}, Fg={Fg}, Bg={Bg})";
    }

    /// <summary>
    ///     Creates a new cell with the same visual and background but different foreground color
    /// </summary>
    /// <param name="fg">New foreground color</param>
    /// <returns>Cell with updated foreground color</returns>
    public Cell<TVisual> WithForeground(Color fg)
    {
        return new Cell<TVisual>(Visual, fg, Bg);
    }

    /// <summary>
    ///     Creates a new cell with the same visual and foreground but different background color
    /// </summary>
    /// <param name="bg">New background color</param>
    /// <returns>Cell with updated background color</returns>
    public Cell<TVisual> WithBackground(Color bg)
    {
        return new Cell<TVisual>(Visual, Fg, bg);
    }

    /// <summary>
    ///     Creates a new cell with the same colors but different visual payload
    /// </summary>
    /// <param name="visual">New visual payload</param>
    /// <returns>Cell with updated visual</returns>
    public Cell<TVisual> WithVisual(TVisual visual)
    {
        return new Cell<TVisual>(visual, Fg, Bg);
    }
}
