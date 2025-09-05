using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Render.Commands;

/// <summary>
///     Command to draw text at specified position
/// </summary>
/// <param name="Text">Text content</param>
/// <param name="Position">Text position</param>
/// <param name="Color">Text color</param>
/// <param name="FontSize">Font size (backend-specific units)</param>
public record DrawTextCommand(string Text, Position Position, Color Color, float FontSize = 12.0f) : RenderCommand;
