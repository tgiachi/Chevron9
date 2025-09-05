namespace Chevron9.Core.Interfaces;

/// <summary>
///     Interface for managing scene stack with push/pop/replace operations
///     Handles scene lifecycle, updates, rendering and input processing
/// </summary>
public interface ISceneManager
{
    /// <summary>
    ///     Gets the currently active scene at top of stack
    /// </summary>
    IScene? Current { get; }

    /// <summary>
    ///     Pushes a new scene onto the stack, making it active
    ///     Previous scene remains in memory but becomes inactive
    /// </summary>
    /// <param name="scene">Scene to push and activate</param>
    void Push(IScene scene);

    /// <summary>
    ///     Pops the current scene from stack, activating the previous one
    ///     Calls Exit() on the popped scene for cleanup
    /// </summary>
    void Pop();

    /// <summary>
    ///     Replaces current scene with a new one
    ///     Equivalent to Pop() followed by Push(), but more efficient
    /// </summary>
    /// <param name="scene">Scene to replace current with</param>
    void Replace(IScene scene);

    /// <summary>
    ///     Updates the current active scene with fixed timestep
    /// </summary>
    /// <param name="fixedDt">Fixed delta time for consistent game logic</param>
    /// <param name="input">Input device for handling user input</param>
    void Update(double fixedDt, IInputDevice input);

    /// <summary>
    ///     Renders the current scene to the render command collector
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering between updates</param>
    void Render(IRenderCommandCollector rq, float alpha);

    /// <summary>
    ///     Processes input through the current scene
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed by the scene</returns>
    bool HandleInput(IInputDevice input);
}
