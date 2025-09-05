namespace Chevron9.Shared.Primitives;

/// <summary>
/// Represents a rectangle with integer coordinates and dimensions.
/// </summary>
public readonly struct RectI : IEquatable<RectI>
{
    /// <summary>
    /// The X coordinate of the top-left corner of the rectangle.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// The Y coordinate of the top-left corner of the rectangle.
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Initializes a new instance of the RectangleI struct.
    /// </summary>
    /// <param name="x">The X coordinate of the top-left corner.</param>
    /// <param name="y">The Y coordinate of the top-left corner.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public RectI(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Initializes a new instance of the RectangleI struct from position and size.
    /// </summary>
    /// <param name="position">The position of the top-left corner.</param>
    /// <param name="size">The size of the rectangle.</param>
    public RectI((int X, int Y) position, (int Width, int Height) size)
        : this(position.X, position.Y, size.Width, size.Height)
    {
    }

    /// <summary>
    /// Gets an empty rectangle (0, 0, 0, 0).
    /// </summary>
    public static RectI Empty => new(0, 0, 0, 0);

    /// <summary>
    /// Gets the X coordinate of the left edge of the rectangle.
    /// </summary>
    public int Left => X;

    /// <summary>
    /// Gets the X coordinate of the right edge of the rectangle.
    /// </summary>
    public int Right => X + Width;

    /// <summary>
    /// Gets the Y coordinate of the top edge of the rectangle.
    /// </summary>
    public int Top => Y;

    /// <summary>
    /// Gets the Y coordinate of the bottom edge of the rectangle.
    /// </summary>
    public int Bottom => Y + Height;

    /// <summary>
    /// Gets the position of the top-left corner.
    /// </summary>
    public (int X, int Y) Position => (X, Y);

    /// <summary>
    /// Gets the size of the rectangle.
    /// </summary>
    public (int Width, int Height) Size => (Width, Height);

    /// <summary>
    /// Gets the position of the center of the rectangle.
    /// </summary>
    public (float X, float Y) Center => (X + Width / 2f, Y + Height / 2f);

    /// <summary>
    /// Gets the area of the rectangle.
    /// </summary>
    public int Area => Width * Height;

    /// <summary>
    /// Gets a value indicating whether the rectangle is empty (width or height is zero or negative).
    /// </summary>
    public bool IsEmpty => Width <= 0 || Height <= 0;

    /// <summary>
    /// Determines whether the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="x">The X coordinate of the point.</param>
    /// <param name="y">The Y coordinate of the point.</param>
    /// <returns>True if the point is contained within this rectangle; otherwise, false.</returns>
    public bool Contains(int x, int y)
    {
        return x >= X && x < X + Width && y >= Y && y < Y + Height;
    }

    /// <summary>
    /// Determines whether the specified point is contained within this rectangle.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is contained within this rectangle; otherwise, false.</returns>
    public bool Contains((int X, int Y) point)
    {
        return Contains(point.X, point.Y);
    }

    /// <summary>
    /// Determines whether the specified rectangle is entirely contained within this rectangle.
    /// </summary>
    /// <param name="other">The rectangle to test.</param>
    /// <returns>True if the rectangle is entirely contained within this rectangle; otherwise, false.</returns>
    public bool Contains(RectI other)
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
    public RectI Intersect(RectI other)
    {
        var left = Math.Max(X, other.X);
        var top = Math.Max(Y, other.Y);
        var right = Math.Min(X + Width, other.X + other.Width);
        var bottom = Math.Min(Y + Height, other.Y + other.Height);

        if (right <= left || bottom <= top)
            return Empty;

        return new RectI(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Returns the union of this rectangle with another rectangle.
    /// </summary>
    /// <param name="other">The rectangle to union with.</param>
    /// <returns>The union rectangle.</returns>
    public RectI Union(RectI other)
    {
        if (IsEmpty) return other;
        if (other.IsEmpty) return this;

        var left = Math.Min(X, other.X);
        var top = Math.Min(Y, other.Y);
        var right = Math.Max(X + Width, other.X + other.Width);
        var bottom = Math.Max(Y + Height, other.Y + other.Height);

        return new RectI(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Returns a new rectangle with the specified position.
    /// </summary>
    /// <param name="x">The new X coordinate.</param>
    /// <param name="y">The new Y coordinate.</param>
    /// <returns>A new rectangle with the specified position.</returns>
    public RectI WithPosition(int x, int y)
    {
        return new RectI(x, y, Width, Height);
    }

    /// <summary>
    /// Returns a new rectangle with the specified size.
    /// </summary>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    /// <returns>A new rectangle with the specified size.</returns>
    public RectI WithSize(int width, int height)
    {
        return new RectI(X, Y, width, height);
    }

    /// <summary>
    /// Returns a new rectangle inflated by the specified amounts.
    /// </summary>
    /// <param name="horizontal">The amount to inflate horizontally (added to both sides).</param>
    /// <param name="vertical">The amount to inflate vertically (added to both sides).</param>
    /// <returns>The inflated rectangle.</returns>
    public RectI Inflate(int horizontal, int vertical)
    {
        return new RectI(X - horizontal, Y - vertical, Width + 2 * horizontal, Height + 2 * vertical);
    }

    /// <summary>
    /// Returns a new rectangle offset by the specified amounts.
    /// </summary>
    /// <param name="offsetX">The amount to offset horizontally.</param>
    /// <param name="offsetY">The amount to offset vertically.</param>
    /// <returns>The offset rectangle.</returns>
    public RectI Offset(int offsetX, int offsetY)
    {
        return new RectI(X + offsetX, Y + offsetY, Width, Height);
    }

    /// <summary>
    /// Determines whether the specified RectangleI is equal to the current RectangleI.
    /// </summary>
    /// <param name="other">The RectangleI to compare with the current RectangleI.</param>
    /// <returns>true if the specified RectangleI is equal to the current RectangleI; otherwise, false.</returns>
    public bool Equals(RectI other)
    {
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current RectangleI.
    /// </summary>
    /// <param name="obj">The object to compare with the current RectangleI.</param>
    /// <returns>true if the specified object is equal to the current RectangleI; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is RectI other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this RectangleI.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    /// <summary>
    /// Returns a string representation of this RectangleI.
    /// </summary>
    /// <returns>A string that represents this RectangleI.</returns>
    public override string ToString()
    {
        return $"RectangleI(X:{X}, Y:{Y}, W:{Width}, H:{Height})";
    }

    /// <summary>
    /// Determines whether two RectangleI instances are equal.
    /// </summary>
    /// <param name="left">The first RectangleI to compare.</param>
    /// <param name="right">The second RectangleI to compare.</param>
    /// <returns>true if the RectangleI instances are equal; otherwise, false.</returns>
    public static bool operator ==(RectI left, RectI right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two RectangleI instances are not equal.
    /// </summary>
    /// <param name="left">The first RectangleI to compare.</param>
    /// <param name="right">The second RectangleI to compare.</param>
    /// <returns>true if the RectangleI instances are not equal; otherwise, false.</returns>
    public static bool operator !=(RectI left, RectI right)
    {
        return !left.Equals(right);
    }
}
