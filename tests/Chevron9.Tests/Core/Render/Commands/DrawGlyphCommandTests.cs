using System.Text;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render.Commands;

[TestFixture]
public class DrawGlyphCommandTests
{
    [Test]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var position = new Position(10, 20);
        var codepoint = 65; // 'A'
        var foreground = Color.Red;
        var background = Color.Blue;

        var command = new DrawGlyphCommand(position, codepoint, foreground, background);

        Assert.That(command.Position, Is.EqualTo(position));
        Assert.That(command.Codepoint, Is.EqualTo(codepoint));
        Assert.That(command.Foreground, Is.EqualTo(foreground));
        Assert.That(command.Background, Is.EqualTo(background));
    }

    [Test]
    public void FromChar_ShouldCreateCommandWithCharCodepoint()
    {
        var position = new Position(5, 15);
        var character = 'X';
        var foreground = Color.Green;
        var background = Color.Yellow;

        var command = DrawGlyphCommand.FromChar(position, character, foreground, background);

        Assert.That(command.Position, Is.EqualTo(position));
        Assert.That(command.Codepoint, Is.EqualTo((int)character));
        Assert.That(command.Foreground, Is.EqualTo(foreground));
        Assert.That(command.Background, Is.EqualTo(background));
    }

    [Test]
    public void FromRune_ShouldCreateCommandWithRuneCodepoint()
    {
        var position = new Position(8, 12);
        var rune = new Rune('€'); // Euro symbol
        var foreground = Color.Black;
        var background = Color.White;

        var command = DrawGlyphCommand.FromRune(position, rune, foreground, background);

        Assert.That(command.Position, Is.EqualTo(position));
        Assert.That(command.Codepoint, Is.EqualTo(rune.Value));
        Assert.That(command.Foreground, Is.EqualTo(foreground));
        Assert.That(command.Background, Is.EqualTo(background));
    }

    [Test]
    public void FromRune_WithUnicodeCharacter_ShouldHandleCorrectly()
    {
        var position = new Position(0, 0);
        var rune = new Rune(0x1F3AE); // Game controller emoji
        var foreground = Color.Red;
        var background = Color.Black;

        var command = DrawGlyphCommand.FromRune(position, rune, foreground, background);

        Assert.That(command.Position, Is.EqualTo(position));
        Assert.That(command.Codepoint, Is.EqualTo(rune.Value));
        Assert.That(command.Foreground, Is.EqualTo(foreground));
        Assert.That(command.Background, Is.EqualTo(background));
    }

    [Test]
    public void AsChar_WithBasicCharacter_ShouldReturnChar()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 65, Color.Black, Color.White);

        var result = command.AsChar();

        Assert.That(result, Is.EqualTo('A'));
    }

    [Test]
    public void AsChar_WithUnicodeOutsideCharRange_ShouldReturnNull()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 0x1F3AE, Color.Black, Color.White); // Game controller emoji

        var result = command.AsChar();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void AsChar_WithValidCharRange_ShouldReturnChar()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 8364, Color.Black, Color.White); // Euro symbol

        var result = command.AsChar();

        Assert.That(result, Is.EqualTo('€'));
    }

    [Test]
    public void AsRune_ShouldReturnRuneWithSameCodepoint()
    {
        var codepoint = 0x1F3AE; // Game controller emoji
        var command = new DrawGlyphCommand(new Position(0, 0), codepoint, Color.Black, Color.White);

        var result = command.AsRune();

        Assert.That(result.Value, Is.EqualTo(codepoint));
    }

    [Test]
    public void AsRune_WithBasicCharacter_ShouldReturnValidRune()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 65, Color.Black, Color.White); // 'A'

        var result = command.AsRune();

        Assert.That(result.Value, Is.EqualTo(65));
        Assert.That(result.ToString(), Is.EqualTo("A"));
    }

    [Test]
    public void IsPrintable_WithPrintableCharacter_ShouldReturnTrue()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 65, Color.Black, Color.White); // 'A'

        var result = command.IsPrintable();

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsPrintable_WithSpace_ShouldReturnTrue()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 32, Color.Black, Color.White); // Space

        var result = command.IsPrintable();

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsPrintable_WithControlCharacter_ShouldReturnFalse()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 9, Color.Black, Color.White); // Tab

        var result = command.IsPrintable();

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPrintable_WithNewline_ShouldReturnFalse()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 10, Color.Black, Color.White); // Newline

        var result = command.IsPrintable();

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsPrintable_WithUnicodeSymbol_ShouldReturnTrue()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 0x1F3AE, Color.Black, Color.White); // Game controller emoji

        var result = command.IsPrintable();

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsWhitespace_WithSpace_ShouldReturnTrue()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 32, Color.Black, Color.White); // Space

        var result = command.IsWhitespace();

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsWhitespace_WithTab_ShouldReturnTrue()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 9, Color.Black, Color.White); // Tab

        var result = command.IsWhitespace();

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsWhitespace_WithNewline_ShouldReturnTrue()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 10, Color.Black, Color.White); // Newline

        var result = command.IsWhitespace();

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsWhitespace_WithRegularCharacter_ShouldReturnFalse()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 65, Color.Black, Color.White); // 'A'

        var result = command.IsWhitespace();

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsWhitespace_WithUnicodeCharacter_ShouldReturnFalse()
    {
        var command = new DrawGlyphCommand(new Position(0, 0), 0x1F3AE, Color.Black, Color.White); // Game controller emoji

        var result = command.IsWhitespace();

        Assert.That(result, Is.False);
    }

    [Test]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var position = new Position(10, 20);
        var codepoint = 65;
        var foreground = Color.Red;
        var background = Color.Blue;

        var command1 = new DrawGlyphCommand(position, codepoint, foreground, background);
        var command2 = new DrawGlyphCommand(position, codepoint, foreground, background);

        Assert.That(command1, Is.EqualTo(command2));
        Assert.That(command1.GetHashCode(), Is.EqualTo(command2.GetHashCode()));
    }

    [Test]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        var command1 = new DrawGlyphCommand(new Position(10, 20), 65, Color.Red, Color.Blue);
        var command2 = new DrawGlyphCommand(new Position(10, 20), 66, Color.Red, Color.Blue); // Different codepoint

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void ToString_ShouldIncludeMainProperties()
    {
        var command = new DrawGlyphCommand(new Position(5, 10), 65, Color.Red, Color.Blue);

        var result = command.ToString();

        Assert.That(result, Does.Contain("DrawGlyphCommand"));
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("65"));
    }

    [Test]
    public void RecordBehavior_ShouldSupportWithExpressions()
    {
        var original = new DrawGlyphCommand(new Position(5, 10), 65, Color.Red, Color.Blue);

        var modified = original with { Foreground = Color.Green };

        Assert.That(modified.Position, Is.EqualTo(original.Position));
        Assert.That(modified.Codepoint, Is.EqualTo(original.Codepoint));
        Assert.That(modified.Foreground, Is.EqualTo(Color.Green));
        Assert.That(modified.Background, Is.EqualTo(original.Background));
    }
}
