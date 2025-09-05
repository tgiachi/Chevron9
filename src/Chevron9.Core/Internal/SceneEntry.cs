using Chevron9.Core.Interfaces;

namespace Chevron9.Core.Internal;

/// <summary>
///     Internal entry structure combining a scene with its optional DI scope
///     Used by SceneManager to manage scene lifecycle and resource cleanup
/// </summary>
internal readonly struct SceneEntry
{
    /// <summary>Gets the scene instance</summary>
    public readonly IScene Scene;

    /// <summary>Gets the optional DI scope for resource management</summary>
    public readonly IDisposable? Scope;

    /// <summary>
    ///     Initializes a new entry with scene and optional scope
    /// </summary>
    /// <param name="scene">Scene instance to manage</param>
    /// <param name="scope">Optional DI scope for cleanup</param>
    public SceneEntry(IScene scene, IDisposable? scope)
    {
        Scene = scene;
        Scope = scope;
    }
}
