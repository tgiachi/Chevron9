using Chevron9.Backends.Terminal.Utils;
using Chevron9.Shared.Graphics;
using Chevron9.Tests.Backends.Terminal;

namespace Chevron9.Tests.Backends.Terminal;

/// <summary>
///     Integration tests using TerminalEmulator to verify ANSI sequences work correctly.
///     These tests simulate a real terminal and verify the visual output.
/// </summary>
[TestFixture]
public class TerminalEmulatorIntegrationTests
{
    [Test]
    public void TerminalEmulator_WithSimpleText_ShouldRenderCorrectly()
    {
        var emulator = new TerminalEmulator(10, 5);
        var builder = new AnsiBuilder();

        var sequence = builder
            .Append("Hello")
            .Build();

        emulator.ProcessAnsiSequence(sequence);

        // Verify the text was written at cursor position (0,0)
        Assert.That(emulator.GetChar(0, 0).Visual, Is.EqualTo('H'));
        Assert.That(emulator.GetChar(1, 0).Visual, Is.EqualTo('e'));
        Assert.That(emulator.GetChar(2, 0).Visual, Is.EqualTo('l'));
        Assert.That(emulator.GetChar(3, 0).Visual, Is.EqualTo('l'));
        Assert.That(emulator.GetChar(4, 0).Visual, Is.EqualTo('o'));

        // Cursor should have moved to position (5, 0)
        Assert.That(emulator.CursorPosition.X, Is.EqualTo(5));
        Assert.That(emulator.CursorPosition.Y, Is.EqualTo(0));
    }

    [Test]
    public void TerminalEmulator_WithCursorMovement_ShouldPositionCorrectly()
    {
        var emulator = new TerminalEmulator(10, 5);
        var builder = new AnsiBuilder();

        var sequence = builder
            .CursorPosition(2, 3)
            .Append("X")
            .Build();

        emulator.ProcessAnsiSequence(sequence);

        // Verify cursor moved to (2, 3) 1-based and wrote 'X' at (2, 1) 0-based, then advanced to (3, 1)
        Assert.That(emulator.GetChar(2, 1).Visual, Is.EqualTo('X'));
        Assert.That(emulator.CursorPosition.X, Is.EqualTo(3.0f));
        Assert.That(emulator.CursorPosition.Y, Is.EqualTo(1.0f));
    }

    [Test]
    public void TerminalEmulator_WithColorText_ShouldApplyColors()
    {
        var emulator = new TerminalEmulator(10, 5);
        var builder = new AnsiBuilder();

        var sequence = builder
            .FgColor(Color.Red)
            .Append("Red")
            .FgColor(Color.Green)
            .Append("Green")
            .Build();

        emulator.ProcessAnsiSequence(sequence);

        // Verify colors were applied correctly
        Assert.That(emulator.GetChar(0, 0).Fg, Is.EqualTo(Color.Red));
        Assert.That(emulator.GetChar(1, 0).Fg, Is.EqualTo(Color.Red));
        Assert.That(emulator.GetChar(2, 0).Fg, Is.EqualTo(Color.Red));

        Assert.That(emulator.GetChar(3, 0).Fg, Is.EqualTo(Color.Green));
        Assert.That(emulator.GetChar(4, 0).Fg, Is.EqualTo(Color.Green));
        Assert.That(emulator.GetChar(5, 0).Fg, Is.EqualTo(Color.Green));
        Assert.That(emulator.GetChar(6, 0).Fg, Is.EqualTo(Color.Green));
        Assert.That(emulator.GetChar(7, 0).Fg, Is.EqualTo(Color.Green));
    }

    [Test]
    public void TerminalEmulator_WithBackgroundColor_ShouldApplyBackground()
    {
        var emulator = new TerminalEmulator(10, 5);
        var builder = new AnsiBuilder();

        var sequence = builder
            .BgColor(Color.Blue)
            .Append("Test")
            .Build();

        emulator.ProcessAnsiSequence(sequence);

        // Verify background color was applied
        Assert.That(emulator.GetChar(0, 0).Bg, Is.EqualTo(Color.Blue));
        Assert.That(emulator.GetChar(1, 0).Bg, Is.EqualTo(Color.Blue));
        Assert.That(emulator.GetChar(2, 0).Bg, Is.EqualTo(Color.Blue));
        Assert.That(emulator.GetChar(3, 0).Bg, Is.EqualTo(Color.Blue));
    }



