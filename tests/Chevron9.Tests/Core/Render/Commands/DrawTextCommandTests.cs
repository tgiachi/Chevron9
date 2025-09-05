using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Render.Commands;

[TestFixture]
public class DrawTextCommandTests
{
    [Test]
    public void Constructor_ShouldInitializeAllProperties()
    {
        var position = new Position(25, 50);
        var text = "Hello, World!";
        var fontSize = 42.0f;
        var color = Color.DarkGreen;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.Position, Is.EqualTo(position));
        Assert.That(command.Text, Is.EqualTo(text));
        Assert.That(command.FontSize, Is.EqualTo(fontSize));
        Assert.That(command.Color, Is.EqualTo(color));
    }

    [Test]
    public void Constructor_WithEmptyText_ShouldAllowEmptyString()
    {
        var position = new Position(10, 20);
        var text = "";
        var fontSize = 12.0f;
        var color = Color.Black;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.Text, Is.EqualTo(""));
    }

    [Test]
    public void Constructor_WithNullText_ShouldAllowNull()
    {
        var position = new Position(10, 20);
        string? text = null;
        var fontSize = 12.0f;
        var color = Color.Black;

        var command = new DrawTextCommand(text!, position, color, fontSize);

        Assert.That(command.Text, Is.Null);
    }

    [Test]
    public void Constructor_WithZeroFontSize_ShouldAllowZeroFontSize()
    {
        var position = new Position(5, 15);
        var text = "Test";
        var fontSize = 0.0f;
        var color = Color.Red;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.FontSize, Is.EqualTo(0.0f));
    }

    [Test]
    public void Constructor_WithNegativeFontSize_ShouldAllowNegativeFontSize()
    {
        var position = new Position(5, 15);
        var text = "Test";
        var fontSize = -1.0f;
        var color = Color.Blue;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.FontSize, Is.EqualTo(-1.0f));
    }

    [Test]
    public void Constructor_WithUnicodeText_ShouldHandleUnicodeCorrectly()
    {
        var position = new Position(30, 40);
        var text = "Hello 🌍 World! 你好 €";
        var fontSize = 16.0f;
        var color = Color.Purple;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.Text, Is.EqualTo("Hello 🌍 World! 你好 €"));
    }

    [Test]
    public void Constructor_WithMultilineText_ShouldHandleNewlines()
    {
        var position = new Position(0, 0);
        var text = "Line 1\nLine 2\r\nLine 3";
        var fontSize = 14.0f;
        var color = Color.Orange;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.Text, Is.EqualTo("Line 1\nLine 2\r\nLine 3"));
    }

    [Test]
    public void Constructor_WithLongText_ShouldHandleLongStrings()
    {
        var position = new Position(100, 200);
        var text = new string('A', 10000);
        var fontSize = 12.0f;
        var color = Color.Cyan;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.Text.Length, Is.EqualTo(10000));
        Assert.That(command.Text, Does.StartWith("AAAA"));
    }

    [Test]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        var position = new Position(10, 20);
        var text = "Test Text";
        var fontSize = 16.0f;
        var color = Color.Yellow;

        var command1 = new DrawTextCommand(text, position, color, fontSize);
        var command2 = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command1, Is.EqualTo(command2));
        Assert.That(command1.GetHashCode(), Is.EqualTo(command2.GetHashCode()));
    }

    [Test]
    public void Equality_WithDifferentPosition_ShouldNotBeEqual()
    {
        var text = "Same Text";
        var fontSize = 12.0f;
        var color = Color.Red;

        var command1 = new DrawTextCommand(text, new Position(10, 20), color, fontSize);
        var command2 = new DrawTextCommand(text, new Position(11, 20), color, fontSize);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentText_ShouldNotBeEqual()
    {
        var position = new Position(10, 20);
        var fontSize = 12.0f;
        var color = Color.Red;

        var command1 = new DrawTextCommand("Text 1", position, color, fontSize);
        var command2 = new DrawTextCommand("Text 2", position, color, fontSize);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentFontSize_ShouldNotBeEqual()
    {
        var position = new Position(10, 20);
        var text = "Same Text";
        var color = Color.Red;

        var command1 = new DrawTextCommand(text, position, color, 12.0f);
        var command2 = new DrawTextCommand(text, position, color, 14.0f);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void Equality_WithDifferentColor_ShouldNotBeEqual()
    {
        var position = new Position(10, 20);
        var text = "Same Text";
        var fontSize = 12.0f;

        var command1 = new DrawTextCommand(text, position, Color.Red, fontSize);
        var command2 = new DrawTextCommand(text, position, Color.Blue, fontSize);

        Assert.That(command1, Is.Not.EqualTo(command2));
    }

    [Test]
    public void ToString_ShouldIncludeMainProperties()
    {
        var command = new DrawTextCommand("Sample", new Position(5, 10), Color.Green, 16.0f);

        var result = command.ToString();

        Assert.That(result, Does.Contain("DrawTextCommand"));
        Assert.That(result, Does.Contain("5"));
        Assert.That(result, Does.Contain("10"));
        Assert.That(result, Does.Contain("Sample"));
        Assert.That(result, Does.Contain("16"));
    }

    [Test]
    public void RecordBehavior_ShouldSupportWithExpressions()
    {
        var original = new DrawTextCommand("Original", new Position(5, 10), Color.Red, 12.0f);

        var modified = original with { Text = "Modified" };

        Assert.That(modified.Position, Is.EqualTo(original.Position));
        Assert.That(modified.Text, Is.EqualTo("Modified"));
        Assert.That(modified.FontSize, Is.EqualTo(original.FontSize));
        Assert.That(modified.Color, Is.EqualTo(original.Color));
    }

    [Test]
    public void RecordBehavior_WithMultipleChanges_ShouldUpdateAllSpecifiedProperties()
    {
        var original = new DrawTextCommand("Original", new Position(5, 10), Color.Red, 12.0f);

        var modified = original with { Position = new Position(20, 30), FontSize = 16.0f };

        Assert.That(modified.Position, Is.EqualTo(new Position(20, 30)));
        Assert.That(modified.Text, Is.EqualTo(original.Text));
        Assert.That(modified.FontSize, Is.EqualTo(16.0f));
        Assert.That(modified.Color, Is.EqualTo(original.Color));
    }

    [Test]
    public void Constructor_WithSpecialCharacters_ShouldHandleSpecialChars()
    {
        var position = new Position(0, 0);
        var text = "Special: \t\r\n\"'\\/@#$%^&*()";
        var fontSize = 12.0f;
        var color = Color.White;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.Text, Is.EqualTo("Special: \t\r\n\"'\\/@#$%^&*()"));
    }

    [Test]
    public void Constructor_WithWhitespaceText_ShouldPreserveWhitespace()
    {
        var position = new Position(10, 20);
        var text = "   Leading and trailing spaces   ";
        var fontSize = 14.0f;
        var color = Color.Gray;

        var command = new DrawTextCommand(text, position, color, fontSize);

        Assert.That(command.Text, Is.EqualTo("   Leading and trailing spaces   "));
    }
}
