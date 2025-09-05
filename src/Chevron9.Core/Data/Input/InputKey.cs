namespace Chevron9.Core.Data.Input;

/// <summary>
///     Represents a keyboard input key with name and virtual key code
///     Immutable value type for consistent key identification
/// </summary>
public readonly struct InputKey(string Name, int Code)
{
    /// <summary>
    ///     Gets the human-readable name of the key
    /// </summary>
    public string Name { get; } = Name;

    /// <summary>
    ///     Gets the virtual key code for this key
    /// </summary>
    public int Code { get; } = Code;
}
