using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render;

/// <summary>
/// Abstract base class for all rendering commands
/// Uses record syntax for immutability and value semantics
/// </summary>
public abstract record RenderCommand;

/// <summary>
/// Command to draw a filled rectangle
/// </summary>
/// <param name="Bounds">Rectangle boundaries</param>
/// <param name="Color">Fill color</param>
public record DrawRectangleCommand(RectF Bounds, Color Color) : RenderCommand;

/// <summary>
/// Command to draw rectangle outline
/// </summary>
/// <param name="Bounds">Rectangle boundaries</param>
/// <param name="Color">Outline color</param>
/// <param name="Thickness">Line thickness</param>
public record DrawRectangleOutlineCommand(RectF Bounds, Color Color, float Thickness = 1.0f) : RenderCommand;

/// <summary>
/// Command to draw a line between two points
/// </summary>
/// <param name="Start">Start position</param>
/// <param name="End">End position</param>
/// <param name="Color">Line color</param>
/// <param name="Thickness">Line thickness</param>
public record DrawLineCommand(Position Start, Position End, Color Color, float Thickness = 1.0f) : RenderCommand;

/// <summary>
/// Command to draw text at specified position
/// </summary>
/// <param name="Text">Text content</param>
/// <param name="Position">Text position</param>
/// <param name="Color">Text color</param>
/// <param name="FontSize">Font size (backend-specific units)</param>
public record DrawTextCommand(string Text, Position Position, Color Color, float FontSize = 12.0f) : RenderCommand;

/// <summary>
/// Command to draw a filled circle
/// </summary>
/// <param name="Center">Circle center position</param>
/// <param name="Radius">Circle radius</param>
/// <param name="Color">Fill color</param>
public record DrawCircleCommand(Position Center, float Radius, Color Color) : RenderCommand;

/// <summary>
/// Command to draw circle outline
/// </summary>
/// <param name="Center">Circle center position</param>
/// <param name="Radius">Circle radius</param>
/// <param name="Color">Outline color</param>
/// <param name="Thickness">Line thickness</param>
public record DrawCircleOutlineCommand(Position Center, float Radius, Color Color, float Thickness = 1.0f) : RenderCommand;
