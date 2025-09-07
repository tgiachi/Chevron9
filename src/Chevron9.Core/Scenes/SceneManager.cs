using Chevron9.Core.Extensions;
using Chevron9.Core.Extensions.Arrays;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Internal;

namespace Chevron9.Core.Scenes;

/// <summary>
///     Default stack-based scene manager implementing ISceneManager
///     Provides thread-safe scene management with deferred operations during Update/Render cycles
///     Supports dependency injection scopes and proper resource cleanup
///     Includes support for global layers that are always rendered on top of scenes
/// </summary>
public sealed class SceneManager : ISceneManager, IDisposable
{
    private readonly Queue<Action> _pendingOps = new();
    private readonly List<ILayer> _globalLayers = new();

    private readonly Stack<SceneEntry> _stack = new();
    private bool _disposed;
    private bool _mutating; // Prevents stack modifications during Update/Render


    /// <summary>
    ///     Disposes the scene manager and all managed scenes
    ///     Pops all scenes with proper cleanup and clears pending operations
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        // Pop all scenes with proper Close()/Dispose() cleanup
        while (_stack.Count > 0)
        {
            DoPop();
        }

        // Clear any remaining pending operations
        _pendingOps.Clear();
    }

    /// <summary>
    ///     Gets the currently active scene (top of stack) or null if empty
    ///     Only the current scene receives input and updates
    /// </summary>
    public IScene? Current => _stack.Count > 0 ? _stack.Peek().Scene : null;

    /// <summary>
    ///     Gets the read-only list of global layers
    ///     Global layers are always rendered on top of all scenes
    /// </summary>
    public IReadOnlyList<ILayer> GlobalLayers => _globalLayers;

    // ----------------- Public API -----------------

    /// <summary>
    ///     Pushes a scene onto the stack without DI scope
    /// </summary>
    /// <param name="scene">Scene to push and activate</param>
    public void Push(IScene scene)
    {
        Push(scene, null);
    }

    /// <summary>
    ///     Pops the current scene from stack, calling Close() and Dispose()
    ///     Operation may be deferred during Update/Render cycles
    /// </summary>
    public void Pop()
    {
        EnqueueOrRun(DoPop);
    }

    /// <summary>
    ///     Replaces current scene without DI scope
    /// </summary>
    /// <param name="scene">Scene to replace current with</param>
    public void Replace(IScene scene)
    {
        Replace(scene, null);
    }

    // ----------------- Global Layer Management -----------------

    /// <summary>
    ///     Adds a global layer that will be rendered on top of all scenes
    ///     Global layers receive input after scene processing and are always updated
    /// </summary>
    /// <param name="layer">Global layer to add</param>
    public void AddGlobalLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        if (_globalLayers.Contains(layer))
        {
            throw new InvalidOperationException("Global layer is already added");
        }

        _globalLayers.Add(layer);

        // Sort global layers by Z-index after adding
        SortGlobalLayersByZIndex();
    }

    /// <summary>
    ///     Removes a global layer
    /// </summary>
    /// <param name="layer">Global layer to remove</param>
    /// <returns>True if the layer was removed</returns>
    public bool RemoveGlobalLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);
        return _globalLayers.Remove(layer);
    }

    /// <summary>
    ///     Gets a global layer by name
    /// </summary>
    /// <param name="name">Name of the global layer to find</param>
    /// <returns>The global layer with the specified name, or null if not found</returns>
    public ILayer? GetGlobalLayer(string name)
    {
        return _globalLayers.FirstOrDefault(layer => layer.Name == name);
    }

    /// <summary>
    ///     Gets a global layer by type
    /// </summary>
    /// <typeparam name="T">Type of global layer to find</typeparam>
    /// <returns>The first global layer of the specified type, or null if not found</returns>
    public T? GetGlobalLayer<T>() where T : class, ILayer
    {
        return _globalLayers.FirstOrDefault(layer => layer is T) as T;
    }

    /// <summary>
    ///     Updates the current active scene with fixed timestep
    ///     Processes input first, then updates game logic
    ///     Also updates all global layers
    ///     Defers any pending Push/Pop/Replace operations until after update
    /// </summary>
    /// <param name="fixedDt">Fixed delta time for consistent game logic</param>
    /// <param name="input">Input device for user interaction</param>
    public void Update(double fixedDt, IInputDevice input)
    {
        if (_disposed)
        {
            return;
        }

        _mutating = true;
        try
        {
            var top = Current;
            top?.HandleInput(input); // Handle input before logic update
            top?.Update(fixedDt, input);

            // Update all enabled global layers
            foreach (var layer in _globalLayers)
            {
                if (layer.Enabled)
                {
                    layer.Update(fixedDt, input);
                }
            }
        }
        finally
        {
            _mutating = false;
            DrainPendingOps(); // Process any deferred scene operations
        }
    }

    /// <summary>
    ///     Renders all scenes in the stack from bottom to top for proper layering
    ///     Background scenes render first, current scene renders on top
    ///     Then renders all visible global layers on top of everything
    ///     Defers any pending Push/Pop/Replace operations until after render
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering</param>
    public void Render(IRenderCommandCollector rq, float alpha)
    {
        if (_disposed)
        {
            return;
        }

        _mutating = true;
        try
        {
            if (_stack.Count == 0)
            {
                return;
            }

            // Render in bottom -> top order for proper scene layering
            foreach (var entry in _stack.ToArray().ReverseEnumerate())
            {
                if (entry.Scene is { } s)
                {
                    s.Render(rq, alpha);
                }
            }

            // Render all visible global layers on top
            foreach (var layer in _globalLayers)
            {
                if (layer.Visible)
                {
                    layer.Render(rq, alpha);
                }
            }
        }
        finally
        {
            _mutating = false;
            DrainPendingOps(); // Process any deferred scene operations
        }
    }

    /// <summary>
    ///     Processes input through the current active scene first, then global layers
    ///     Global layers receive input after the scene, allowing them to override scene input
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed by the scene or any global layer</returns>
    public bool HandleInput(IInputDevice input)
    {
        // First, let the current scene handle input
        var top = Current;
        if (top is not null && top.HandleInput(input))
        {
            return true;
        }

        // Then, let global layers handle input (processed from bottom to top by Z-index)
        foreach (var layer in _globalLayers)
        {
            if (layer.Enabled && layer.HandleInput(input))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Pushes a scene onto the stack with optional DI scope for dependency injection
    ///     Scene.Enter() is called immediately, operation may be deferred during Update/Render
    /// </summary>
    /// <param name="scene">Scene to push and activate</param>
    /// <param name="scope">Optional DI scope that will be disposed when scene is popped</param>
    public void Push(IScene scene, IDisposable? scope)
    {
        ArgumentNullException.ThrowIfNull(scene);
        EnqueueOrRun(() => DoPush(scene, scope));
    }

    /// <summary>
    ///     Replaces current scene with a new one, including optional DI scope
    ///     Equivalent to Pop() followed by Push() but more efficient
    ///     Operation may be deferred during Update/Render cycles
    /// </summary>
    /// <param name="scene">Scene to replace current with</param>
    /// <param name="scope">Optional DI scope for the new scene</param>
    public void Replace(IScene scene, IDisposable? scope)
    {
        ArgumentNullException.ThrowIfNull(scene);
        EnqueueOrRun(() =>
            {
                DoPop();
                DoPush(scene, scope);
            }
        );
    }


    // ----------------- Internal Implementation -----------------

    /// <summary>
    ///     Internal method to push a scene onto the stack
    ///     Calls Enter() before pushing to ensure proper initialization
    /// </summary>
    private void DoPush(IScene scene, IDisposable? scope)
    {
        // Enter BEFORE pushing to ensure initialization before any potential Render calls
        scene.Enter();
        _stack.Push(new SceneEntry(scene, scope));
    }

    /// <summary>
    ///     Internal method to pop current scene from stack with safe cleanup
    /// </summary>
    private void DoPop()
    {
        if (_stack.Count == 0)
        {
            return;
        }

        var entry = _stack.Pop();
        SafeExit(entry.Scene);
        SafeDispose(entry.Scope);
    }

    /// <summary>
    ///     Executes operation immediately or defers it if currently mutating
    ///     Prevents stack modifications during Update/Render iteration
    /// </summary>
    private void EnqueueOrRun(Action op)
    {
        if (_mutating)
        {
            _pendingOps.Enqueue(op);
        }
        else
        {
            op();
        }
    }

    /// <summary>
    ///     Processes all pending deferred operations in FIFO order
    /// </summary>
    private void DrainPendingOps()
    {
        while (_pendingOps.Count > 0)
        {
            var op = _pendingOps.Dequeue();
            op();
        }
    }

    /// <summary>
    ///     Safely exits a scene with exception handling
    ///     Calls Close() then Dispose() with individual try-catch blocks
    /// </summary>
    private static void SafeExit(IScene scene)
    {
        try
        {
            scene.Close();
        }
        catch
        {
            // Swallow exceptions during scene exit to prevent cascade failures
            // Production code should log these exceptions
        }

        try
        {
            scene.Dispose();
        }
        catch
        {
            // Swallow exceptions during disposal to ensure cleanup continues
            // Production code should log these exceptions
        }
    }

    /// <summary>
    ///     Safely disposes an optional disposable with exception handling
    /// </summary>
    private static void SafeDispose(IDisposable? d)
    {
        try
        {
            d?.Dispose();
        }
        catch
        {
            // Swallow exceptions during scope disposal
            // Production code should log these exceptions
        }
    }

    /// <summary>
    ///     Sorts global layers by Z-index for proper rendering order
    /// </summary>
    private void SortGlobalLayersByZIndex()
    {
        _globalLayers.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
    }
}
