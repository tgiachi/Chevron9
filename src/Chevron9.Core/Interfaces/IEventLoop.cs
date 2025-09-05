namespace Chevron9.Core.Interfaces;

/// <summary>
/// Interface for game/application event loop with timing information
/// Provides frame-based timing for consistent updates and rendering
/// </summary>
public interface IEventLoop
{
    /// <summary>
    /// Gets the total elapsed time since the event loop started
    /// </summary>
    /// <value>Total elapsed time in seconds</value>
    double Total { get; }
    
    /// <summary>
    /// Gets the time elapsed since the last frame/tick
    /// </summary>
    /// <value>Delta time in seconds for frame-rate independent calculations</value>
    double Delta { get; }
    
    /// <summary>
    /// Advances the event loop by one frame, updating timing information
    /// Call once per frame to maintain accurate timing
    /// </summary>
    void Tick();
}