    [Test]
    public void TerminalEmulator_WithClearScreen_ShouldClearAll()
    {
        var emulator = new TerminalEmulator(5, 3);
        var builder = new AnsiBuilder();

        // Fill screen with text first
        var fillSequence = builder
            .Append("AAAAA")
            .CursorPosition(2, 1)
            .Append("BBBBB")
            .CursorPosition(3, 1)
            .Append("CCCCC")
            .Build();

        emulator.ProcessAnsiSequence(fillSequence);

        // Verify screen is filled
        Assert.That(emulator.GetChar(0, 0).Visual, Is.EqualTo('A'));
        Assert.That(emulator.GetChar(0, 1).Visual, Is.EqualTo('B'));
        Assert.That(emulator.GetChar(0, 2).Visual, Is.EqualTo('C'));

        // Clear screen
        var clearSequence = new AnsiBuilder()
            .ClearScreen()
            .Build();

        emulator.ProcessAnsiSequence(clearSequence);

        // Verify screen is cleared (spaces)
        Assert.That(emulator.GetChar(0, 0).Visual, Is.EqualTo(' '));
        Assert.That(emulator.GetChar(0, 1).Visual, Is.EqualTo(' '));
        Assert.That(emulator.GetChar(0, 2).Visual, Is.EqualTo(' '));
    }

    [Test]
    public void TerminalEmulator_WithComplexSequence_ShouldWorkCorrectly()
    {
        var emulator = new TerminalEmulator(20, 10);
        var builder = new AnsiBuilder();

        var sequence = builder
            .ClearScreen()
            .CursorPosition(1, 1)
            .FgColor(Color.Red)
            .BgColor(Color.White)
            .Append("ERROR: ")
            .FgColor(Color.Black)
            .Append("File not found")
            .CursorPosition(3, 1)
            .FgColor(Color.Green)
            .BgColor(Color.Black)
            .Append("✓ Operation completed")
            .Build();

        emulator.ProcessAnsiSequence(sequence);

        // Verify error message
        Assert.That(emulator.GetChar(0, 0).Visual, Is.EqualTo('E'));
        Assert.That(emulator.GetChar(0, 0).Fg, Is.EqualTo(Color.Red));
        Assert.That(emulator.GetChar(0, 0).Bg, Is.EqualTo(Color.White));

        // Verify success message
        Assert.That(emulator.GetChar(0, 2).Visual, Is.EqualTo('✓'));
        Assert.That(emulator.GetChar(0, 2).Fg, Is.EqualTo(Color.Green));
        Assert.That(emulator.GetChar(0, 2).Bg, Is.EqualTo(Color.Black));
    }

    [Test]
    public void TerminalEmulator_WithBoundaryConditions_ShouldHandleCorrectly()
    {
        var emulator = new TerminalEmulator(5, 3);

        // Try to move cursor beyond boundaries
        var builder = new AnsiBuilder();
        var sequence = builder
            .CursorPosition(10, 10)  // Beyond boundaries
            .Append("X")
            .Build();

        emulator.ProcessAnsiSequence(sequence);

        // Should be clamped to valid boundaries (4, 2) 0-based, then advance to (5, 2) after writing
        Assert.That(emulator.CursorPosition.X, Is.EqualTo(5.0f));
        Assert.That(emulator.CursorPosition.Y, Is.EqualTo(2.0f));
        Assert.That(emulator.GetChar(4, 2).Visual, Is.EqualTo('X'));
    }

    [Test]
    public void TerminalEmulator_WithRgbColors_ShouldApplyRgbColors()
    {
        var emulator = new TerminalEmulator(10, 5);
        var builder = new AnsiBuilder();

        var sequence = builder
            .FgColorRgb(255, 0, 0)  // Red
            .Append("RGB")
            .Build();

        emulator.ProcessAnsiSequence(sequence);

        // Verify RGB color was applied
        var redColor = new Color(255, 0, 0);
        Assert.That(emulator.GetChar(0, 0).Fg, Is.EqualTo(redColor));
        Assert.That(emulator.GetChar(1, 0).Fg, Is.EqualTo(redColor));
        Assert.That(emulator.GetChar(2, 0).Fg, Is.EqualTo(redColor));
    }
}
