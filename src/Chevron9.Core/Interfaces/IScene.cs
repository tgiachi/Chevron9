namespace Chevron9.Core.Interfaces;

/// <summary>
///     Interface for a game scene containing multiple rendering layers
///     Scenes represent different application states (menu, game, settings, etc.)
/// </summary>
public interface IScene : IDisposable
{
    /// <summary>
    ///     Gets the scene name for identification and debugging
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the read-only list of rendering layers in this scene
    ///     Layers are rendered in order with Z-sorting
    /// </summary>
    IReadOnlyList<ILayer> Layers { get; }

    /// <summary>
    ///     Called when scene becomes active (pushed onto scene stack)
    ///     Use for initialization, resource loading, state setup
    /// </summary>
    void Enter();

    /// <summary>
    ///     Called when scene becomes inactive (popped from scene stack)
    ///     Use for cleanup, state saving, resource unloading
    /// </summary>
    void Close();

    /// <summary>
    ///     Updates scene logic with fixed timestep for consistent gameplay
    /// </summary>
    /// <param name="fixedDt">Fixed delta time in seconds</param>
    /// <param name="input">Input device for user interaction</param>
    void Update(double fixedDt, IInputDevice input);

    /// <summary>
    ///     Renders all scene layers to the render command collector
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering between updates</param>
    void Render(IRenderCommandCollector rq, float alpha);

    /// <summary>
    ///     Processes input through scene layers, typically from top to bottom
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed (e.g., by UI layer)</returns>
    bool HandleInput(IInputDevice input);
}
