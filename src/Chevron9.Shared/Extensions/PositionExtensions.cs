using System.Numerics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Shared.Extensions;

/// <summary>
///     Provides extension methods for the Position struct.
/// </summary>
public static class PositionExtensions
{
    /// <summary>
    ///     Converts a Position to cell coordinates (Col, Row) by rounding the X and Y values.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static (int Col, int Row) ToCell(this Position pos)
    {
        return ((int)MathF.Round(pos.X), (int)MathF.Round(pos.Y));
    }

    /// <summary>
    ///     Converts a Position to a Vector2.
    /// </summary>
    /// <param name="pos">The Position to convert.</param>
    /// <returns>A Vector2 with the same X and Y values.</returns>
    public static Vector2 ToVector2(this Position pos)
    {
        return new Vector2(pos.X, pos.Y);
    }

    /// <summary>
    ///     Converts a Vector2 to a Position.
    /// </summary>
    /// <param name="vector">The Vector2 to convert.</param>
    /// <returns>A Position with the same X and Y values.</returns>
    public static Position ToPosition(this Vector2 vector)
    {
        return new Position(vector.X, vector.Y);
    }
}
