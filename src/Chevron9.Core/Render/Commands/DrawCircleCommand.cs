using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Command to draw a filled circle
/// </summary>
/// <param name="Center">Circle center position</param>
/// <param name="Radius">Circle radius</param>
/// <param name="Color">Fill color</param>
public record DrawCircleCommand(Position Center, float Radius, Color Color) : RenderCommand;
