using Chevron9.Core.Scenes;
using Chevron9.Demo.Core.Layers;

namespace Chevron9.Demo.Core.Scenes;

/// <summary>
///     Scene that displays a selection menu for demo options
/// </summary>
public class MenuScene : Scene
{
    private readonly Action<int> _onMenuItemSelected;

    /// <summary>
    ///     Initializes a new instance of the MenuScene class
    /// </summary>
    /// <param name="onMenuItemSelected">Callback when a menu item is selected</param>
    public MenuScene(Action<int> onMenuItemSelected)
        : base("MenuScene")
    {
        _onMenuItemSelected = onMenuItemSelected ?? throw new ArgumentNullException(nameof(onMenuItemSelected));

        InitializeLayers();
    }

    /// <summary>
    ///     Initializes the layers for this scene
    /// </summary>
    private void InitializeLayers()
    {
        var menuItems = new[]
        {
            "Rectangle Demo",
            "Exit"
        };

        var menuLayer = new MenuLayer(menuItems, OnMenuItemSelected);
        AddLayer(menuLayer);
    }

    /// <summary>
    ///     Handles menu item selection
    /// </summary>
    /// <param name="selectedIndex">Index of the selected menu item</param>
    private void OnMenuItemSelected(int selectedIndex)
    {
        _onMenuItemSelected(selectedIndex);
    }
}
