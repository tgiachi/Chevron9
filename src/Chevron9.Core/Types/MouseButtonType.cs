namespace Chevron9.Core.Types;

/// <summary>
/// Represents different mouse button types
/// </summary>
public enum MouseButtonType : byte
{
    /// <summary>Left mouse button (primary)</summary>
    Left = 0,
    /// <summary>Right mouse button (secondary/context)</summary>
    Right = 1,
    /// <summary>Middle mouse button (wheel click)</summary>
    Middle = 2,
    /// <summary>Extra button 1 (back/previous)</summary>
    XButton1 = 3,
    /// <summary>Extra button 2 (forward/next)</summary>
    XButton2 = 4,
}
