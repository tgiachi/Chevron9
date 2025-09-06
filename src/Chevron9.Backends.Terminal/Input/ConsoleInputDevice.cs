using System.Text;
using Chevron9.Backends.Terminal.Utils;
using Chevron9.Backends.Terminal.Parsers;
using Chevron9.Core.Data.Input;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Types;
using Chevron9.Shared.Primitives;

namespace Chevron9.Backends.Terminal.Input;

/// <summary>
///     Terminal-based input device that processes keyboard and mouse input from console
///     Parses ANSI escape sequences for advanced input detection and supports key repeat functionality
///     Optimized for 30+ FPS game loops with non-blocking I/O operations
/// </summary>
public class ConsoleInputDevice : IInputDevice, IDisposable
{
    private readonly Stream _reader;
    private readonly AnsiParser _parser;
    private readonly byte[] _inputBuffer = new byte[4096];

    // Input state tracking
    private readonly HashSet<(InputKey key, InputKeyModifierType modifiers)> _pressedThisFrame = new();
    private readonly HashSet<(InputKey key, InputKeyModifierType modifiers)> _releasedThisFrame = new();
    private readonly HashSet<(InputKey key, InputKeyModifierType modifiers)> _currentlyDown = new();

    // Key repeat tracking
    private readonly Dictionary<(InputKey key, InputKeyModifierType modifiers), DateTime> _keyPressStartTimes = new();
    private readonly Dictionary<(InputKey key, InputKeyModifierType modifiers), DateTime> _lastKeyRepeatTimes = new();
    private readonly HashSet<(InputKey key, InputKeyModifierType modifiers)> _repeatedThisFrame = new();
    private readonly List<(InputKey key, InputKeyModifierType modifiers)> _keysToRepeat = new();

    // Mouse state tracking  
    private readonly HashSet<MouseButtonType> _mousePressedThisFrame = new();
    private readonly HashSet<MouseButtonType> _mouseReleasedThisFrame = new();
    private readonly HashSet<MouseButtonType> _mouseCurrentlyDown = new();
    private readonly HashSet<MouseButtonType> _mouseClickedThisFrame = new();

    private Position _currentMousePosition = new(0, 0);
    private Position _mouseDelta = new(0, 0);
    private Position _mouseWheelDelta = new(0, 0);
    private InputKeyModifierType _activeModifiers = InputKeyModifierType.None;

    // Key repeat configuration  
    public TimeSpan KeyRepeatDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    public ConsoleInputDevice()
    {
        Console.InputEncoding = Encoding.UTF8;

        InitializeTerminal();

        _reader = Console.OpenStandardInput(4096);
        _parser = new AnsiParser();
    }

    public void Poll()
    {
        // Clear previous frame state
        _pressedThisFrame.Clear();
        _releasedThisFrame.Clear();
        _repeatedThisFrame.Clear();
        _mousePressedThisFrame.Clear();
        _mouseReleasedThisFrame.Clear();
        _mouseClickedThisFrame.Clear();
        _mouseDelta = new Position(0, 0);
        _mouseWheelDelta = new Position(0, 0);

        // Process key repeats for currently held keys
        ProcessKeyRepeats();

        // Read available input data with non-blocking approach
        try
        {
            // Only read if there's data available and the stream supports non-blocking reads
            if (_reader.CanRead && Console.KeyAvailable)
            {
                // Use a very short timeout to avoid blocking
                var readTask = _reader.ReadAsync(_inputBuffer, 0, _inputBuffer.Length);

                // Wait for a maximum of 1ms to avoid blocking the frame
                if (readTask.Wait(1))
                {
                    var bytesRead = readTask.Result;
                    if (bytesRead > 0)
                    {
                        var inputSpan = _inputBuffer.AsSpan(0, bytesRead);
                        var (keyEvents, mouseEvents) = _parser.Parse(inputSpan);

                        ProcessKeyEvents(keyEvents);
                        ProcessMouseEvents(mouseEvents);
                    }
                }
                // If timeout occurred, we simply skip input processing for this frame
            }
        }
        catch (InvalidOperationException)
        {
            // Handle case where input is not available
        }
        catch (TimeoutException)
        {
            // Reading took too long, skip for this frame
        }
    }

    private void ProcessKeyEvents(List<KeyEvent> keyEvents)
    {
        foreach (var keyEvent in keyEvents)
        {
            var keyTuple = (keyEvent.Key, keyEvent.Modifiers);

            if (keyEvent.IsPressed)
            {
                // Key was pressed
                if (!_currentlyDown.Contains(keyTuple))
                {
                    _pressedThisFrame.Add(keyTuple);
                    _currentlyDown.Add(keyTuple);

                    // Start tracking for key repeat
                    _keyPressStartTimes[keyTuple] = DateTime.UtcNow;
                    _lastKeyRepeatTimes[keyTuple] = DateTime.UtcNow;
                }
            }
            else
            {
                // Key was released
                if (_currentlyDown.Contains(keyTuple))
                {
                    _releasedThisFrame.Add(keyTuple);
                    _currentlyDown.Remove(keyTuple);

                    // Stop tracking for key repeat
                    _keyPressStartTimes.Remove(keyTuple);
                    _lastKeyRepeatTimes.Remove(keyTuple);
                }
            }
        }

        // Update active modifiers based on currently pressed keys
        UpdateActiveModifiers();
    }

