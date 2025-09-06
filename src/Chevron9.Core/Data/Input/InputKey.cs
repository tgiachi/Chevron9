namespace Chevron9.Core.Data.Input;

/// <summary>
///     Represents a keyboard input key with name and virtual key code
///     Immutable value type for consistent key identification
/// </summary>
public readonly struct InputKey(string Name, int Code) : IEquatable<InputKey>
{
    /// <summary>
    ///     Gets the human-readable name of the key
    /// </summary>
    public string Name { get; } = Name;

    /// <summary>
    ///     Gets the virtual key code for this key
    /// </summary>
    public int Code { get; } = Code;

    public bool Equals(InputKey other)
    {
        return Code == other.Code && string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is InputKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Code);
    }

    public static bool operator ==(InputKey left, InputKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InputKey left, InputKey right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return $"{Name} ({Code})";
    }
}
