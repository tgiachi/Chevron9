using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Command to draw circle outline
/// </summary>
/// <param name="Center">Circle center position</param>
/// <param name="Radius">Circle radius</param>
/// <param name="Color">Outline color</param>
/// <param name="Thickness">Line thickness</param>
public record DrawCircleOutlineCommand(Position Center, float Radius, Color Color, float Thickness = 1.0f) : RenderCommand;
