using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Chevron9.Core.Data.Input;
using Chevron9.Core.Types;

namespace Chevron9.Backends.Terminal.Parsers;

/// <summary>
///     High-performance ANSI escape sequence parser for terminal input
///     Parses raw byte buffer input and extracts keyboard events and mouse events
/// </summary>
public sealed partial class AnsiParser
{
    [GeneratedRegex(@"\e\[([ABCDH])", RegexOptions.Compiled)]
    private static partial Regex AnsiSequenceRegex();

    [GeneratedRegex(@"\e\[<(\d+);(\d+);(\d+)([Mm])", RegexOptions.Compiled)]
    private static partial Regex MouseEventRegex();

    [GeneratedRegex(@"\e\[(\d+)~", RegexOptions.Compiled)]
    private static partial Regex FunctionKeyRegex();

    [GeneratedRegex(@"\e\[1;(\d+)([A-Z])", RegexOptions.Compiled)]
    private static partial Regex ModifiedArrowKeyRegex();

    private readonly Dictionary<string, InputKey> _keyMappings;
    private readonly StringBuilder _buffer = new();

    public AnsiParser()
    {
        _keyMappings = InitializeKeyMappings();
    }

    /// <summary>
    ///     Parses raw byte buffer input and returns keyboard and mouse events
    /// </summary>
    /// <param name="buffer">Raw byte buffer from terminal input</param>
    /// <returns>Tuple containing list of key events and list of mouse events</returns>
    public (List<KeyEvent> keys, List<MouseEvent> mouse) Parse(ReadOnlySpan<byte> buffer)
    {
        var keys = new List<KeyEvent>();
        var mouse = new List<MouseEvent>();

        if (buffer.IsEmpty)
            return (keys, mouse);

        var input = Encoding.UTF8.GetString(buffer);
        _buffer.Clear();
        _buffer.Append(input);

        var position = 0;
        while (position < _buffer.Length)
        {
            var consumed = ProcessNextSequence(_buffer.ToString(), position, keys, mouse);
            if (consumed == 0)
            {
                // Single character input
                var ch = _buffer[position];
                if (ch >= 32 && ch <= 126) // Printable ASCII
                {
                    keys.Add(new KeyEvent(InputKeys.GetKey(ch), InputKeyModifierType.None, true));
                }
                else if (ch == '\r' || ch == '\n')
                {
                    // Check if this is a trailing line ending after meaningful content
                    var isTrailing = false;
                    var nextPosition = position + 1;
                    if (ch == '\r' && position + 1 < _buffer.Length && _buffer[position + 1] == '\n')
                    {
                        // \r\n sequence - check if it's at the end AND we have content before it
                        nextPosition = position + 2;
                    }

                    // Only treat as trailing if at end of buffer AND either:
                    // 1) we have content before this line ending, OR 
                    // 2) it's specifically a \r\n sequence (not single \r or \n)
                    var isCRLF = ch == '\r' && position + 1 < _buffer.Length && _buffer[position + 1] == '\n';
                    isTrailing = (nextPosition >= _buffer.Length) && (position > 0 || isCRLF);

                    if (!isTrailing)
                    {
                        keys.Add(new KeyEvent(InputKeys.Enter, InputKeyModifierType.None, true));
                    }

                    // Skip LF if it follows CR
                    if (ch == '\r' && position + 1 < _buffer.Length && _buffer[position + 1] == '\n')
                    {
                        position++; // Skip the following LF
                    }
                }
                else if (ch == '\t')
                {
                    keys.Add(new KeyEvent(InputKeys.Tab, InputKeyModifierType.None, true));
                }
                else if (ch == '\b' || ch == '\x7f')
                {
                    keys.Add(new KeyEvent(InputKeys.Backspace, InputKeyModifierType.None, true));
                }
                else if (ch == '\x1b')
                {
                    keys.Add(new KeyEvent(InputKeys.Escape, InputKeyModifierType.None, true));
                }

                position++;
            }
            else
            {
                position += consumed;
            }
        }

        return (keys, mouse);
    }

