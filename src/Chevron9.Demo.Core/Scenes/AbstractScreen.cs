using Chevron9.Core.Interfaces;
using Chevron9.Core.Layers;
using Chevron9.Core.Render.Commands;
using Chevron9.Core.Scenes;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Demo.Core.Scenes;

/// <summary>
///     Abstract base class for demo screens providing common functionality
///     and simplified layer management for demonstration purposes
/// </summary>
public abstract class AbstractScreen : Scene
{
    private readonly List<ILayer> _screenLayers = new();

    /// <summary>
    ///     Initializes a new instance of the AbstractScreen class
    /// </summary>
    /// <param name="name">Screen name for identification</param>
    /// <param name="title">Display title for the screen</param>
    /// <param name="description">Description of what this screen demonstrates</param>
    protected AbstractScreen(string name, string title, string description)
        : base(name)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    /// <summary>
    ///     Gets the display title for this screen
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     Gets the description of what this screen demonstrates
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets whether this screen is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    ///     Called when screen becomes active
    ///     Override in derived classes for screen-specific initialization
    /// </summary>
    public override void Enter()
    {
        IsActive = true;
        OnEnter();
        base.Enter();
    }

    /// <summary>
    ///     Called when screen becomes inactive
    ///     Override in derived classes for screen-specific cleanup
    /// </summary>
    public override void Close()
    {
        IsActive = false;
        OnClose();
        base.Close();
    }

    /// <summary>
    ///     Template method called when entering the screen
    ///     Override in derived classes for custom initialization
    /// </summary>
    protected virtual void OnEnter()
    {
        // Default implementation does nothing
        // Override in derived classes for custom initialization
    }

    /// <summary>
    ///     Template method called when closing the screen
    ///     Override in derived classes for custom cleanup
    /// </summary>
    protected virtual void OnClose()
    {
        // Default implementation does nothing
        // Override in derived classes for custom cleanup
    }

    /// <summary>
    ///     Adds a layer to this screen
    /// </summary>
    /// <param name="layer">Layer to add</param>
    protected void AddScreenLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        _screenLayers.Add(layer);
        AddLayer(layer);
    }

    /// <summary>
    ///     Removes a layer from this screen
    /// </summary>
    /// <param name="layer">Layer to remove</param>
    /// <returns>True if the layer was removed</returns>
    protected bool RemoveScreenLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        _screenLayers.Remove(layer);
        return RemoveLayer(layer);
    }

    /// <summary>
    ///     Gets a layer by name from this screen
    /// </summary>
    /// <param name="name">Name of the layer to find</param>
    /// <returns>The layer with the specified name, or null if not found</returns>
    protected ILayer? GetScreenLayer(string name)
    {
        return _screenLayers.FirstOrDefault(layer => layer.Name == name);
    }

    /// <summary>
    ///     Gets a layer by type from this screen
    /// </summary>
    /// <typeparam name="T">Type of layer to find</typeparam>
    /// <returns>The first layer of the specified type, or null if not found</returns>
    protected T? GetScreenLayer<T>() where T : class, ILayer
    {
        return _screenLayers.FirstOrDefault(layer => layer is T) as T;
    }

    /// <summary>
    ///     Gets all layers of a specific type from this screen
    /// </summary>
    /// <typeparam name="T">Type of layers to find</typeparam>
    /// <returns>Enumerable of layers of the specified type</returns>
    protected IEnumerable<T> GetScreenLayers<T>() where T : class, ILayer
    {
        return _screenLayers.Where(layer => layer is T).Cast<T>();
    }

    /// <summary>
    ///     Creates a standard info text layer for displaying screen information
    /// </summary>
    /// <param name="zIndex">Z-index for the info layer</param>
    /// <returns>A layer displaying screen title and description</returns>
    protected ILayer CreateInfoLayer(int zIndex = 900)
    {
        return new InfoLayer(Title, Description, zIndex);
    }

    /// <summary>
    ///     Creates a standard background layer
    /// </summary>
    /// <param name="color">Background color</param>
    /// <param name="zIndex">Z-index for the background layer</param>
    /// <returns>A background layer</returns>
    protected static ILayer CreateBackgroundLayer(Color color, int zIndex = 100)
    {
        return new BackgroundLayer(color, zIndex);
    }



    /// <summary>
    ///     Simple info layer for displaying screen information
    /// </summary>
    private sealed class InfoLayer : AbstractLayer
    {
        private readonly string _title;
        private readonly string _description;

        public InfoLayer(string title, string description, int zIndex)
            : base("InfoLayer", zIndex)
        {
            _title = title;
            _description = description;
        }

        public override void Render(IRenderCommandCollector rq, float alpha)
        {
            // Render title
            rq.Submit(ZIndex, new DrawTextCommand(_title, new Position(20, 20), Color.White, 18.0f));

            // Render description
            rq.Submit(ZIndex, new DrawTextCommand(_description, new Position(20, 60), Color.Gray, 14.0f));

            // Render controls hint
            rq.Submit(ZIndex, new DrawTextCommand("Press ESC to return to menu", new Position(20, 100), Color.Yellow, 12.0f));
        }
    }

    /// <summary>
    ///     Simple background layer
    /// </summary>
    private sealed class BackgroundLayer : AbstractLayer
    {
        private readonly Color _color;

        public BackgroundLayer(Color color, int zIndex)
            : base("BackgroundLayer", zIndex)
        {
            _color = color;
        }

        public override void Render(IRenderCommandCollector rq, float alpha)
        {
            // Render full screen background
            // Note: This assumes a typical screen size, in practice you'd want to get actual screen dimensions
            rq.Submit(ZIndex, new DrawRectangleCommand(new RectF(0, 0, 800, 600), _color));
        }
    }
}
