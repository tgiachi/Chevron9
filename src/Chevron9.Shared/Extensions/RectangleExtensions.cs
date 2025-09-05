using Chevron9.Shared.Primitives;

namespace Chevron9.Shared.Extensions;

/// <summary>
///     Provides extension methods for rectangle conversions and utilities.
/// </summary>
public static class RectangleExtensions
{
    /// <summary>
    ///     Converts a RectangleI to a RectangleF.
    /// </summary>
    /// <param name="rect">The RectangleI to convert.</param>
    /// <returns>A RectangleF with the same coordinates and dimensions.</returns>
    public static RectF ToRectF(this RectI rect)
    {
        return new RectF(rect.X, rect.Y, rect.Width, rect.Height);
    }

    /// <summary>
    ///     Converts a RectangleF to a RectangleI by rounding the coordinates and dimensions.
    /// </summary>
    /// <param name="rect">The RectangleF to convert.</param>
    /// <returns>A RectangleI with rounded coordinates and dimensions.</returns>
    public static RectI ToRectI(this RectF rect)
    {
        return new RectI(
            (int)MathF.Round(rect.X),
            (int)MathF.Round(rect.Y),
            (int)MathF.Round(rect.Width),
            (int)MathF.Round(rect.Height)
        );
    }

    /// <summary>
    ///     Converts a RectangleF to a RectangleI by flooring the coordinates and dimensions.
    /// </summary>
    /// <param name="rect">The RectangleF to convert.</param>
    /// <returns>A RectangleI with floored coordinates and dimensions.</returns>
    public static RectI ToRectIFloor(this RectF rect)
    {
        return new RectI(
            (int)MathF.Floor(rect.X),
            (int)MathF.Floor(rect.Y),
            (int)MathF.Floor(rect.Width),
            (int)MathF.Floor(rect.Height)
        );
    }

    /// <summary>
    ///     Converts a RectangleF to a RectangleI by ceiling the coordinates and dimensions.
    /// </summary>
    /// <param name="rect">The RectangleF to convert.</param>
    /// <returns>A RectangleI with ceiling coordinates and dimensions.</returns>
    public static RectI ToRectICeiling(this RectF rect)
    {
        return new RectI(
            (int)MathF.Ceiling(rect.X),
            (int)MathF.Ceiling(rect.Y),
            (int)MathF.Ceiling(rect.Width),
            (int)MathF.Ceiling(rect.Height)
        );
    }

    /// <summary>
    ///     Converts a RectangleF to a RectangleI by truncating the coordinates and dimensions.
    /// </summary>
    /// <param name="rect">The RectangleF to convert.</param>
    /// <returns>A RectangleI with truncated coordinates and dimensions.</returns>
    public static RectI ToRectITruncate(this RectF rect)
    {
        return new RectI(
            (int)MathF.Truncate(rect.X),
            (int)MathF.Truncate(rect.Y),
            (int)MathF.Truncate(rect.Width),
            (int)MathF.Truncate(rect.Height)
        );
    }

    /// <summary>
    ///     Gets the four corner positions of the rectangle.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <returns>A tuple containing the four corner positions (TopLeft, TopRight, BottomLeft, BottomRight).</returns>
    public static (Position TopLeft, Position TopRight, Position BottomLeft, Position BottomRight) GetCorners(
        this RectF rect
    )
    {
        return (
            TopLeft: new Position(rect.Left, rect.Top),
            TopRight: new Position(rect.Right, rect.Top),
            BottomLeft: new Position(rect.Left, rect.Bottom),
            BottomRight: new Position(rect.Right, rect.Bottom)
        );
    }

    /// <summary>
    ///     Gets the four corner positions of the rectangle.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <returns>A tuple containing the four corner positions (TopLeft, TopRight, BottomLeft, BottomRight).</returns>
    public static ((int X, int Y) TopLeft, (int X, int Y) TopRight, (int X, int Y) BottomLeft, (int X, int Y) BottomRight)
        GetCorners(this RectI rect)
    {
        return (
            TopLeft: (rect.Left, rect.Top),
            TopRight: (rect.Right, rect.Top),
            BottomLeft: (rect.Left, rect.Bottom),
            BottomRight: (rect.Right, rect.Bottom)
        );
    }

