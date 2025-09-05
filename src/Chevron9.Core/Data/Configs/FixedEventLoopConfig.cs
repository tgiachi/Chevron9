namespace Chevron9.Core.Data.Configs;

/// <summary>
///     Configuration for FixedEventLoop.
/// </summary>
/// <param name="FramesPerSecond">Target logical updates per second (e.g. 60).</param>
/// <param name="MaxFrameTime">Maximum delta per frame in seconds (clamp to avoid spiral of death).</param>
public readonly record struct FixedEventLoopConfig(
    int FramesPerSecond = 60,
    double MaxFrameTime = 0.25
)
{
    /// <summary>
    ///     Step duration in seconds (1 / FPS).
    /// </summary>
    public double FixedStep => 1.0 / FramesPerSecond;

    /// <summary>
    ///     Creates a configuration optimized for terminal rendering at 30 FPS
    ///     Lower framerate reduces CPU usage for text-based applications
    /// </summary>
    /// <returns>Configuration with 30 FPS and appropriate max frame time</returns>
    public static FixedEventLoopConfig Terminal30()
    {
        return new FixedEventLoopConfig(30, 0.1);
    }

    /// <summary>
    ///     Creates the default configuration for smooth 60 FPS gameplay
    ///     Standard configuration for most game applications
    /// </summary>
    /// <returns>Configuration with 60 FPS and spiral-of-death protection</returns>
    public static FixedEventLoopConfig Default60()
    {
        return new FixedEventLoopConfig(60);
    }
}
