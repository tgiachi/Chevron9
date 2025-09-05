using Chevron9.Core.Interfaces;

namespace Chevron9.Core.Scenes;

/// <summary>
///     Abstract base class for game scenes containing multiple rendering layers
///     Provides common scene functionality and layer management
/// </summary>
public abstract class Scene : IScene
{
    private readonly List<ILayer> _layers = new();
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the Scene class
    /// </summary>
    /// <param name="name">Scene name for identification</param>
    protected Scene(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    ///     Gets the scene name for identification and debugging
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the read-only list of rendering layers in this scene
    ///     Layers are rendered in order with Z-sorting
    /// </summary>
    public IReadOnlyList<ILayer> Layers => _layers;

    /// <summary>
    ///     Called when scene becomes active (pushed onto scene stack)
    ///     Use for initialization, resource loading, state setup
    /// </summary>
    public virtual void Enter()
    {
        // Notify all layers that the scene is entering
        foreach (var layer in _layers)
        {
            // Layers don't have Enter/Exit methods, but derived classes can override this
        }
    }

    /// <summary>
    ///     Called when scene becomes inactive (popped from scene stack)
    ///     Use for cleanup, state saving, resource unloading
    /// </summary>
    public virtual void Close()
    {
        // Notify all layers that the scene is closing
        foreach (var layer in _layers)
        {
            // Layers don't have Close methods, but derived classes can override this
        }
    }

    /// <summary>
    ///     Updates scene logic with fixed timestep for consistent gameplay
    /// </summary>
    /// <param name="fixedDt">Fixed delta time in seconds</param>
    /// <param name="input">Input device for user interaction</param>
    public virtual void Update(double fixedDt, IInputDevice input)
    {
        // Update all enabled layers
        foreach (var layer in _layers)
        {
            if (layer.Enabled)
            {
                layer.Update(fixedDt, input);
            }
        }
    }

    /// <summary>
    ///     Renders all scene layers to the render command collector
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering between updates</param>
    public virtual void Render(IRenderCommandCollector rq, float alpha)
    {
        // Render all visible layers
        foreach (var layer in _layers)
        {
            if (layer.Visible)
            {
                layer.Render(rq, alpha);
            }
        }
    }

    /// <summary>
    ///     Processes input through scene layers, typically from top to bottom
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed (e.g., by UI layer)</returns>
    public virtual bool HandleInput(IInputDevice input)
    {
        // Process input through layers from top to bottom (reverse order)
        for (int i = _layers.Count - 1; i >= 0; i--)
        {
            var layer = _layers[i];
            if (layer.Enabled && layer.HandleInput(input))
            {
                // Input was consumed by this layer
                return true;
            }
        }

        // Input was not consumed by any layer
        return false;
    }

    /// <summary>
    ///     Disposes the scene and all its layers
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        // Dispose all layers
        foreach (var layer in _layers)
        {
            if (layer is IDisposable disposableLayer)
            {
                disposableLayer.Dispose();
            }
        }

        _layers.Clear();

        // Allow derived classes to perform additional cleanup
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Adds a layer to the scene
    /// </summary>
    /// <param name="layer">Layer to add</param>
    protected void AddLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        if (_layers.Contains(layer))
        {
            throw new InvalidOperationException("Layer is already added to this scene");
        }

        _layers.Add(layer);

        // Sort layers by Z-index after adding
        SortLayersByZIndex();
    }

    /// <summary>
    ///     Removes a layer from the scene
    /// </summary>
    /// <param name="layer">Layer to remove</param>
    /// <returns>True if the layer was removed</returns>
    protected bool RemoveLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        return _layers.Remove(layer);
    }

    /// <summary>
    ///     Gets a layer by name
    /// </summary>
    /// <param name="name">Name of the layer to find</param>
    /// <returns>The layer with the specified name, or null if not found</returns>
    protected ILayer? GetLayer(string name)
    {
        return _layers.FirstOrDefault(layer => layer.Name == name);
    }

    /// <summary>
    ///     Gets a layer by type
    /// </summary>
    /// <typeparam name="T">Type of layer to find</typeparam>
    /// <returns>The first layer of the specified type, or null if not found</returns>
    protected T? GetLayer<T>() where T : class, ILayer
    {
        return _layers.FirstOrDefault(layer => layer is T) as T;
    }

    /// <summary>
    ///     Sorts layers by Z-index for proper rendering order
    /// </summary>
    private void SortLayersByZIndex()
    {
        _layers.Sort((a, b) => a.ZIndex.CompareTo(b.ZIndex));
    }

    /// <summary>
    ///     Allows derived classes to perform additional cleanup
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
    protected virtual void Dispose(bool disposing)
    {
        // Default implementation does nothing
        // Override in derived classes for custom cleanup
    }
}
