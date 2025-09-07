using Chevron9.Core.Extensions;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Layers;
using Chevron9.Core.Render.Commands;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Demo.Core.Layers;

/// <summary>
///     Layer for rendering and handling input for a selection menu
/// </summary>
public class MenuLayer : AbstractLayer
{
    private readonly string[] _menuItems;
    private readonly Action<int> _onItemSelected;
    private int _selectedIndex;

    /// <summary>
    ///     Initializes a new instance of the MenuLayer class
    /// </summary>
    /// <param name="menuItems">Array of menu item texts</param>
    /// <param name="onItemSelected">Callback when an item is selected</param>
    public MenuLayer(string[] menuItems, Action<int> onItemSelected)
        : base("MenuLayer", 1000) // UI layer Z-index
    {
        _menuItems = menuItems ?? throw new ArgumentNullException(nameof(menuItems));
        _onItemSelected = onItemSelected ?? throw new ArgumentNullException(nameof(onItemSelected));
        _selectedIndex = 0;
    }

    /// <summary>
    ///     Processes input for menu navigation and selection
    /// </summary>
    /// <param name="input">Input device to process</param>
    /// <returns>True if input was consumed</returns>
    public override bool HandleInput(IInputDevice input)
    {
        // Handle keyboard input for menu navigation
        if (input.WasPressed(Chevron9.Core.Data.Input.InputKeys.Up))
        {
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
            return true;
        }

        if (input.WasPressed(Chevron9.Core.Data.Input.InputKeys.Down))
        {
            _selectedIndex = Math.Min(_menuItems.Length - 1, _selectedIndex + 1);
            return true;
        }

        if (input.WasPressed(Chevron9.Core.Data.Input.InputKeys.Enter))
        {
            _onItemSelected(_selectedIndex);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Renders the menu to the render command collector
    /// </summary>
    /// <param name="rq">Render command collector to submit commands to</param>
    /// <param name="alpha">Interpolation alpha for smooth rendering</param>
    public override void Render(IRenderCommandCollector rq, float alpha)
    {
        const float fontSize = 16.0f;
        var startY = 100.0f;
        const float lineHeight = 30.0f;

        // Render title
        rq.SubmitText(ZIndex, "Chevron9 Demo Menu", new Position(50, 50), Color.White, fontSize * 1.5f);

        // Render menu items
        for (int i = 0; i < _menuItems.Length; i++)
        {
            var color = i == _selectedIndex ? Color.Yellow : Color.White;
            var position = new Position(50, startY + i * lineHeight);

            // Render selection indicator
            if (i == _selectedIndex)
            {
                rq.SubmitText(ZIndex, ">", new Position(30, startY + i * lineHeight), Color.Yellow, fontSize);
            }

            rq.SubmitText(ZIndex, _menuItems[i], position, color, fontSize);
        }

        // Render instructions
        rq.SubmitText(ZIndex, "Use UP/DOWN arrows to navigate, ENTER to select",
            new Position(50, startY + _menuItems.Length * lineHeight + 20),
            Color.Gray, fontSize * 0.8f);
    }
}
