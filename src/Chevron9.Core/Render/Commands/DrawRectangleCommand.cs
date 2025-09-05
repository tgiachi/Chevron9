using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Command to draw a filled rectangle
/// </summary>
/// <param name="Bounds">Rectangle boundaries</param>
/// <param name="Color">Fill color</param>
public record DrawRectangleCommand(RectF Bounds, Color Color) : RenderCommand;
