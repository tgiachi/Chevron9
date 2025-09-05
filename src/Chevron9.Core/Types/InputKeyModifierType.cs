namespace Chevron9.Core.Types;

/// <summary>
/// Bitfield enum for keyboard modifier keys (Ctrl, Shift, Alt, etc.)
/// Can be combined using bitwise OR operations
/// </summary>
[Flags]
public enum InputKeyModifierType
{
    /// <summary>No modifiers pressed</summary>
    None = 0,
    /// <summary>Shift key modifier</summary>
    Shift = 1 << 0,
    /// <summary>Control key modifier</summary>
    Control = 1 << 1,
    /// <summary>Alt key modifier</summary>
    Alt = 1 << 2,
    /// <summary>Super/Windows/Cmd key modifier</summary>
    Super = 1 << 3,
}
