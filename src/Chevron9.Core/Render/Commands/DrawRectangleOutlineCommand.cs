using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Command to draw rectangle outline
/// </summary>
/// <param name="Bounds">Rectangle boundaries</param>
/// <param name="Color">Outline color</param>
/// <param name="Thickness">Line thickness</param>
public record DrawRectangleOutlineCommand(RectF Bounds, Color Color, float Thickness = 1.0f) : RenderCommand;
