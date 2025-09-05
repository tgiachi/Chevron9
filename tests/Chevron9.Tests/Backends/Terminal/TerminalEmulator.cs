using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Backends.Terminal;

/// <summary>
///     Simple terminal emulator for integration testing of ANSI sequences.
///     Maintains a character grid with colors and interprets ANSI escape codes.
/// </summary>
public class TerminalEmulator
{
    private readonly Cell<char>[,] _grid;
    private int _cursorX;
    private int _cursorY;
    private Color _currentFgColor;
    private Color _currentBgColor;
    private bool _cursorVisible;

    /// <summary>
    ///     Gets the width of the terminal grid.
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     Gets the height of the terminal grid.
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     Gets the current cursor position.
    /// </summary>
    public Position CursorPosition => new Position(_cursorX, _cursorY);

    /// <summary>
    ///     Gets whether the cursor is visible.
    /// </summary>
    public bool CursorVisible => _cursorVisible;

    /// <summary>
    ///     Initializes a new TerminalEmulator with the specified dimensions.
    /// </summary>
    /// <param name="width">Terminal width in characters</param>
    /// <param name="height">Terminal height in characters</param>
    public TerminalEmulator(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new Cell<char>[height, width];
        _cursorX = 0;
        _cursorY = 0;
        _currentFgColor = Color.White;
        _currentBgColor = Color.Black;
        _cursorVisible = true;

        // Initialize grid with spaces
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _grid[y, x] = new Cell<char>(' ', _currentFgColor, _currentBgColor);
            }
        }
    }

    /// <summary>
    ///     Processes an ANSI sequence string and updates the terminal state.
    /// </summary>
    /// <param name="ansiSequence">The ANSI sequence to process</param>
    public void ProcessAnsiSequence(string ansiSequence)
    {
        int i = 0;
        while (i < ansiSequence.Length)
        {
            if (ansiSequence[i] == '\e' && i + 1 < ansiSequence.Length && ansiSequence[i + 1] == '[')
            {
                // ANSI escape sequence
                i = ProcessEscapeSequence(ansiSequence, i);
            }
            else
            {
                // Regular character
                WriteChar(ansiSequence[i]);
                i++;
            }
        }
    }

    private int ProcessEscapeSequence(string sequence, int startIndex)
    {
        int i = startIndex + 2; // Skip \e[
        var parameters = new List<string>();
        string currentParam = "";
        char command = '\0';

        // Parse parameters until we find the command character
        while (i < sequence.Length)
        {
            char c = sequence[i];
            if (char.IsLetter(c) || c == '@' || c == '`')
            {
                command = c;
                break;
            }
            else if (c == ';')
            {
                parameters.Add(currentParam);
                currentParam = "";
            }
            else
            {
                currentParam += c;
            }
            i++;
        }

        // Add the last parameter if any
        if (!string.IsNullOrEmpty(currentParam))
        {
            parameters.Add(currentParam);
        }

        if (command != '\0')
        {
            ExecuteCommand(command, parameters.ToArray());
        }

        return i + 1;
    }

    private void ExecuteCommand(char command, string[] parameters)
    {
        switch (command)
        {
            case 'H': // Cursor Position
            case 'f': // Horizontal and Vertical Position
                SetCursorPosition(parameters);
                break;
            case 'A': // Cursor Up
                MoveCursorUp(ParseInt(parameters, 0, 1));
                break;
            case 'B': // Cursor Down
                MoveCursorDown(ParseInt(parameters, 0, 1));
                break;
            case 'C': // Cursor Forward
                MoveCursorForward(ParseInt(parameters, 0, 1));
                break;
            case 'D': // Cursor Back
                MoveCursorBack(ParseInt(parameters, 0, 1));
                break;
            case 'J': // Erase in Display
                EraseDisplay(ParseInt(parameters, 0, 0));
                break;
            case 'K': // Erase in Line
                EraseLine(ParseInt(parameters, 0, 0));
                break;
            case 'm': // Select Graphic Rendition (colors and styles)
                SetGraphicRendition(parameters);
                break;
            case 'h': // Set Mode
                SetMode(parameters);
                break;
            case 'l': // Reset Mode
                ResetMode(parameters);
                break;
        }
    }

    private void SetCursorPosition(string[] parameters)
    {
        int row = ParseInt(parameters, 0, 1) - 1; // 1-based to 0-based
        int col = ParseInt(parameters, 1, 1) - 1; // 1-based to 0-based
        _cursorX = Math.Clamp(col, 0, Width - 1);
        _cursorY = Math.Clamp(row, 0, Height - 1);
    }

    private void MoveCursorUp(int lines)
    {
        _cursorY = Math.Max(0, _cursorY - lines);
    }

    private void MoveCursorDown(int lines)
    {
        _cursorY = Math.Min(Height - 1, _cursorY + lines);
    }

    private void MoveCursorForward(int cols)
    {
        _cursorX = Math.Min(Width - 1, _cursorX + cols);
    }

    private void MoveCursorBack(int cols)
    {
        _cursorX = Math.Max(0, _cursorX - cols);
    }

    private void EraseDisplay(int mode)
    {
        switch (mode)
        {
            case 0: // Clear from cursor to end of screen
                for (int y = _cursorY; y < Height; y++)
                {
                    int startX = (y == _cursorY) ? _cursorX : 0;
                    for (int x = startX; x < Width; x++)
                    {
                        _grid[y, x] = new Cell<char>(' ', _currentFgColor, _currentBgColor);
                    }
                }
                break;
            case 1: // Clear from cursor to beginning of screen
                for (int y = 0; y <= _cursorY; y++)
                {
                    int endX = (y == _cursorY) ? _cursorX + 1 : Width;
                    for (int x = 0; x < endX; x++)
                    {
                        _grid[y, x] = new Cell<char>(' ', _currentFgColor, _currentBgColor);
                    }
                }
                break;
            case 2: // Clear entire screen
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        _grid[y, x] = new Cell<char>(' ', _currentFgColor, _currentBgColor);
                    }
                }
                break;
        }
    }

    private void EraseLine(int mode)
    {
        switch (mode)
        {
            case 0: // Clear from cursor to end of line
                for (int x = _cursorX; x < Width; x++)
                {
                    _grid[_cursorY, x] = new Cell<char>(' ', _currentFgColor, _currentBgColor);
                }
                break;
            case 1: // Clear from cursor to beginning of line
                for (int x = 0; x <= _cursorX; x++)
                {
                    _grid[_cursorY, x] = new Cell<char>(' ', _currentFgColor, _currentBgColor);
                }
                break;
            case 2: // Clear entire line
                for (int x = 0; x < Width; x++)
                {
                    _grid[_cursorY, x] = new Cell<char>(' ', _currentFgColor, _currentBgColor);
                }
                break;
        }
    }

    private void SetGraphicRendition(string[] parameters)
    {
        int i = 0;
        while (i < parameters.Length)
        {
            int code = ParseInt(new[] { parameters[i] }, 0, 0);
            switch (code)
            {
                case 0: // Reset all
                    _currentFgColor = Color.White;
                    _currentBgColor = Color.Black;
                    break;
                case 30: _currentFgColor = Color.Black; break;
                case 31: _currentFgColor = Color.Red; break;
                case 32: _currentFgColor = Color.Green; break;
                case 33: _currentFgColor = Color.Yellow; break;
                case 34: _currentFgColor = Color.Blue; break;
                case 35: _currentFgColor = Color.Magenta; break;
                case 36: _currentFgColor = Color.Cyan; break;
                case 37: _currentFgColor = Color.White; break;
                case 40: _currentBgColor = Color.Black; break;
                case 41: _currentBgColor = Color.Red; break;
                case 42: _currentBgColor = Color.Green; break;
                case 43: _currentBgColor = Color.Yellow; break;
                case 44: _currentBgColor = Color.Blue; break;
                case 45: _currentBgColor = Color.Magenta; break;
                case 46: _currentBgColor = Color.Cyan; break;
                case 47: _currentBgColor = Color.White; break;
                case 38: // 256-color or RGB foreground
                    if (i + 2 < parameters.Length && parameters[i + 1] == "5")
                    {
                        // 256-color: \e[38;5;<n>m
                        int colorIndex = ParseInt(new[] { parameters[i + 2] }, 0, 0);
                        _currentFgColor = ColorFrom256Index(colorIndex);
                        i += 2; // Skip the processed parameters
                    }
                    else if (i + 4 < parameters.Length && parameters[i + 1] == "2")
                    {
                        // RGB: \e[38;2;<r>;<g>;<b>m
                        int r = ParseInt(new[] { parameters[i + 2] }, 0, 0);
                        int g = ParseInt(new[] { parameters[i + 3] }, 0, 0);
                        int b = ParseInt(new[] { parameters[i + 4] }, 0, 0);
                        _currentFgColor = new Color((byte)r, (byte)g, (byte)b);
                        i += 4; // Skip the processed parameters
                    }
                    break;
                case 48: // 256-color or RGB background
                    if (i + 2 < parameters.Length && parameters[i + 1] == "5")
                    {
                        // 256-color: \e[48;5;<n>m
                        int colorIndex = ParseInt(new[] { parameters[i + 2] }, 0, 0);
                        _currentBgColor = ColorFrom256Index(colorIndex);
                        i += 2; // Skip the processed parameters
                    }
                    else if (i + 4 < parameters.Length && parameters[i + 1] == "2")
                    {
                        // RGB: \e[48;2;<r>;<g>;<b>m
                        int r = ParseInt(new[] { parameters[i + 2] }, 0, 0);
                        int g = ParseInt(new[] { parameters[i + 3] }, 0, 0);
                        int b = ParseInt(new[] { parameters[i + 4] }, 0, 0);
                        _currentBgColor = new Color((byte)r, (byte)g, (byte)b);
                        i += 4; // Skip the processed parameters
                    }
                    break;
            }
            i++;
        }
    }

    private void SetMode(string[] parameters)
    {
        foreach (var param in parameters)
        {
            if (param == "?25") // Hide cursor
            {
                _cursorVisible = false;
            }
        }
    }

    private void ResetMode(string[] parameters)
    {
        foreach (var param in parameters)
        {
            if (param == "?25") // Show cursor
            {
                _cursorVisible = true;
            }
        }
    }

    private void WriteChar(char c)
    {
        if (_cursorX >= 0 && _cursorX < Width &&
            _cursorY >= 0 && _cursorY < Height)
        {
            _grid[_cursorY, _cursorX] = new Cell<char>(c, _currentFgColor, _currentBgColor);
            _cursorX++;
        }
    }

    /// <summary>
    ///     Gets the character at the specified position.
    /// </summary>
    /// <param name="x">X coordinate (column)</param>
    /// <param name="y">Y coordinate (row)</param>
    /// <returns>The character cell at the position</returns>
    public Cell<char> GetChar(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return _grid[y, x];
        }
        return new Cell<char>(' ', Color.White, Color.Black);
    }

    /// <summary>
    ///     Gets the character at the specified position.
    /// </summary>
    /// <param name="position">The position to get the character from</param>
    /// <returns>The character cell at the position</returns>
    public Cell<char> GetChar(Position position)
    {
        return GetChar((int)position.X, (int)position.Y);
    }

    /// <summary>
    ///     Gets a string representation of the terminal grid for debugging.
    /// </summary>
    /// <returns>String representation of the terminal</returns>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                sb.Append(_grid[y, x].Visual);
            }
            if (y < Height - 1) sb.AppendLine();
        }
        return sb.ToString();
    }

    private static int ParseInt(string[] parameters, int index, int defaultValue)
    {
        if (index < parameters.Length && int.TryParse(parameters[index], out int result))
        {
            return result;
        }
        return defaultValue;
    }

    private static Color ColorFrom256Index(int index)
    {
        // Simple 256-color palette approximation
        if (index < 16)
        {
            // Basic colors
            return index switch
            {
                0 => Color.Black,
                1 => Color.Red,
                2 => Color.Green,
                3 => Color.Yellow,
                4 => Color.Blue,
                5 => Color.Magenta,
                6 => Color.Cyan,
                7 => Color.White,
                8 => Color.Gray,
                9 => Color.Red,
                10 => Color.Green,
                11 => Color.Yellow,
                12 => Color.Blue,
                13 => Color.Magenta,
                14 => Color.Cyan,
                15 => Color.White,
                _ => Color.White
            };
        }
        else if (index < 232)
        {
            // 216 color cube
            int cubeIndex = index - 16;
            int r = (cubeIndex / 36) % 6;
            int g = (cubeIndex / 6) % 6;
            int b = cubeIndex % 6;
            return new Color(
                (byte)(r * 51),
                (byte)(g * 51),
                (byte)(b * 51)
            );
        }
        else
        {
            // Grayscale
            int gray = (index - 232) * 10 + 8;
            return new Color((byte)gray, (byte)gray, (byte)gray);
        }
    }
}
