namespace Chevron9.Shared.Primitives;

/// <summary>
/// Represents a 2D position with X and Y coordinates as floating-point values.
/// </summary>
public readonly struct Position : IEquatable<Position>
{
    /// <summary>
    /// The X coordinate of the position.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// The Y coordinate of the position.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Initializes a new instance of the Position struct with the specified coordinates.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public Position(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets a Position with both coordinates set to zero.
    /// </summary>
    public static Position Zero => new(0f, 0f);

    /// <summary>
    /// Gets a Position with both coordinates set to one.
    /// </summary>
    public static Position One => new(1f, 1f);

    /// <summary>
    /// Gets a Position representing the unit vector in the X direction (1, 0).
    /// </summary>
    public static Position UnitX => new(1f, 0f);

    /// <summary>
    /// Gets a Position representing the unit vector in the Y direction (0, 1).
    /// </summary>
    public static Position UnitY => new(0f, 1f);

    /// <summary>
    /// Calculates the distance between this position and another position.
    /// </summary>
    /// <param name="other">The other position.</param>
    /// <returns>The distance between the two positions.</returns>
    public float DistanceTo(Position other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Calculates the squared distance between this position and another position.
    /// This is more efficient than DistanceTo when you only need to compare distances.
    /// </summary>
    /// <param name="other">The other position.</param>
    /// <returns>The squared distance between the two positions.</returns>
    public float DistanceSquaredTo(Position other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        return dx * dx + dy * dy;
    }

    /// <summary>
    /// Calculates the length (magnitude) of this position as a vector from origin.
    /// </summary>
    /// <returns>The length of the vector.</returns>
    public float Length()
    {
        return MathF.Sqrt(X * X + Y * Y);
    }

    /// <summary>
    /// Calculates the squared length of this position as a vector from origin.
    /// This is more efficient than Length when you only need to compare lengths.
    /// </summary>
    /// <returns>The squared length of the vector.</returns>
    public float LengthSquared()
    {
        return X * X + Y * Y;
    }

    /// <summary>
    /// Returns a normalized version of this position (unit vector in the same direction).
    /// </summary>
    /// <returns>The normalized position.</returns>
    /// <exception cref="InvalidOperationException">Thrown when trying to normalize a zero vector.</exception>
    public Position Normalized()
    {
        var length = Length();
        if (length == 0f)
            throw new InvalidOperationException("Cannot normalize a zero vector.");

        return new Position(X / length, Y / length);
    }

    /// <summary>
    /// Linearly interpolates between this position and another position.
    /// </summary>
    /// <param name="other">The target position.</param>
    /// <param name="t">The interpolation factor (0.0 to 1.0).</param>
    /// <returns>The interpolated position.</returns>
    public Position Lerp(Position other, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Position(
            X + (other.X - X) * t,
            Y + (other.Y - Y) * t
        );
    }

    /// <summary>
    /// Calculates the dot product of this position and another position.
    /// </summary>
    /// <param name="other">The other position.</param>
    /// <returns>The dot product.</returns>
    public float Dot(Position other)
    {
        return X * other.X + Y * other.Y;
    }

    /// <summary>
    /// Calculates the cross product of this position and another position (Z component only).
    /// </summary>
    /// <param name="other">The other position.</param>
    /// <returns>The Z component of the cross product.</returns>
    public float Cross(Position other)
    {
        return X * other.Y - Y * other.X;
    }

    /// <summary>
    /// Returns a position with absolute values of the coordinates.
    /// </summary>
    /// <returns>The position with absolute coordinate values.</returns>
    public Position Abs()
    {
        return new Position(MathF.Abs(X), MathF.Abs(Y));
    }

    /// <summary>
    /// Returns a position with the coordinates rounded to the nearest integers.
    /// </summary>
    /// <returns>The position with rounded coordinates.</returns>
    public Position Round()
    {
        return new Position(MathF.Round(X), MathF.Round(Y));
    }

    /// <summary>
    /// Returns a position with the coordinates rounded down to the nearest integers.
    /// </summary>
    /// <returns>The position with floored coordinates.</returns>
    public Position Floor()
    {
        return new Position(MathF.Floor(X), MathF.Floor(Y));
    }

    /// <summary>
    /// Returns a position with the coordinates rounded up to the nearest integers.
    /// </summary>
    /// <returns>The position with ceiled coordinates.</returns>
    public Position Ceiling()
    {
        return new Position(MathF.Ceiling(X), MathF.Ceiling(Y));
    }

    /// <summary>
    /// Clamps this position within the specified bounds.
    /// </summary>
    /// <param name="min">The minimum position.</param>
    /// <param name="max">The maximum position.</param>
    /// <returns>The clamped position.</returns>
    public Position Clamp(Position min, Position max)
    {
        return new Position(
            Math.Clamp(X, min.X, max.X),
            Math.Clamp(Y, min.Y, max.Y)
        );
    }

    /// <summary>
    /// Determines whether the specified Position is equal to the current Position.
    /// </summary>
    /// <param name="other">The Position to compare with the current Position.</param>
    /// <returns>true if the specified Position is equal to the current Position; otherwise, false.</returns>
    public bool Equals(Position other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    /// <summary>
    /// Determines whether the specified Position is approximately equal to the current Position.
    /// </summary>
    /// <param name="other">The Position to compare with the current Position.</param>
    /// <param name="tolerance">The tolerance for comparison (default: 1e-6f).</param>
    /// <returns>true if the positions are approximately equal; otherwise, false.</returns>
    public bool Approximately(Position other, float tolerance = 1e-6f)
    {
        return MathF.Abs(X - other.X) <= tolerance && MathF.Abs(Y - other.Y) <= tolerance;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current Position.
    /// </summary>
    /// <param name="obj">The object to compare with the current Position.</param>
    /// <returns>true if the specified object is equal to the current Position; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this Position.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    /// <summary>
    /// Returns a string representation of this Position.
    /// </summary>
    /// <returns>A string that represents this Position.</returns>
    public override string ToString()
    {
        return $"Position(X:{X:F2}, Y:{Y:F2})";
    }

    /// <summary>
    /// Returns a string representation of this Position with the specified format.
    /// </summary>
    /// <param name="format">The format string to use for the coordinates.</param>
    /// <returns>A string that represents this Position with the specified format.</returns>
    public string ToString(string format)
    {
        return $"Position(X:{X.ToString(format)}, Y:{Y.ToString(format)})";
    }

    #region Operators

    /// <summary>
    /// Adds two positions.
    /// </summary>
    /// <param name="left">The first position.</param>
    /// <param name="right">The second position.</param>
    /// <returns>The sum of the two positions.</returns>
    public static Position operator +(Position left, Position right)
    {
        return new Position(left.X + right.X, left.Y + right.Y);
    }

    /// <summary>
    /// Subtracts one position from another.
    /// </summary>
    /// <param name="left">The first position.</param>
    /// <param name="right">The second position.</param>
    /// <returns>The difference of the two positions.</returns>
    public static Position operator -(Position left, Position right)
    {
        return new Position(left.X - right.X, left.Y - right.Y);
    }

    /// <summary>
    /// Negates a position.
    /// </summary>
    /// <param name="position">The position to negate.</param>
    /// <returns>The negated position.</returns>
    public static Position operator -(Position position)
    {
        return new Position(-position.X, -position.Y);
    }

    /// <summary>
    /// Multiplies a position by a scalar.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The scaled position.</returns>
    public static Position operator *(Position position, float scalar)
    {
        return new Position(position.X * scalar, position.Y * scalar);
    }

    /// <summary>
    /// Multiplies a position by a scalar.
    /// </summary>
    /// <param name="scalar">The scalar value.</param>
    /// <param name="position">The position.</param>
    /// <returns>The scaled position.</returns>
    public static Position operator *(float scalar, Position position)
    {
        return new Position(position.X * scalar, position.Y * scalar);
    }

    /// <summary>
    /// Multiplies two positions component-wise.
    /// </summary>
    /// <param name="left">The first position.</param>
    /// <param name="right">The second position.</param>
    /// <returns>The component-wise product.</returns>
    public static Position operator *(Position left, Position right)
    {
        return new Position(left.X * right.X, left.Y * right.Y);
    }

    /// <summary>
    /// Divides a position by a scalar.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The scaled position.</returns>
    /// <exception cref="DivideByZeroException">Thrown when scalar is zero.</exception>
    public static Position operator /(Position position, float scalar)
    {
        if (scalar == 0f)
            throw new DivideByZeroException("Cannot divide by zero.");

        return new Position(position.X / scalar, position.Y / scalar);
    }

    /// <summary>
    /// Divides two positions component-wise.
    /// </summary>
    /// <param name="left">The first position.</param>
    /// <param name="right">The second position.</param>
    /// <returns>The component-wise quotient.</returns>
    /// <exception cref="DivideByZeroException">Thrown when any component of right is zero.</exception>
    public static Position operator /(Position left, Position right)
    {
        if (right.X == 0f || right.Y == 0f)
            throw new DivideByZeroException("Cannot divide by zero.");

        return new Position(left.X / right.X, left.Y / right.Y);
    }

    /// <summary>
    /// Determines whether two Position instances are equal.
    /// </summary>
    /// <param name="left">The first Position to compare.</param>
    /// <param name="right">The second Position to compare.</param>
    /// <returns>true if the Positions are equal; otherwise, false.</returns>
    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two Position instances are not equal.
    /// </summary>
    /// <param name="left">The first Position to compare.</param>
    /// <param name="right">The second Position to compare.</param>
    /// <returns>true if the Positions are not equal; otherwise, false.</returns>
    public static bool operator !=(Position left, Position right)
    {
        return !left.Equals(right);
    }

    #endregion
}
