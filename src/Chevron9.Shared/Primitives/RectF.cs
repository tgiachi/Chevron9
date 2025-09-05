using Chevron9.Shared.Graphics;

namespace Chevron9.Shared.Primitives;

/// <summary>
/// Represents a rectangle with floating-point coordinates and dimensions.
/// </summary>
public readonly struct RectF : IEquatable<RectF>
{
    /// <summary>
    /// The X coordinate of the top-left corner of the rectangle.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// The Y coordinate of the top-left corner of the rectangle.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Initializes a new instance of the RectangleF struct.
    /// </summary>
    /// <param name="x">The X coordinate of the top-left corner.</param>
    /// <param name="y">The Y coordinate of the top-left corner.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public RectF(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Initializes a new instance of the RectangleF struct from position and size.
    /// </summary>
    /// <param name="position">The position of the top-left corner.</param>
    /// <param name="size">The size of the rectangle.</param>
    public RectF(Position position, (float Width, float Height) size)
        : this(position.X, position.Y, size.Width, size.Height)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RectangleF struct from position and size.
    /// </summary>
    /// <param name="position">The position of the top-left corner.</param>
    /// <param name="size">The size of the rectangle.</param>
    public RectF((float X, float Y) position, (float Width, float Height) size)
        : this(position.X, position.Y, size.Width, size.Height)
    {
    }

    /// <summary>
    /// Gets an empty rectangle (0, 0, 0, 0).
    /// </summary>
    public static RectF Empty => new(0f, 0f, 0f, 0f);

    /// <summary>
    /// Gets the X coordinate of the left edge of the rectangle.
    /// </summary>
    public float Left => X;

    /// <summary>
    /// Gets the X coordinate of the right edge of the rectangle.
    /// </summary>
    public float Right => X + Width;

    /// <summary>
    /// Gets the Y coordinate of the top edge of the rectangle.
    /// </summary>
    public float Top => Y;

    /// <summary>
    /// Gets the Y coordinate of the bottom edge of the rectangle.
    /// </summary>
    public float Bottom => Y + Height;

    /// <summary>
    /// Gets the position of the top-left corner as a Position.
    /// </summary>
    public Position Position => new(X, Y);

    /// <summary>
    /// Gets the size of the rectangle.
    /// </summary>
    public (float Width, float Height) Size => (Width, Height);

    /// <summary>
    /// Gets the position of the center of the rectangle.
    /// </summary>
    public Position Center => new(X + Width / 2f, Y + Height / 2f);

    /// <summary>
    /// Gets the area of the rectangle.
    /// </summary>
    public float Area => Width * Height;

    /// <summary>
    /// Gets a value indicating whether the rectangle is empty (width or height is zero or negative).
    /// </summary>
    public bool IsEmpty => Width <= 0f || Height <= 0f;

    /// <summary>
    /// Determines whether the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="x">The X coordinate of the point.</param>
    /// <param name="y">The Y coordinate of the point.</param>
    /// <returns>True if the point is contained within this rectangle; otherwise, false.</returns>
    public bool Contains(float x, float y)
    {
        return x >= X && x < X + Width && y >= Y && y < Y + Height;
    }

    /// <summary>
    /// Determines whether the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is contained within this rectangle; otherwise, false.</returns>
    public bool Contains(Position point)
    {
        return Contains(point.X, point.Y);
    }

    /// <summary>
    /// Determines whether the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is contained within this rectangle; otherwise, false.</returns>
    public bool Contains((float X, float Y) point)
    {
        return Contains(point.X, point.Y);
    }

    /// <summary>
    /// Determines whether the specified rectangle is entirely contained within this rectangle.
    /// </summary>
    /// <param name="other">The rectangle to test.</param>
    /// <returns>True if the rectangle is entirely contained within this rectangle; otherwise, false.</returns>
    public bool Contains(RectF other)
    {
        return other.X >= X && other.Y >= Y &&
               other.X + other.Width <= X + Width &&
               other.Y + other.Height <= Y + Height;
    }

    /// <summary>
    /// Determines whether this rectangle intersects with another rectangle.
    /// </summary>
    /// <param name="other">The rectangle to test for intersection.</param>
    /// <returns>True if the rectangles intersect; otherwise, false.</returns>
    public bool Intersects(RectF other)
    {
        return other.X < X + Width && X < other.X + other.Width &&
               other.Y < Y + Height && Y < other.Y + other.Height;
    }

    /// <summary>
    /// Determines whether this rectangle intersects with an integer rectangle.
    /// </summary>
    /// <param name="other">The rectangle to test for intersection.</param>
    /// <returns>True if the rectangles intersect; otherwise, false.</returns>
    public bool Intersects(RectI other)
    {
        return other.X < X + Width && X < other.X + other.Width &&
               other.Y < Y + Height && Y < other.Y + other.Height;
    }

    /// <summary>
    /// Returns the intersection of this rectangle with another rectangle.
    /// </summary>
    /// <param name="other">The rectangle to intersect with.</param>
    /// <returns>The intersection rectangle, or Empty if no intersection exists.</returns>
    public RectF Intersect(RectF other)
    {
        var left = MathF.Max(X, other.X);
        var top = MathF.Max(Y, other.Y);
        var right = MathF.Min(X + Width, other.X + other.Width);
        var bottom = MathF.Min(Y + Height, other.Y + other.Height);

        if (right <= left || bottom <= top)
            return Empty;

        return new RectF(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Returns the union of this rectangle with another rectangle.
    /// </summary>
    /// <param name="other">The rectangle to union with.</param>
    /// <returns>The union rectangle.</returns>
    public RectF Union(RectF other)
    {
        if (IsEmpty) return other;
        if (other.IsEmpty) return this;

        var left = MathF.Min(X, other.X);
        var top = MathF.Min(Y, other.Y);
        var right = MathF.Max(X + Width, other.X + other.Width);
        var bottom = MathF.Max(Y + Height, other.Y + other.Height);

        return new RectF(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Returns a new rectangle with the specified position.
    /// </summary>
    /// <param name="x">The new X coordinate.</param>
    /// <param name="y">The new Y coordinate.</param>
    /// <returns>A new rectangle with the specified position.</returns>
    public RectF WithPosition(float x, float y)
    {
        return new RectF(x, y, Width, Height);
    }

    /// <summary>
    /// Returns a new rectangle with the specified position.
    /// </summary>
    /// <param name="position">The new position.</param>
    /// <returns>A new rectangle with the specified position.</returns>
    public RectF WithPosition(Position position)
    {
        return new RectF(position.X, position.Y, Width, Height);
    }

    /// <summary>
    /// Returns a new rectangle with the specified size.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    /// <returns>A new rectangle with the specified size.</returns>
    public RectF WithSize(float width, float height)
    {
        return new RectF(X, Y, width, height);
    }

    /// <summary>
    /// Returns a new rectangle inflated by the specified amounts.
    /// </summary>
    /// <param name="horizontal">The amount to inflate horizontally (added to both sides).</param>
    /// <param name="vertical">The amount to inflate vertically (added to both sides).</param>
    /// <returns>The inflated rectangle.</returns>
    public RectF Inflate(float horizontal, float vertical)
    {
        return new RectF(X - horizontal, Y - vertical, Width + 2 * horizontal, Height + 2 * vertical);
    }

    /// <summary>
    /// Returns a new rectangle offset by the specified amounts.
    /// </summary>
    /// <param name="offsetX">The amount to offset horizontally.</param>
    /// <param name="offsetY">The amount to offset vertically.</param>
    /// <returns>The offset rectangle.</returns>
    public RectF Offset(float offsetX, float offsetY)
    {
        return new RectF(X + offsetX, Y + offsetY, Width, Height);
    }

    /// <summary>
    /// Returns a new rectangle offset by the specified position.
    /// </summary>
    /// <param name="offset">The offset to apply.</param>
    /// <returns>The offset rectangle.</returns>
    public RectF Offset(Position offset)
    {
        return new RectF(X + offset.X, Y + offset.Y, Width, Height);
    }

    /// <summary>
    /// Linearly interpolates between this rectangle and another rectangle.
    /// </summary>
    /// <param name="other">The target rectangle.</param>
    /// <param name="t">The interpolation factor (0.0 to 1.0).</param>
    /// <returns>The interpolated rectangle.</returns>
    public RectF Lerp(RectF other, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new RectF(
            X + (other.X - X) * t,
            Y + (other.Y - Y) * t,
            Width + (other.Width - Width) * t,
            Height + (other.Height - Height) * t
        );
    }

    /// <summary>
    /// Determines whether the specified RectangleF is equal to the current RectangleF.
    /// </summary>
    /// <param name="other">The RectangleF to compare with the current RectangleF.</param>
    /// <returns>true if the specified RectangleF is equal to the current RectangleF; otherwise, false.</returns>
    public bool Equals(RectF other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
    }

    /// <summary>
    /// Determines whether the specified RectangleF is approximately equal to the current RectangleF.
    /// </summary>
    /// <param name="other">The RectangleF to compare with the current RectangleF.</param>
    /// <param name="tolerance">The tolerance for comparison (default: 1e-6f).</param>
    /// <returns>true if the rectangles are approximately equal; otherwise, false.</returns>
    public bool Approximately(RectF other, float tolerance = 1e-6f)
    {
        return MathF.Abs(X - other.X) <= tolerance &&
               MathF.Abs(Y - other.Y) <= tolerance &&
               MathF.Abs(Width - other.Width) <= tolerance &&
               MathF.Abs(Height - other.Height) <= tolerance;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current RectangleF.
    /// </summary>
    /// <param name="obj">The object to compare with the current RectangleF.</param>
    /// <returns>true if the specified object is equal to the current RectangleF; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is RectF other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this RectangleF.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    /// <summary>
    /// Returns a string representation of this RectangleF.
    /// </summary>
    /// <returns>A string that represents this RectangleF.</returns>
    public override string ToString()
    {
        return $"RectangleF(X:{X:F2}, Y:{Y:F2}, W:{Width:F2}, H:{Height:F2})";
    }

    /// <summary>
    /// Returns a string representation of this RectangleF with the specified format.
    /// </summary>
    /// <param name="format">The format string to use for the coordinates and dimensions.</param>
    /// <returns>A string that represents this RectangleF with the specified format.</returns>
    public string ToString(string format)
    {
        return $"RectangleF(X:{X.ToString(format)}, Y:{Y.ToString(format)}, W:{Width.ToString(format)}, H:{Height.ToString(format)})";
    }

    /// <summary>
    /// Determines whether two RectangleF instances are equal.
    /// </summary>
    /// <param name="left">The first RectangleF to compare.</param>
    /// <param name="right">The second RectangleF to compare.</param>
    /// <returns>true if the RectangleF instances are equal; otherwise, false.</returns>
    public static bool operator ==(RectF left, RectF right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two RectangleF instances are not equal.
    /// </summary>
    /// <param name="left">The first RectangleF to compare.</param>
    /// <param name="right">The second RectangleF to compare.</param>
    /// <returns>true if the RectangleF instances are not equal; otherwise, false.</returns>
    public static bool operator !=(RectF left, RectF right)
    {
        return !left.Equals(right);
    }
}
