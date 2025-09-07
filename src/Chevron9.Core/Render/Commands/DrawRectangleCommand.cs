using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Command to draw a filled rectangle
/// </summary>
/// <param name="Bounds">Rectangle boundaries in world coordinates</param>
/// <param name="Color">Fill color with alpha support</param>
public record DrawRectangleCommand(RectF Bounds, Color Color) : RenderCommand
{
    /// <summary>
    ///     Validates that the rectangle bounds are valid
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when bounds have negative width or height</exception>
    public virtual bool IsValid =>
        Bounds.Width >= 0 && Bounds.Height >= 0;
};