    /// <summary>
    ///     Scales a rectangle by the specified factor.
    /// </summary>
    /// <param name="rect">The rectangle to scale.</param>
    /// <param name="scale">The scale factor.</param>
    /// <returns>The scaled rectangle.</returns>
    public static RectF Scale(this RectF rect, float scale)
    {
        return new RectF(rect.X * scale, rect.Y * scale, rect.Width * scale, rect.Height * scale);
    }

    /// <summary>
    ///     Scales a rectangle by different factors for X and Y.
    /// </summary>
    /// <param name="rect">The rectangle to scale.</param>
    /// <param name="scaleX">The X scale factor.</param>
    /// <param name="scaleY">The Y scale factor.</param>
    /// <returns>The scaled rectangle.</returns>
    public static RectF Scale(this RectF rect, float scaleX, float scaleY)
    {
        return new RectF(rect.X * scaleX, rect.Y * scaleY, rect.Width * scaleX, rect.Height * scaleY);
    }

    /// <summary>
    ///     Scales a rectangle by the specified Position as scale factors.
    /// </summary>
    /// <param name="rect">The rectangle to scale.</param>
    /// <param name="scale">The scale factors (X and Y).</param>
    /// <returns>The scaled rectangle.</returns>
    public static RectF Scale(this RectF rect, Position scale)
    {
        return new RectF(rect.X * scale.X, rect.Y * scale.Y, rect.Width * scale.X, rect.Height * scale.Y);
    }

    /// <summary>
    ///     Gets a rectangle that represents the aspect-ratio-preserving fit of one rectangle within another.
    /// </summary>
    /// <param name="container">The container rectangle.</param>
    /// <param name="content">The content rectangle to fit.</param>
    /// <returns>The fitted rectangle within the container.</returns>
    public static RectF FitWithin(this RectF container, RectF content)
    {
        if (content.IsEmpty || container.IsEmpty)
        {
            return RectF.Empty;
        }

        var scaleX = container.Width / content.Width;
        var scaleY = container.Height / content.Height;
        var scale = MathF.Min(scaleX, scaleY);

        var scaledWidth = content.Width * scale;
        var scaledHeight = content.Height * scale;

        var x = container.X + (container.Width - scaledWidth) / 2f;
        var y = container.Y + (container.Height - scaledHeight) / 2f;

        return new RectF(x, y, scaledWidth, scaledHeight);
    }

    /// <summary>
    ///     Gets a rectangle that represents the aspect-ratio-preserving fill of one rectangle within another.
    /// </summary>
    /// <param name="container">The container rectangle.</param>
    /// <param name="content">The content rectangle to fill.</param>
    /// <returns>The filled rectangle within the container.</returns>
    public static RectF FillWithin(this RectF container, RectF content)
    {
        if (content.IsEmpty || container.IsEmpty)
        {
            return RectF.Empty;
        }

        var scaleX = container.Width / content.Width;
        var scaleY = container.Height / content.Height;
        var scale = MathF.Max(scaleX, scaleY);

        var scaledWidth = content.Width * scale;
        var scaledHeight = content.Height * scale;

        var x = container.X + (container.Width - scaledWidth) / 2f;
        var y = container.Y + (container.Height - scaledHeight) / 2f;

        return new RectF(x, y, scaledWidth, scaledHeight);
    }

    /// <summary>
    ///     Clamps a rectangle to be entirely within the bounds of another rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to clamp.</param>
    /// <param name="bounds">The bounding rectangle.</param>
    /// <returns>The clamped rectangle.</returns>
    public static RectF ClampToBounds(this RectF rect, RectF bounds)
    {
        var width = Math.Min(rect.Width, bounds.Width);
        var height = Math.Min(rect.Height, bounds.Height);
        var x = Math.Clamp(rect.X, bounds.X, bounds.Right - width);
        var y = Math.Clamp(rect.Y, bounds.Y, bounds.Bottom - height);

        return new RectF(x, y, width, height);
    }

    /// <summary>
    ///     Clamps a rectangle to be entirely within the bounds of another rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to clamp.</param>
    /// <param name="bounds">The bounding rectangle.</param>
    /// <returns>The clamped rectangle.</returns>
    public static RectI ClampToBounds(this RectI rect, RectI bounds)
    {
        var x = Math.Clamp(rect.X, bounds.X, bounds.Right - rect.Width);
        var y = Math.Clamp(rect.Y, bounds.Y, bounds.Bottom - rect.Height);
        var width = Math.Min(rect.Width, bounds.Width);
        var height = Math.Min(rect.Height, bounds.Height);

        return new RectI(x, y, width, height);
    }
}