    private void UpdateActiveModifiers()
    {
        _activeModifiers = InputKeyModifierType.None;
        foreach (var (key, modifiers) in _currentlyDown)
        {
            _activeModifiers |= modifiers;
        }
    }

    private void ProcessMouseEvents(List<MouseEvent> mouseEvents)
    {
        foreach (var mouseEvent in mouseEvents)
        {
            // Update mouse position
            var oldPosition = _currentMousePosition;
            _currentMousePosition = new Position(mouseEvent.X, mouseEvent.Y);
            _mouseDelta = new Position(
                _currentMousePosition.X - oldPosition.X,
                _currentMousePosition.Y - oldPosition.Y
            );

            switch (mouseEvent.Action)
            {
                case MouseAction.Down:
                    if (!_mouseCurrentlyDown.Contains(mouseEvent.Button))
                    {
                        _mousePressedThisFrame.Add(mouseEvent.Button);
                        _mouseCurrentlyDown.Add(mouseEvent.Button);
                    }
                    break;

                case MouseAction.Up:
                    if (_mouseCurrentlyDown.Contains(mouseEvent.Button))
                    {
                        _mouseReleasedThisFrame.Add(mouseEvent.Button);
                        _mouseCurrentlyDown.Remove(mouseEvent.Button);

                        // Check for click (press and release in close succession)
                        _mouseClickedThisFrame.Add(mouseEvent.Button);
                    }
                    break;

                case MouseAction.Wheel:
                    // For wheel events, X/Y represent wheel delta
                    _mouseWheelDelta = new Position(mouseEvent.X, mouseEvent.Y);
                    break;

                case MouseAction.Move:
                    // Mouse move is already handled by position update above
                    break;
            }
        }
    }

    private static void InitializeTerminal()
    {
        Console.Write(TerminalInit.Initialize());
    }

    public bool IsDown(InputKey key)
    {
        // Check if key is down with any modifiers
        return _currentlyDown.Any(tuple => tuple.key.Equals(key));
    }

    public bool WasPressed(InputKey key)
    {
        // Check if key was pressed this frame with any modifiers
        return _pressedThisFrame.Any(tuple => tuple.key.Equals(key));
    }

    public bool WasReleased(InputKey key)
    {
        // Check if key was released this frame with any modifiers
        return _releasedThisFrame.Any(tuple => tuple.key.Equals(key));
    }

    public bool IsDown(InputKey key, InputKeyModifierType modifiers)
    {
        return _currentlyDown.Contains((key, modifiers));
    }

    public bool WasPressed(InputKey key, InputKeyModifierType modifiers)
    {
        return _pressedThisFrame.Contains((key, modifiers));
    }

    public bool WasReleased(InputKey key, InputKeyModifierType modifiers)
    {
        return _releasedThisFrame.Contains((key, modifiers));
    }

    public bool IsRepeating(InputKey key)
    {
        return _repeatedThisFrame.Any(tuple => tuple.key.Equals(key));
    }

    public bool IsRepeating(InputKey key, InputKeyModifierType modifiers)
    {
        return _repeatedThisFrame.Contains((key, modifiers));
    }

    public InputKeyModifierType GetActiveModifiers()
    {
        return _activeModifiers;
    }

    public Position MousePosition()
    {
        return _currentMousePosition;
    }

    public bool MouseDown(MouseButtonType button)
    {
        return _mouseCurrentlyDown.Contains(button);
    }

    public bool MouseClicked(MouseButtonType button)
    {
        return _mouseClickedThisFrame.Contains(button);
    }

    public Position MouseWheelDelta()
    {
        return _mouseWheelDelta;
    }

    public Position MouseDelta()
    {
        return _mouseDelta;
    }

    public bool MousePressed(MouseButtonType button)
    {
        return _mousePressedThisFrame.Contains(button);
    }

    public bool MouseReleased(MouseButtonType button)
    {
        return _mouseReleasedThisFrame.Contains(button);
    }

    public bool IsPointerInside()
    {
        // For terminal applications, we assume the pointer is always "inside"
        // as long as the terminal window has focus
        return HasFocus();
    }

    public bool HasFocus()
    {
        // In a terminal environment, we assume focus is always true
        // This could be enhanced with terminal focus detection sequences
        return true;
    }

    private void ProcessKeyRepeats()
    {
        var currentTime = DateTime.UtcNow;
        _keysToRepeat.Clear(); // Reuse existing list to avoid allocations

        foreach (var keyTuple in _currentlyDown)
        {
            if (!_keyPressStartTimes.TryGetValue(keyTuple, out var pressTime))
                continue;

            // Check if enough time has passed for initial repeat
            var timeSincePress = currentTime - pressTime;
            if (timeSincePress >= KeyRepeatDelay)
            {
                // Check if it's time for another repeat
                var lastRepeatTime = _lastKeyRepeatTimes.TryGetValue(keyTuple, out var lastTime)
                    ? lastTime
                    : pressTime;

                var timeSinceLastRepeat = currentTime - lastRepeatTime;
                if (timeSinceLastRepeat >= KeyRepeatDelay)
                {
                    _keysToRepeat.Add(keyTuple);
                    _lastKeyRepeatTimes[keyTuple] = currentTime;
                }
            }
        }

        // Add repeated keys to the frame
        foreach (var keyTuple in _keysToRepeat)
        {
            _repeatedThisFrame.Add(keyTuple);
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
        GC.SuppressFinalize(this);
    }
}
