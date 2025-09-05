using System.Text;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Render command to draw a single glyph (character) at a specific position
///     Primary command for terminal rendering, supports Unicode codepoints with foreground/background colors
/// </summary>
/// <param name="Position">Position where the glyph should be drawn</param>
/// <param name="Codepoint">Unicode codepoint of the character to draw</param>
/// <param name="Foreground">Foreground color of the glyph</param>
/// <param name="Background">Background color of the glyph</param>
public sealed record DrawGlyphCommand(
    Position Position,
    int Codepoint,
    Color Foreground,
    Color Background
) : RenderCommand
{
    /// <summary>
    ///     Creates a DrawGlyphCommand with the specified character
    /// </summary>
    /// <param name="position">Position where the glyph should be drawn</param>
    /// <param name="character">Character to draw</param>
    /// <param name="foreground">Foreground color of the glyph</param>
    /// <param name="background">Background color of the glyph</param>
    /// <returns>DrawGlyphCommand with the character's codepoint</returns>
    public static DrawGlyphCommand FromChar(Position position, char character, Color foreground, Color background)
    {
        return new DrawGlyphCommand(position, character, foreground, background);
    }

    /// <summary>
    ///     Creates a DrawGlyphCommand with the specified Rune
    /// </summary>
    /// <param name="position">Position where the glyph should be drawn</param>
    /// <param name="rune">Rune to draw</param>
    /// <param name="foreground">Foreground color of the glyph</param>
    /// <param name="background">Background color of the glyph</param>
    /// <returns>DrawGlyphCommand with the rune's codepoint</returns>
    public static DrawGlyphCommand FromRune(Position position, Rune rune, Color foreground, Color background)
    {
        return new DrawGlyphCommand(position, rune.Value, foreground, background);
    }

    /// <summary>
    ///     Gets the character representation of this glyph's codepoint
    /// </summary>
    /// <returns>Character if valid, null if the codepoint cannot be represented as a single char</returns>
    public char? AsChar()
    {
        if (Codepoint >= char.MinValue && Codepoint <= char.MaxValue)
        {
            return (char)Codepoint;
        }
        return null;
    }

    /// <summary>
    ///     Gets the Rune representation of this glyph's codepoint
    /// </summary>
    /// <returns>Rune representing the codepoint</returns>
    public Rune AsRune()
    {
        return new Rune(Codepoint);
    }

    /// <summary>
    ///     Checks if this glyph represents a printable character
    /// </summary>
    /// <returns>True if the glyph is printable, false if it's a control character</returns>
    public bool IsPrintable()
    {
        var rune = new Rune(Codepoint);
        return !Rune.IsControl(rune) && !Rune.IsWhiteSpace(rune) || Codepoint == ' ';
    }

    /// <summary>
    ///     Checks if this glyph represents whitespace
    /// </summary>
    /// <returns>True if the glyph is whitespace</returns>
    public bool IsWhitespace()
    {
        var rune = new Rune(Codepoint);
        return Rune.IsWhiteSpace(rune);
    }
}
