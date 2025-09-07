using Chevron9.Core.Interfaces;
using Chevron9.Core.Types;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Layers.Examples;

/// <summary>
///     Example UI layer that demonstrates efficient use of pooled rendering commands
///     for drawing user interface elements like panels, buttons, and text
/// </summary>
public sealed class UILayer : BaseDrawingLayer
{
    private float _healthValue = 0.8f;
    private float _manaValue = 0.6f;
    private bool _buttonHovered;

    /// <summary>
    ///     Creates a new UI layer
    /// </summary>
    public UILayer() : base("UI", 1000) // High Z-index for UI on top
    {
        // UI layers typically don't clear anything and use alpha blending
        SetClear(LayerClear.None);
        SetCompose(LayerCompositeMode.AlphaBlend);
    }

    /// <summary>
    ///     Updates UI state (animations, hover states, etc.)
    /// </summary>
    /// <param name="fixedDt">Fixed delta time</param>
    /// <param name="input">Input device</param>
    public override void Update(double fixedDt, IInputDevice input)
    {
        // Animate health and mana for demo
        _healthValue += (float)(Math.Sin(DateTime.Now.Millisecond * 0.01) * fixedDt * 0.1);
        _healthValue = Math.Clamp(_healthValue, 0.0f, 1.0f);

        _manaValue += (float)(Math.Cos(DateTime.Now.Millisecond * 0.008) * fixedDt * 0.05);
        _manaValue = Math.Clamp(_manaValue, 0.0f, 1.0f);

        // Simple button hover detection (example)
        var mousePos = input.MousePosition();
        var buttonBounds = new RectF(10, 150, 120, 30);
        _buttonHovered = mousePos.X >= buttonBounds.X && mousePos.X <= buttonBounds.X + buttonBounds.Width &&
                       mousePos.Y >= buttonBounds.Y && mousePos.Y <= buttonBounds.Y + buttonBounds.Height;
    }

    /// <summary>
    ///     Renders UI elements using efficient pooled commands
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="alpha">Interpolation alpha</param>
    public override void Render(IRenderCommandCollector rq, float alpha)
    {
        // Draw main HUD panel
        DrawPanel(rq, new RectF(10, 10, 200, 120), new Color(0, 0, 0, 180)); // Semi-transparent black

        // Health bar
        DrawTextScreen(rq, "Health:", new Position(20, 25), Color.White, 14f);
        DrawHealthBar(rq, new RectF(20, 40, 150, 12), _healthValue * 100, 100, 10,
            new Color(220, 20, 20), new Color(60, 10, 10));

        // Mana bar using progress bar
        DrawTextScreen(rq, "Mana:", new Position(20, 60), Color.White, 14f);
        DrawProgressBar(rq, new RectF(20, 75, 150, 12), _manaValue,
            new Color(20, 20, 60), new Color(60, 60, 220), new Color(100, 100, 100));

        // Score display
        DrawTextScreen(rq, "Score: 12,340", new Position(20, 95), Color.Yellow, 12f);
        DrawTextScreen(rq, "Level: 5", new Position(20, 110), Color.Cyan, 12f);

        // Interactive button
        var buttonColor = _buttonHovered ? new Color(80, 120, 80) : new Color(60, 80, 60);
        var borderColor = _buttonHovered ? Color.White : new Color(120, 120, 120);
        DrawButton(rq, new RectF(10, 150, 120, 30), "Start Game", buttonColor, Color.White, borderColor, 14f);

        // Mini-map frame
        DrawPanelWithBorder(rq, new RectF(250, 10, 100, 100), new Color(40, 40, 40), Color.Gray, 2f);
        DrawTextScreen(rq, "Mini-Map", new Position(275, 55), Color.LightGray, 10f);

        // Status indicators
        DrawStatusIndicators(rq);

        // FPS counter (top-right corner)
        DrawTextScreen(rq, $"FPS: {GetEstimatedFPS()}", new Position(300, 10), Color.Green, 10f);
    }

    /// <summary>
    ///     Handles UI input (button clicks, etc.)
    /// </summary>
    /// <param name="input">Input device</param>
    /// <returns>True if input was consumed by UI</returns>
    public override bool HandleInput(IInputDevice input)
    {
        // Handle button click
        if (_buttonHovered && input.MousePressed(Core.Types.MouseButtonType.Left))
        {
            OnStartGameClicked();
            return true; // Consume the input
        }

        return false; // Don't consume input
    }

    /// <summary>
    ///     Draws various status indicators using pooled commands
    /// </summary>
    private static void DrawStatusIndicators(IRenderCommandCollector rq)
    {
        var startX = 370f;
        var startY = 10f;
        var indicatorSize = 12f;
        var spacing = 15f;

        // Connection status
        var connectionColor = DateTime.Now.Millisecond % 2000 < 1000 ? Color.Green : Color.DarkGreen;
        rq.SubmitRectangle(1000, new RectF(startX, startY, indicatorSize, indicatorSize), connectionColor);
        rq.SubmitText(1000, "Online", new Position(startX + indicatorSize + 5, startY), Color.White, 8f);

        // Sound status
        startY += spacing;
        rq.SubmitRectangle(1000, new RectF(startX, startY, indicatorSize, indicatorSize), Color.Yellow);
        rq.SubmitText(1000, "Sound", new Position(startX + indicatorSize + 5, startY), Color.White, 8f);

        // Notifications
        startY += spacing;
        var notificationColor = DateTime.Now.Millisecond % 3000 < 500 ? Color.Red : Color.DarkRed;
        rq.SubmitRectangle(1000, new RectF(startX, startY, indicatorSize, indicatorSize), notificationColor);
        rq.SubmitText(1000, "3 msgs", new Position(startX + indicatorSize + 5, startY), Color.White, 8f);
    }

    /// <summary>
    ///     Gets estimated FPS for display (simplified)
    /// </summary>
    private static int GetEstimatedFPS()
    {
        return 60; // Placeholder - in real implementation would track frame times
    }

    /// <summary>
    ///     Handles start game button click
    /// </summary>
    private static void OnStartGameClicked()
    {
        // Handle button click logic here
        Console.WriteLine("Start Game button clicked!");
    }
}
