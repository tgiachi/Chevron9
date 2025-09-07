using Chevron9.Core.Extensions.Cameras;
using Chevron9.Core.Interfaces;
using Chevron9.Core.Types;
using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Core.Layers.Examples;

/// <summary>
///     Example game world layer that demonstrates efficient rendering of world objects
///     using pooled commands with camera transformations for optimal memory usage
/// </summary>
public sealed class GameWorldLayer : BaseDrawingLayer
{
    private readonly List<GameObject> _gameObjects = new();
    private readonly Random _random = new();
    private float _time;

    /// <summary>
    ///     Creates a new game world layer
    /// </summary>
    public GameWorldLayer() : base("GameWorld", 100) // Lower Z-index for world objects
    {
        // World layers typically clear depth but not color
        SetClear(LayerClear.Depth);
        SetCompose(LayerCompositeMode.Overwrite);

        // Initialize some demo game objects
        InitializeGameObjects();
    }

    /// <summary>
    ///     Updates game world state and objects
    /// </summary>
    /// <param name="fixedDt">Fixed delta time</param>
    /// <param name="input">Input device</param>
    public override void Update(double fixedDt, IInputDevice input)
    {
        _time += (float)fixedDt;

        // Update camera position with input
        UpdateCamera(fixedDt, input);

        // Update all game objects
        foreach (var obj in _gameObjects)
        {
            obj.Update(fixedDt);
        }

        // Spawn new objects occasionally
        if (_random.Next(0, 120) == 0) // About once every 2 seconds at 60fps
        {
            SpawnRandomObject();
        }
    }

    /// <summary>
    ///     Renders all game world objects using efficient pooled commands
    /// </summary>
    /// <param name="rq">Render command collector</param>
    /// <param name="alpha">Interpolation alpha</param>
    public override void Render(IRenderCommandCollector rq, float alpha)
    {
        // Get camera viewport for culling
        var viewport = Camera.ViewBounds;

        // Render background grid
        RenderWorldGrid(rq, viewport);

        // Render all visible game objects
        var renderedCount = 0;
        foreach (var obj in _gameObjects)
        {
            // Frustum culling - only render objects that are visible
            if (Camera.IsVisible(obj.Bounds))
            {
                RenderGameObject(rq, obj, alpha);
                renderedCount++;
            }
        }

        // Debug info (world coordinates, will be transformed to screen)
        var debugPos = new Position(viewport.X + 10, viewport.Y + 10);
        DrawText(rq, $"Objects: {_gameObjects.Count} (rendered: {renderedCount})",
            debugPos, Color.White, 10f);

        DrawText(rq, $"Camera: ({Camera.Position.X:F1}, {Camera.Position.Y:F1}) Zoom: {Camera.Zoom:F2}",
            new Position(debugPos.X, debugPos.Y + 15), Color.White, 10f);
    }

    /// <summary>
    ///     Handles world input (camera controls, object interaction)
    /// </summary>
    /// <param name="input">Input device</param>
    /// <returns>True if input was consumed</returns>
    public override bool HandleInput(IInputDevice input)
    {
        // Handle object selection with mouse
        if (input.MousePressed(Core.Types.MouseButtonType.Left))
        {
            var mouseWorldPos = Camera.ScreenToWorld(input.MousePosition());

            foreach (var obj in _gameObjects)
            {
                if (IsPointInBounds(mouseWorldPos, obj.Bounds))
                {
                    obj.OnClicked();
                    return true; // Consume input
                }
            }
        }

        return false;
    }

    /// <summary>
    ///     Updates camera position based on input
    /// </summary>
    private void UpdateCamera(double fixedDt, IInputDevice input)
    {
        const float cameraSpeed = 200f;
        const float zoomSpeed = 2f;

        var movement = new Position(0, 0);

        // Camera movement with WASD or arrow keys
        if (input.IsDown(Core.Data.Input.InputKeys.W) || input.IsDown(Core.Data.Input.InputKeys.ArrowUp))
            movement = new Position(movement.X, movement.Y - cameraSpeed * (float)fixedDt);

        if (input.IsDown(Core.Data.Input.InputKeys.S) || input.IsDown(Core.Data.Input.InputKeys.ArrowDown))
            movement = new Position(movement.X, movement.Y + cameraSpeed * (float)fixedDt);

        if (input.IsDown(Core.Data.Input.InputKeys.A) || input.IsDown(Core.Data.Input.InputKeys.ArrowLeft))
            movement = new Position(movement.X - cameraSpeed * (float)fixedDt, movement.Y);

        if (input.IsDown(Core.Data.Input.InputKeys.D) || input.IsDown(Core.Data.Input.InputKeys.ArrowRight))
            movement = new Position(movement.X + cameraSpeed * (float)fixedDt, movement.Y);

        if (movement.X != 0 || movement.Y != 0)
        {
            Camera.Position = new Position(
                Camera.Position.X + movement.X,
                Camera.Position.Y + movement.Y);
        }

        // Zoom with mouse wheel
        var wheelDelta = input.MouseWheelDelta();
        if (wheelDelta.Y != 0)
        {
            var newZoom = Math.Clamp(Camera.Zoom + wheelDelta.Y * zoomSpeed * (float)fixedDt, 0.1f, 5.0f);
            Camera.Zoom = newZoom;
        }
    }

