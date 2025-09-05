namespace Chevron9.Core.Args;

/// <summary>
///     Event arguments for renderer resize events
/// </summary>
public class ResizeEventArgs : EventArgs
{
    /// <summary>
    ///     Gets the new width of the renderer
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     Gets the new height of the renderer
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     Initializes a new instance of the ResizeEventArgs class
    /// </summary>
    /// <param name="width">The new width</param>
    /// <param name="height">The new height</param>
    public ResizeEventArgs(int width, int height)
    {
        Width = width;
        Height = height;
    }
}
