using Chevron9.Core.Render;
using Chevron9.Shared.Graphics;

namespace Chevron9.Core.Interfaces;

/// <summary>
/// Interface for rendering system using command pattern
/// Supports different backends (Terminal, MonoGame, OpenGL, etc.) with unified API
/// </summary>
public interface IRenderer : IDisposable
{
    /// <summary>
    /// Gets the render target width in backend-specific units (cells for Terminal, pixels for GPU)
    /// </summary>
    int Width { get; }
    
    /// <summary>
    /// Gets the render target height in backend-specific units (cells for Terminal, pixels for GPU)
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Sets the rendering viewport (clipping region)
    /// </summary>
    /// <param name="x">Viewport X offset</param>
    /// <param name="y">Viewport Y offset</param>
    /// <param name="width">Viewport width</param>
    /// <param name="height">Viewport height</param>
    void SetViewport(int x, int y, int width, int height);
    
    /// <summary>
    /// Clears the render target with specified color
    /// </summary>
    /// <param name="color">Clear color</param>
    void Clear(Color color);

    /// <summary>
    /// Begins frame rendering - resets state and prepares for new frame
    /// </summary>
    void BeginFrame();
    
    /// <summary>
    /// Begins batch rendering for performance optimization
    /// Commands submitted between BeginBatch/EndBatch may be batched together
    /// </summary>
    void BeginBatch();
    
    /// <summary>
    /// Submits a render command to the rendering pipeline
    /// </summary>
    /// <param name="cmd">Command to execute</param>
    void Submit(RenderCommand cmd);
    
    /// <summary>
    /// Ends batch rendering and flushes batched commands
    /// </summary>
    void EndBatch();
    
    /// <summary>
    /// Ends frame rendering - flushes commands and presents final image
    /// </summary>
    void EndFrame();
}
