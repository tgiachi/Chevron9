using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Command to draw a line between two points
/// </summary>
/// <param name="Start">Start position</param>
/// <param name="End">End position</param>
/// <param name="Color">Line color</param>
/// <param name="Thickness">Line thickness</param>
public record DrawLineCommand(Position Start, Position End, Color Color, float Thickness = 1.0f) : RenderCommand;
