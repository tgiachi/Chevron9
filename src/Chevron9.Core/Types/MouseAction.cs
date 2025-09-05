namespace Chevron9.Core.Types;

/// <summary>
/// Represents different types of mouse actions/events
/// </summary>
public enum MouseAction : byte
{
    /// <summary>Mouse button pressed down</summary>
    Down = 0,
    /// <summary>Mouse button released</summary>
    Up = 1,
    /// <summary>Mouse cursor moved</summary>
    Move = 2,
    /// <summary>Mouse wheel scrolled</summary>
    Wheel = 3,
}
