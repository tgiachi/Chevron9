using Chevron9.Core.Cameras;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Core.Cameras;

[TestFixture]
public class Camera2DTests
{
    private Camera2D _camera = null!;

    [SetUp]
    public void SetUp()
    {
        _camera = new Camera2D();
        _camera.Viewport = new RectF(0, 0, 800, 600);
    }

    [Test]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var camera = new Camera2D();

        Assert.That(camera.Position.X, Is.EqualTo(0.0f));
        Assert.That(camera.Position.Y, Is.EqualTo(0.0f));
        Assert.That(camera.Zoom, Is.EqualTo(1.0f));
        Assert.That(camera.Rotation, Is.EqualTo(0.0f));
    }

    [Test]
    public void Position_WhenSet_ShouldUpdatePosition()
    {
        var newPosition = new Position(100, 200);

        _camera.Position = newPosition;

        Assert.That(_camera.Position, Is.EqualTo(newPosition));
    }

    [Test]
    public void Zoom_WhenSetToPositiveValue_ShouldUpdateZoom()
    {
        _camera.Zoom = 2.0f;

        Assert.That(_camera.Zoom, Is.EqualTo(2.0f));
    }

    [Test]
    public void Zoom_WhenSetToZeroOrNegative_ShouldClampToMinimum()
    {
        _camera.Zoom = 0.0f;
        Assert.That(_camera.Zoom, Is.GreaterThan(0.0f));

        _camera.Zoom = -1.0f;
        Assert.That(_camera.Zoom, Is.GreaterThan(0.0f));
    }

    [Test]
    public void Rotation_WhenSet_ShouldUpdateRotation()
    {
        var rotation = (float)Math.PI / 4; // 45 degrees

        _camera.Rotation = rotation;

        Assert.That(_camera.Rotation, Is.EqualTo(rotation).Within(0.0001f));
    }

    [Test]
    public void Viewport_WhenSet_ShouldUpdateViewport()
    {
        var newViewport = new RectF(0, 0, 1920, 1080);

        _camera.Viewport = newViewport;

        Assert.That(_camera.Viewport, Is.EqualTo(newViewport));
    }

    [Test]
    public void WorldToScreen_WithIdentityTransform_ShouldReturnScreenCenter()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 1.0f;
        _camera.Rotation = 0.0f;

        var screenPos = _camera.WorldToScreen(new Position(0, 0));

        Assert.That(screenPos.X, Is.EqualTo(400.0f).Within(0.0001f)); // Half of 800
        Assert.That(screenPos.Y, Is.EqualTo(300.0f).Within(0.0001f)); // Half of 600
    }

    [Test]
    public void WorldToScreen_WithOffset_ShouldTransformCorrectly()
    {
        _camera.Position = new Position(100, 50);
        
        var screenPos = _camera.WorldToScreen(new Position(200, 150));

        Assert.That(screenPos.X, Is.EqualTo(500.0f).Within(0.0001f)); // (200-100) + 400
        Assert.That(screenPos.Y, Is.EqualTo(400.0f).Within(0.0001f)); // (150-50) + 300
    }

    [Test]
    public void WorldToScreen_WithZoom_ShouldScaleCorrectly()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 2.0f;

        var screenPos = _camera.WorldToScreen(new Position(100, 100));

        Assert.That(screenPos.X, Is.EqualTo(600.0f).Within(0.0001f)); // 100 * 2 + 400
        Assert.That(screenPos.Y, Is.EqualTo(500.0f).Within(0.0001f)); // 100 * 2 + 300
    }

    [Test]
    public void ScreenToWorld_WithIdentityTransform_ShouldReturnWorldOrigin()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 1.0f;
        _camera.Rotation = 0.0f;

        var worldPos = _camera.ScreenToWorld(new Position(400, 300));

        Assert.That(worldPos.X, Is.EqualTo(0.0f).Within(0.0001f));
        Assert.That(worldPos.Y, Is.EqualTo(0.0f).Within(0.0001f));
    }

    [Test]
    public void ScreenToWorld_WithOffset_ShouldTransformCorrectly()
    {
        _camera.Position = new Position(100, 50);

        var worldPos = _camera.ScreenToWorld(new Position(500, 400));

        Assert.That(worldPos.X, Is.EqualTo(200.0f).Within(0.0001f)); // (500-400) + 100
        Assert.That(worldPos.Y, Is.EqualTo(150.0f).Within(0.0001f)); // (400-300) + 50
    }

    [Test]
    public void ScreenToWorld_WithZoom_ShouldScaleCorrectly()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 2.0f;

        var worldPos = _camera.ScreenToWorld(new Position(600, 500));

        Assert.That(worldPos.X, Is.EqualTo(100.0f).Within(0.0001f)); // (600-400) / 2
        Assert.That(worldPos.Y, Is.EqualTo(100.0f).Within(0.0001f)); // (500-300) / 2
    }

    [Test]
    public void WorldToScreen_AndScreenToWorld_ShouldBeInverse()
    {
        _camera.Position = new Position(123, 456);
        _camera.Zoom = 1.5f;
        _camera.Rotation = 0.3f;

        var originalWorld = new Position(789, 321);
        var screenPos = _camera.WorldToScreen(originalWorld);
        var backToWorld = _camera.ScreenToWorld(screenPos);

        Assert.That(backToWorld.X, Is.EqualTo(originalWorld.X).Within(0.001f));
        Assert.That(backToWorld.Y, Is.EqualTo(originalWorld.Y).Within(0.001f));
    }

    [Test]
    public void Move_ShouldOffsetPosition()
    {
        _camera.Position = new Position(100, 200);
        var offset = new Position(50, -30);

        _camera.Move(offset);

        Assert.That(_camera.Position.X, Is.EqualTo(150.0f).Within(0.0001f));
        Assert.That(_camera.Position.Y, Is.EqualTo(170.0f).Within(0.0001f));
    }

    [Test]
    public void LookAt_ShouldSetPosition()
    {
        var targetPosition = new Position(500, 300);

        _camera.LookAt(targetPosition);

        Assert.That(_camera.Position, Is.EqualTo(targetPosition));
    }

    [Test]
    public void ViewBounds_WithIdentityTransform_ShouldCoverViewport()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 1.0f;
        _camera.Rotation = 0.0f;

        var bounds = _camera.ViewBounds;

        Assert.That(bounds.X, Is.EqualTo(-400.0f).Within(0.0001f));
        Assert.That(bounds.Y, Is.EqualTo(-300.0f).Within(0.0001f));
        Assert.That(bounds.Width, Is.EqualTo(800.0f).Within(0.0001f));
        Assert.That(bounds.Height, Is.EqualTo(600.0f).Within(0.0001f));
    }

    [Test]
    public void ViewBounds_WithZoom_ShouldScaleInverse()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 2.0f;

        var bounds = _camera.ViewBounds;

        Assert.That(bounds.X, Is.EqualTo(-200.0f).Within(0.0001f));
        Assert.That(bounds.Y, Is.EqualTo(-150.0f).Within(0.0001f));
        Assert.That(bounds.Width, Is.EqualTo(400.0f).Within(0.0001f));
        Assert.That(bounds.Height, Is.EqualTo(300.0f).Within(0.0001f));
    }

    [Test]
    public void ViewBounds_WithPosition_ShouldOffset()
    {
        _camera.Position = new Position(100, 50);
        _camera.Zoom = 1.0f;

        var bounds = _camera.ViewBounds;

        Assert.That(bounds.X, Is.EqualTo(-300.0f).Within(0.0001f)); // 100 - 400
        Assert.That(bounds.Y, Is.EqualTo(-250.0f).Within(0.0001f)); // 50 - 300
        Assert.That(bounds.Width, Is.EqualTo(800.0f).Within(0.0001f));
        Assert.That(bounds.Height, Is.EqualTo(600.0f).Within(0.0001f));
    }

    [Test]
    public void IsVisible_Position_WithPositionInBounds_ShouldReturnTrue()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 1.0f;

        var result = _camera.IsVisible(new Position(0, 0));

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsVisible_Position_WithPositionOutOfBounds_ShouldReturnFalse()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 1.0f;

        var result = _camera.IsVisible(new Position(1000, 1000));

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsVisible_Rect_WithRectIntersecting_ShouldReturnTrue()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 1.0f;

        var result = _camera.IsVisible(new RectF(-50, -50, 100, 100));

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsVisible_Rect_WithRectNotIntersecting_ShouldReturnFalse()
    {
        _camera.Position = new Position(0, 0);
        _camera.Zoom = 1.0f;

        var result = _camera.IsVisible(new RectF(1000, 1000, 100, 100));

        Assert.That(result, Is.False);
    }

    [Test]
    public void ViewBounds_ShouldBeCached_WhenTransformNotChanged()
    {
        _camera.Position = new Position(100, 100);
        
        var bounds1 = _camera.ViewBounds;
        var bounds2 = _camera.ViewBounds;

        Assert.That(bounds1.Equals(bounds2), Is.True);
    }

    [Test]
    public void ViewBounds_ShouldRecalculate_WhenTransformChanged()
    {
        _camera.Position = new Position(100, 100);
        var bounds1 = _camera.ViewBounds;

        _camera.Position = new Position(200, 200);
        var bounds2 = _camera.ViewBounds;

        Assert.That(bounds1.X, Is.Not.EqualTo(bounds2.X));
        Assert.That(bounds1.Y, Is.Not.EqualTo(bounds2.Y));
    }
}