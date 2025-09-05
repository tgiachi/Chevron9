using Chevron9.Shared.Graphics;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Shared.Graphics;

[TestFixture]
public class RectFTests
{
    [Test]
    public void Constructor_WithXYWidthHeight_ShouldSetProperties()
    {
        var rect = new RectF(10.5f, 20.3f, 30.7f, 40.1f);
        
        Assert.That(rect.X, Is.EqualTo(10.5f));
        Assert.That(rect.Y, Is.EqualTo(20.3f));
        Assert.That(rect.Width, Is.EqualTo(30.7f));
        Assert.That(rect.Height, Is.EqualTo(40.1f));
    }

    [Test]
    public void Constructor_WithPositionAndSize_ShouldSetProperties()
    {
        var position = new Position(10.5f, 20.3f);
        var rect = new RectF(position, (30.7f, 40.1f));
        
        Assert.That(rect.X, Is.EqualTo(10.5f));
        Assert.That(rect.Y, Is.EqualTo(20.3f));
        Assert.That(rect.Width, Is.EqualTo(30.7f));
        Assert.That(rect.Height, Is.EqualTo(40.1f));
    }

    [Test]
    public void Empty_ShouldReturnEmptyRectangle()
    {
        var empty = RectF.Empty;
        
        Assert.That(empty.X, Is.EqualTo(0f));
        Assert.That(empty.Y, Is.EqualTo(0f));
        Assert.That(empty.Width, Is.EqualTo(0f));
        Assert.That(empty.Height, Is.EqualTo(0f));
    }

    [Test]
    public void EdgeProperties_ShouldReturnCorrectValues()
    {
        var rect = new RectF(10.5f, 20.3f, 30.7f, 40.1f);
        
        Assert.That(rect.Left, Is.EqualTo(10.5f));
        Assert.That(rect.Right, Is.EqualTo(41.2f));
        Assert.That(rect.Top, Is.EqualTo(20.3f));
        Assert.That(rect.Bottom, Is.EqualTo(60.4f).Within(0.01f));
    }

    [Test]
    public void Position_ShouldReturnCorrectPosition()
    {
        var rect = new RectF(10.5f, 20.3f, 30.7f, 40.1f);
        
        Assert.That(rect.Position.X, Is.EqualTo(10.5f));
        Assert.That(rect.Position.Y, Is.EqualTo(20.3f));
    }

    [Test]
    public void Area_ShouldCalculateCorrectArea()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        
        Assert.That(rect.Area, Is.EqualTo(1200f));
    }

    [Test]
    public void IsEmpty_WithZeroWidth_ShouldReturnTrue()
    {
        var rect = new RectF(10f, 20f, 0f, 40f);
        
        Assert.That(rect.IsEmpty, Is.True);
    }

    [Test]
    public void IsEmpty_WithValidDimensions_ShouldReturnFalse()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        
        Assert.That(rect.IsEmpty, Is.False);
    }

    [Test]
    public void Contains_WithFloatPointInside_ShouldReturnTrue()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        
        Assert.That(rect.Contains(25.5f, 35.7f), Is.True);
    }

    [Test]
    public void Contains_WithPositionInside_ShouldReturnTrue()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        var position = new Position(25.5f, 35.7f);
        
        Assert.That(rect.Contains(position), Is.True);
    }

    [Test]
    public void Intersects_WithRectangleF_ShouldWork()
    {
        var rect1 = new RectF(10f, 20f, 30f, 40f);
        var rect2 = new RectF(20f, 30f, 30f, 40f);
        
        Assert.That(rect1.Intersects(rect2), Is.True);
    }

    [Test]
    public void Intersect_WithOverlappingRectangles_ShouldReturnIntersection()
    {
        var rect1 = new RectF(10f, 20f, 30f, 40f);
        var rect2 = new RectF(20f, 30f, 30f, 40f);
        
        var intersection = rect1.Intersect(rect2);
        
        Assert.That(intersection.X, Is.EqualTo(20f));
        Assert.That(intersection.Y, Is.EqualTo(30f));
        Assert.That(intersection.Width, Is.EqualTo(20f));
        Assert.That(intersection.Height, Is.EqualTo(30f));
    }

    [Test]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var rect1 = new RectF(10.5f, 20.3f, 30.7f, 40.1f);
        var rect2 = new RectF(10.5f, 20.3f, 30.7f, 40.1f);
        
        Assert.That(rect1.Equals(rect2), Is.True);
        Assert.That(rect1 == rect2, Is.True);
        Assert.That(rect1 != rect2, Is.False);
    }

    [Test]
    public void ToString_ShouldReturnCorrectFormat()
    {
        var rect = new RectF(10.12f, 20.34f, 30.56f, 40.78f);
        
        Assert.That(rect.ToString(), Is.EqualTo("RectangleF(X:10.12, Y:20.34, W:30.56, H:40.78)"));
    }
}