    private static int ProcessNextSequence(string input, int position, List<KeyEvent> keys, List<MouseEvent> mouse)
    {
        if (position >= input.Length)
            return 0;

        // Check for escape sequences
        if (input[position] == '\x1b' && position + 1 < input.Length)
        {
            var remaining = input[position..];

            // Try mouse event first (SGR format)
            var mouseMatch = MouseEventRegex().Match(remaining);
            if (mouseMatch.Success && mouseMatch.Index == 0)
            {
                ParseMouseEvent(mouseMatch, mouse);
                return mouseMatch.Length;
            }

            // Try modified arrow keys
            var modifiedArrowMatch = ModifiedArrowKeyRegex().Match(remaining);
            if (modifiedArrowMatch.Success && modifiedArrowMatch.Index == 0)
            {
                ParseModifiedArrowKey(modifiedArrowMatch, keys);
                return modifiedArrowMatch.Length;
            }

            // Try function keys
            var functionKeyMatch = FunctionKeyRegex().Match(remaining);
            if (functionKeyMatch.Success && functionKeyMatch.Index == 0)
            {
                ParseFunctionKey(functionKeyMatch, keys);
                return functionKeyMatch.Length;
            }

            // Try general ANSI sequence
            var ansiMatch = AnsiSequenceRegex().Match(remaining);
            if (ansiMatch.Success && ansiMatch.Index == 0)
            {
                ParseAnsiSequence(ansiMatch, keys);
                return ansiMatch.Length;
            }

            // Alt+ combinations
            if (remaining.Length >= 2)
            {
                var altChar = remaining[1];
                if (altChar >= 'a' && altChar <= 'z')
                {
                    keys.Add(new KeyEvent(InputKeys.GetKey(char.ToUpper(altChar, CultureInfo.InvariantCulture)), InputKeyModifierType.Alt, true));
                    return 2;
                }
                if (altChar >= 'A' && altChar <= 'Z')
                {
                    keys.Add(new KeyEvent(InputKeys.GetKey(altChar), InputKeyModifierType.Alt, true));
                    return 2;
                }
            }

            // If we reach here and the sequence starts with \e[, it's an invalid ANSI sequence
            // Skip the entire invalid sequence until we find the next escape or end of input
            if (remaining.Length >= 2 && remaining[0] == '\x1b' && remaining[1] == '[')
            {
                // Find the next escape sequence or end of input
                for (int i = 2; i < remaining.Length; i++)
                {
                    var ch = remaining[i];
                    if (ch == '\x1b')
                    {
                        // Found start of next escape sequence, consume up to here
                        return i;
                    }
                }
                // No more escape sequences, consume all remaining characters
                return remaining.Length;
            }
        }

        // Ctrl+ combinations (excluding special characters like BS, Tab, LF, CR, etc.)
        if (input[position] >= 1 && input[position] <= 26 &&
            input[position] != 8 && input[position] != 9 && input[position] != 10 && input[position] != 13)
        {
            var ctrlChar = (char)('A' + input[position] - 1);
            keys.Add(new KeyEvent(InputKeys.GetKey(ctrlChar), InputKeyModifierType.Control, true));
            return 1;
        }

        return 0;
    }

    private static void ParseMouseEvent(Match match, List<MouseEvent> mouse)
    {
        if (!int.TryParse(match.Groups[1].Value, out var button) ||
            !int.TryParse(match.Groups[2].Value, out var x) ||
            !int.TryParse(match.Groups[3].Value, out var y))
            return;

        var isPress = match.Groups[4].Value == "M";
        var mouseButton = ParseMouseButton(button);
        var modifiers = ParseMouseModifiers(button);

        mouse.Add(new MouseEvent(mouseButton, x - 1, y - 1, isPress ? MouseAction.Down : MouseAction.Up, modifiers));
    }

    private static void ParseModifiedArrowKey(Match match, List<KeyEvent> keys)
    {
        if (!int.TryParse(match.Groups[1].Value, out var modifier))
            return;

        var keyChar = match.Groups[2].Value[0];
        var inputKey = keyChar switch
        {
            'A' => InputKeys.Up,
            'B' => InputKeys.Down,
            'C' => InputKeys.Right,
            'D' => InputKeys.Left,
            _ => (InputKey?)null
        };

        if (inputKey != null)
        {
            var modifierType = ParseKeyModifier(modifier);
            keys.Add(new KeyEvent(inputKey.Value, modifierType, true));
        }
    }

    private static void ParseFunctionKey(Match match, List<KeyEvent> keys)
    {
        if (!int.TryParse(match.Groups[1].Value, out var keyCode))
            return;

        var inputKey = keyCode switch
        {
            1 => InputKeys.Home,
            2 => InputKeys.Insert,
            3 => InputKeys.Delete,
            4 => InputKeys.End,
            5 => InputKeys.PageUp,
            6 => InputKeys.PageDown,
            11 => InputKeys.F1,
            12 => InputKeys.F2,
            13 => InputKeys.F3,
            14 => InputKeys.F4,
            15 => InputKeys.F5,
            17 => InputKeys.F6,
            18 => InputKeys.F7,
            19 => InputKeys.F8,
            20 => InputKeys.F9,
            21 => InputKeys.F10,
            23 => InputKeys.F11,
            24 => InputKeys.F12,
            _ => (InputKey?)null
        };

        if (inputKey != null)
        {
            keys.Add(new KeyEvent(inputKey.Value, InputKeyModifierType.None, true));
        }
    }