    /// <summary>
    ///     Renders a background grid for world reference
    /// </summary>
    private void RenderWorldGrid(IRenderCommandCollector rq, RectF viewport)
    {
        const float gridSize = 50f;
        var gridColor = new Color(40, 40, 40);

        // Calculate grid lines to draw
        var startX = (float)(Math.Floor(viewport.X / gridSize) * gridSize);
        var endX = viewport.X + viewport.Width;
        var startY = (float)(Math.Floor(viewport.Y / gridSize) * gridSize);
        var endY = viewport.Y + viewport.Height;

        // Draw vertical grid lines
        for (var x = startX; x <= endX; x += gridSize)
        {
            DrawRectangle(rq, x, startY, 1f, viewport.Height, gridColor);
        }

        // Draw horizontal grid lines
        for (var y = startY; y <= endY; y += gridSize)
        {
            DrawRectangle(rq, startX, y, viewport.Width, 1f, gridColor);
        }
    }

    /// <summary>
    ///     Renders a single game object using pooled commands
    /// </summary>
    private void RenderGameObject(IRenderCommandCollector rq, GameObject obj, float alpha)
    {
        // Interpolate position for smooth rendering
        var renderPos = LerpPosition(obj.PreviousPosition, obj.Position, alpha);
        var renderBounds = new RectF(renderPos.X, renderPos.Y, obj.Bounds.Width, obj.Bounds.Height);

        // Draw object body
        DrawRectangle(rq, renderBounds, obj.Color);

        // Draw object outline if selected
        if (obj.IsSelected)
        {
            var outlineBounds = new RectF(
                renderBounds.X - 2, renderBounds.Y - 2,
                renderBounds.Width + 4, renderBounds.Height + 4);
            DrawRectangle(rq, outlineBounds, Color.Yellow);
        }

        // Draw object label
        if (!string.IsNullOrEmpty(obj.Name))
        {
            var labelPos = new Position(renderPos.X, renderPos.Y - 15);
            DrawText(rq, obj.Name, labelPos, Color.White, 8f);
        }

        // Draw velocity vector for moving objects
        if (obj.Velocity.X != 0 || obj.Velocity.Y != 0)
        {
            var velocityEnd = new Position(
                renderPos.X + obj.Velocity.X * 0.1f,
                renderPos.Y + obj.Velocity.Y * 0.1f);

            // Draw velocity as a small line (using thin rectangle)
            var velocityLength = (float)Math.Sqrt(obj.Velocity.X * obj.Velocity.X + obj.Velocity.Y * obj.Velocity.Y);
            if (velocityLength > 0.1f)
            {
                DrawRectangle(rq, renderPos.X, renderPos.Y, velocityEnd.X - renderPos.X, 2f, Color.Red);
            }
        }
    }

    /// <summary>
    ///     Initializes some demo game objects
    /// </summary>
    private void InitializeGameObjects()
    {
        for (int i = 0; i < 50; i++)
        {
            var obj = new GameObject
            {
                Name = $"Object{i}",
                Position = new Position(_random.Next(-500, 500), _random.Next(-500, 500)),
                Bounds = new RectF(0, 0, _random.Next(10, 40), _random.Next(10, 40)),
                Color = new Color((byte)_random.Next(100, 255), (byte)_random.Next(100, 255), (byte)_random.Next(100, 255)),
                Velocity = new Position((_random.NextSingle() - 0.5f) * 100, (_random.NextSingle() - 0.5f) * 100)
            };
            obj.PreviousPosition = obj.Position;
            _gameObjects.Add(obj);
        }
    }

    /// <summary>
    ///     Spawns a new random object
    /// </summary>
    private void SpawnRandomObject()
    {
        if (_gameObjects.Count >= 100) return; // Limit object count

        var obj = new GameObject
        {
            Name = $"Spawned{_gameObjects.Count}",
            Position = new Position(
                Camera.Position.X + _random.Next(-200, 200),
                Camera.Position.Y + _random.Next(-200, 200)),
            Bounds = new RectF(0, 0, _random.Next(8, 25), _random.Next(8, 25)),
            Color = new Color((byte)_random.Next(150, 255), (byte)_random.Next(150, 255), (byte)_random.Next(150, 255)),
            Velocity = new Position((_random.NextSingle() - 0.5f) * 150, (_random.NextSingle() - 0.5f) * 150)
        };
        obj.PreviousPosition = obj.Position;
        _gameObjects.Add(obj);
    }

    /// <summary>
    ///     Checks if a point is within bounds
    /// </summary>
    private static bool IsPointInBounds(Position point, RectF bounds)
    {
        return point.X >= bounds.X && point.X <= bounds.X + bounds.Width &&
               point.Y >= bounds.Y && point.Y <= bounds.Y + bounds.Height;
    }

    /// <summary>
    ///     Interpolates between two positions
    /// </summary>
    private static Position LerpPosition(Position from, Position to, float alpha)
    {
        return new Position(
            from.X + (to.X - from.X) * alpha,
            from.Y + (to.Y - from.Y) * alpha
        );
    }
}

/// <summary>
///     Simple game object for demonstration
/// </summary>
public class GameObject
{
    public string Name { get; set; } = string.Empty;
    public Position Position { get; set; }
    public Position PreviousPosition { get; set; }
    public Position Velocity { get; set; }
    public RectF Bounds { get; set; }
    public Color Color { get; set; }
    public bool IsSelected { get; set; }

    public void Update(double fixedDt)
    {
        PreviousPosition = Position;

        // Apply velocity
        Position = new Position(
            Position.X + Velocity.X * (float)fixedDt,
            Position.Y + Velocity.Y * (float)fixedDt);

        // Update bounds position
        Bounds = new RectF(Position.X, Position.Y, Bounds.Width, Bounds.Height);

        // Simple physics - slow down over time
        Velocity = new Position(Velocity.X * 0.995f, Velocity.Y * 0.995f);
    }

    public void OnClicked()
    {
        IsSelected = !IsSelected;
        Console.WriteLine($"GameObject {Name} clicked! Selected: {IsSelected}");
    }
}
