using Chevron9.Core.Interfaces;
using Chevron9.Core.Scenes;
using Chevron9.Demo.Core.Layers;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Demo.Core.Scenes;

/// <summary>
///     Scene that displays a rectangle demo with animation
/// </summary>
public class RectangleScene : Scene
{
    private readonly Action _onBackToMenu;

    /// <summary>
    ///     Initializes a new instance of the RectangleScene class
    /// </summary>
    /// <param name="onBackToMenu">Callback to return to menu</param>
    public RectangleScene(Action onBackToMenu)
        : base("RectangleScene")
    {
        _onBackToMenu = onBackToMenu ?? throw new ArgumentNullException(nameof(onBackToMenu));

        InitializeLayers();
    }

    /// <summary>
    ///     Initializes the layers for this scene
    /// </summary>
    private void InitializeLayers()
    {
        // Create a rectangle in the center of a typical screen
        var rectangleBounds = new RectF(300, 200, 200, 150);
        var rectangleColor = Color.Blue;

        var rectangleLayer = new RectangleLayer(rectangleBounds, rectangleColor);
        AddLayer(rectangleLayer);
    }

    /// <summary>
    ///     Processes input for this scene
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed</returns>
    public override bool HandleInput(IInputDevice input)
    {
        // Handle escape key to go back to menu
        if (input.WasPressed(Chevron9.Core.Data.Input.InputKeys.Escape))
        {
            _onBackToMenu();
            return true;
        }

        // Let base class handle other input
        return base.HandleInput(input);
    }
}