    private static void ParseAnsiSequence(Match match, List<KeyEvent> keys)
    {
        var fullSequence = match.Value;

        // Handle basic arrow keys and navigation
        if (fullSequence.EndsWith('A'))
            keys.Add(new KeyEvent(InputKeys.Up, InputKeyModifierType.None, true));
        else if (fullSequence.EndsWith('B'))
            keys.Add(new KeyEvent(InputKeys.Down, InputKeyModifierType.None, true));
        else if (fullSequence.EndsWith('C'))
            keys.Add(new KeyEvent(InputKeys.Right, InputKeyModifierType.None, true));
        else if (fullSequence.EndsWith('D'))
            keys.Add(new KeyEvent(InputKeys.Left, InputKeyModifierType.None, true));
        else if (fullSequence.EndsWith('H'))
            keys.Add(new KeyEvent(InputKeys.Home, InputKeyModifierType.None, true));
    }

    private static MouseButtonType ParseMouseButton(int button)
    {
        var buttonCode = button & 0x03;
        return buttonCode switch
        {
            0 => MouseButtonType.Left,
            1 => MouseButtonType.Middle,
            2 => MouseButtonType.Right,
            _ => MouseButtonType.Left
        };
    }

    private static InputKeyModifierType ParseMouseModifiers(int button)
    {
        var modifiers = InputKeyModifierType.None;

        if ((button & 0x04) != 0) modifiers |= InputKeyModifierType.Shift;
        if ((button & 0x08) != 0) modifiers |= InputKeyModifierType.Alt;
        if ((button & 0x10) != 0) modifiers |= InputKeyModifierType.Control;

        return modifiers;
    }

    private static InputKeyModifierType ParseKeyModifier(int modifier)
    {
        return (modifier - 1) switch
        {
            1 => InputKeyModifierType.Shift,
            2 => InputKeyModifierType.Alt,
            3 => InputKeyModifierType.Alt | InputKeyModifierType.Shift,
            4 => InputKeyModifierType.Control,
            5 => InputKeyModifierType.Control | InputKeyModifierType.Shift,
            6 => InputKeyModifierType.Control | InputKeyModifierType.Alt,
            7 => InputKeyModifierType.Control | InputKeyModifierType.Alt | InputKeyModifierType.Shift,
            _ => InputKeyModifierType.None
        };
    }

    private static Dictionary<string, InputKey> InitializeKeyMappings()
    {
        return new Dictionary<string, InputKey>
        {
            ["\e[A"] = InputKeys.Up,
            ["\e[B"] = InputKeys.Down,
            ["\e[C"] = InputKeys.Right,
            ["\e[D"] = InputKeys.Left,
            ["\e[H"] = InputKeys.Home,
            ["\e[F"] = InputKeys.End,
            ["\e[1~"] = InputKeys.Home,
            ["\e[2~"] = InputKeys.Insert,
            ["\e[3~"] = InputKeys.Delete,
            ["\e[4~"] = InputKeys.End,
            ["\e[5~"] = InputKeys.PageUp,
            ["\e[6~"] = InputKeys.PageDown,
            ["\eOP"] = InputKeys.F1,
            ["\eOQ"] = InputKeys.F2,
            ["\eOR"] = InputKeys.F3,
            ["\eOS"] = InputKeys.F4
        };
    }
}

/// <summary>
///     Represents a keyboard event from terminal input
/// </summary>
/// <param name="Key">The input key that was pressed</param>
/// <param name="Modifiers">Modifier keys held during the press</param>
/// <param name="IsPressed">Whether the key was pressed (true) or released (false)</param>
public readonly record struct KeyEvent(InputKey Key, InputKeyModifierType Modifiers, bool IsPressed);

/// <summary>
///     Represents a mouse event from terminal input  
/// </summary>
/// <param name="Button">The mouse button involved in the event</param>
/// <param name="X">X coordinate (0-based)</param>
/// <param name="Y">Y coordinate (0-based)</param>
/// <param name="Action">The mouse action (press, release, move)</param>
/// <param name="Modifiers">Modifier keys held during the event</param>
public readonly record struct MouseEvent(MouseButtonType Button, int X, int Y, MouseAction Action, InputKeyModifierType Modifiers);
