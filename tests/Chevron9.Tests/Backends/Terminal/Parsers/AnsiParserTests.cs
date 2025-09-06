using System.Text;
using Chevron9.Backends.Terminal.Parsers;
using Chevron9.Core.Data.Input;
using Chevron9.Core.Types;

namespace Chevron9.Tests.Backends.Terminal.Parsers;

[TestFixture]
public class AnsiParserTests
{
    private AnsiParser _parser = null!;

    [SetUp]
    public void SetUp()
    {
        _parser = new AnsiParser();
    }

    [Test]
    public void Parse_WithEmptyBuffer_ShouldReturnEmptyResults()
    {
        var buffer = ReadOnlySpan<byte>.Empty;

        var (keys, mouse) = _parser.Parse(buffer);

        Assert.That(keys.Count, Is.EqualTo(0));
        Assert.That(mouse.Count, Is.EqualTo(0));
    }

    [Test]
    public void Parse_WithPrintableAscii_ShouldReturnKeyEvents()
    {
        var input = "Hello"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(5));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.GetKey('H')));
        Assert.That(keys[1].Key, Is.EqualTo(InputKeys.GetKey('e')));
        Assert.That(keys[2].Key, Is.EqualTo(InputKeys.GetKey('l')));
        Assert.That(keys[3].Key, Is.EqualTo(InputKeys.GetKey('l')));
        Assert.That(keys[4].Key, Is.EqualTo(InputKeys.GetKey('o')));
        Assert.That(mouse.Count, Is.EqualTo(0));
    }

    [Test]
    public void Parse_WithEnterKey_ShouldReturnEnterKeyEvent()
    {
        var input = "\r"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Enter));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithNewlineKey_ShouldReturnEnterKeyEvent()
    {
        var input = "\n"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Enter));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithTabKey_ShouldReturnTabKeyEvent()
    {
        var input = "\t"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Tab));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithBackspaceKey_ShouldReturnBackspaceKeyEvent()
    {
        var input = "\b"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Backspace));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithDeleteKey_ShouldReturnBackspaceKeyEvent()
    {
        var input = "\x7f"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Backspace));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithEscapeKey_ShouldReturnEscapeKeyEvent()
    {
        var input = "\x1b"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Escape));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithArrowUpKey_ShouldReturnArrowUpKeyEvent()
    {
        var input = "\e[A"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Up));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithArrowDownKey_ShouldReturnArrowDownKeyEvent()
    {
        var input = "\e[B"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Down));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithArrowRightKey_ShouldReturnArrowRightKeyEvent()
    {
        var input = "\e[C"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Right));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithArrowLeftKey_ShouldReturnArrowLeftKeyEvent()
    {
        var input = "\e[D"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Left));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithFunctionKey_ShouldReturnFunctionKeyEvent()
    {
        var input = "\e[11~"u8; // F1

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.F1));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithHomeKey_ShouldReturnHomeKeyEvent()
    {
        var input = "\e[1~"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Home));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithEndKey_ShouldReturnEndKeyEvent()
    {
        var input = "\e[4~"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.End));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithPageUpKey_ShouldReturnPageUpKeyEvent()
    {
        var input = "\e[5~"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.PageUp));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithPageDownKey_ShouldReturnPageDownKeyEvent()
    {
        var input = "\e[6~"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.PageDown));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithDeleteFunctionKey_ShouldReturnDeleteKeyEvent()
    {
        var input = "\e[3~"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Delete));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithCtrlA_ShouldReturnCtrlAKeyEvent()
    {
        var input = "\x01"u8; // Ctrl+A

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.GetKey('A')));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.Control));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithCtrlZ_ShouldReturnCtrlZKeyEvent()
    {
        var input = "\x1a"u8; // Ctrl+Z

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.GetKey('Z')));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.Control));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithAltA_ShouldReturnAltAKeyEvent()
    {
        var input = "\ea"u8; // Alt+A

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.GetKey('A')));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.Alt));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithShiftArrowUp_ShouldReturnShiftArrowUpKeyEvent()
    {
        var input = "\e[1;2A"u8; // Shift+Arrow Up

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Up));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.Shift));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithCtrlArrowRight_ShouldReturnCtrlArrowRightKeyEvent()
    {
        var input = "\e[1;5C"u8; // Ctrl+Arrow Right

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(1));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.Right));
        Assert.That(keys[0].Modifiers, Is.EqualTo(InputKeyModifierType.Control));
        Assert.That(keys[0].IsPressed, Is.True);
    }

    [Test]
    public void Parse_WithMouseLeftClick_ShouldReturnMouseEvent()
    {
        var input = "\e[<0;10;5M"u8; // Left button press at (10,5)

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(0));
        Assert.That(mouse.Count, Is.EqualTo(1));
        Assert.That(mouse[0].Button, Is.EqualTo(MouseButtonType.Left));
        Assert.That(mouse[0].X, Is.EqualTo(9)); // 0-based
        Assert.That(mouse[0].Y, Is.EqualTo(4)); // 0-based
        Assert.That(mouse[0].Action, Is.EqualTo(MouseAction.Down));
        Assert.That(mouse[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
    }

    [Test]
    public void Parse_WithMouseRightRelease_ShouldReturnMouseEvent()
    {
        var input = "\e[<2;15;10m"u8; // Right button release at (15,10)

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(0));
        Assert.That(mouse.Count, Is.EqualTo(1));
        Assert.That(mouse[0].Button, Is.EqualTo(MouseButtonType.Right));
        Assert.That(mouse[0].X, Is.EqualTo(14)); // 0-based
        Assert.That(mouse[0].Y, Is.EqualTo(9)); // 0-based
        Assert.That(mouse[0].Action, Is.EqualTo(MouseAction.Up));
        Assert.That(mouse[0].Modifiers, Is.EqualTo(InputKeyModifierType.None));
    }

    [Test]
    public void Parse_WithMouseMiddleClickWithCtrl_ShouldReturnMouseEventWithModifiers()
    {
        var input = "\e[<17;20;15M"u8; // Middle button press with Ctrl at (20,15)

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(0));
        Assert.That(mouse.Count, Is.EqualTo(1));
        Assert.That(mouse[0].Button, Is.EqualTo(MouseButtonType.Middle));
        Assert.That(mouse[0].X, Is.EqualTo(19)); // 0-based
        Assert.That(mouse[0].Y, Is.EqualTo(14)); // 0-based
        Assert.That(mouse[0].Action, Is.EqualTo(MouseAction.Down));
        Assert.That(mouse[0].Modifiers, Is.EqualTo(InputKeyModifierType.Control));
    }

    [Test]
    public void Parse_WithMixedInput_ShouldReturnMixedEvents()
    {
        var input = "A\e[B\e[<0;5;5M"u8; // 'A' + Arrow Down + Mouse Click

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(2));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.GetKey('A')));
        Assert.That(keys[1].Key, Is.EqualTo(InputKeys.Down));

        Assert.That(mouse.Count, Is.EqualTo(1));
        Assert.That(mouse[0].Button, Is.EqualTo(MouseButtonType.Left));
        Assert.That(mouse[0].X, Is.EqualTo(4));
        Assert.That(mouse[0].Y, Is.EqualTo(4));
    }

    [Test]
    public void Parse_WithLongSequence_ShouldHandleAllEvents()
    {
        var input = "Hello\e[A\e[B\e[C\e[D\r\n"u8;

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(9)); // 5 chars + 4 arrows + enter + newline
        Assert.That(mouse.Count, Is.EqualTo(0));

        // Check first few characters
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.GetKey('H')));
        Assert.That(keys[1].Key, Is.EqualTo(InputKeys.GetKey('e')));
        Assert.That(keys[2].Key, Is.EqualTo(InputKeys.GetKey('l')));
        Assert.That(keys[3].Key, Is.EqualTo(InputKeys.GetKey('l')));
        Assert.That(keys[4].Key, Is.EqualTo(InputKeys.GetKey('o')));

        // Check arrow keys
        Assert.That(keys[5].Key, Is.EqualTo(InputKeys.Up));
        Assert.That(keys[6].Key, Is.EqualTo(InputKeys.Down));
        Assert.That(keys[7].Key, Is.EqualTo(InputKeys.Right));
        Assert.That(keys[8].Key, Is.EqualTo(InputKeys.Left));
    }

    [Test]
    public void Parse_WithInvalidSequence_ShouldSkipInvalidParts()
    {
        var input = "A\e[ZZ\e[BX"u8; // Valid 'A', invalid sequence, valid Arrow Down, valid 'X'

        var (keys, mouse) = _parser.Parse(input);

        Assert.That(keys.Count, Is.EqualTo(3));
        Assert.That(keys[0].Key, Is.EqualTo(InputKeys.GetKey('A')));
        Assert.That(keys[1].Key, Is.EqualTo(InputKeys.Down));
        Assert.That(keys[2].Key, Is.EqualTo(InputKeys.GetKey('X')));
    }

    [Test]
    public void Parse_WithUtf8Characters_ShouldHandleCorrectly()
    {
        var input = Encoding.UTF8.GetBytes("Héllo");

        var (keys, mouse) = _parser.Parse(input);

        // Should handle UTF-8 encoded characters correctly
        Assert.That(keys.Count, Is.GreaterThan(0));
        Assert.That(mouse.Count, Is.EqualTo(0));
    }
}
