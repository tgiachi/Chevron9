using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Shared.Graphics;

[TestFixture]
public class RectITests
{
    [Test]
    public void Constructor_WithXYWidthHeight_ShouldSetProperties()
    {
        var rect = new RectI(10, 20, 30, 40);

        Assert.That(rect.X, Is.EqualTo(10));
        Assert.That(rect.Y, Is.EqualTo(20));
        Assert.That(rect.Width, Is.EqualTo(30));
        Assert.That(rect.Height, Is.EqualTo(40));
    }

    [Test]
    public void Empty_ShouldReturnEmptyRectangle()
    {
        var empty = RectI.Empty;

        Assert.That(empty.X, Is.EqualTo(0));
        Assert.That(empty.Y, Is.EqualTo(0));
        Assert.That(empty.Width, Is.EqualTo(0));
        Assert.That(empty.Height, Is.EqualTo(0));
    }

    [Test]
    public void EdgeProperties_ShouldReturnCorrectValues()
    {
        var rect = new RectI(10, 20, 30, 40);

        Assert.That(rect.Left, Is.EqualTo(10));
        Assert.That(rect.Right, Is.EqualTo(40));
        Assert.That(rect.Top, Is.EqualTo(20));
        Assert.That(rect.Bottom, Is.EqualTo(60));
    }

    [Test]
    public void Area_ShouldCalculateCorrectArea()
    {
        var rect = new RectI(10, 20, 30, 40);

        Assert.That(rect.Area, Is.EqualTo(1200));
    }

    [Test]
    public void IsEmpty_WithZeroWidth_ShouldReturnTrue()
    {
        var rect = new RectI(10, 20, 0, 40);

        Assert.That(rect.IsEmpty, Is.True);
    }

    [Test]
    public void IsEmpty_WithValidDimensions_ShouldReturnFalse()
    {
        var rect = new RectI(10, 20, 30, 40);

        Assert.That(rect.IsEmpty, Is.False);
    }

    [Test]
    public void Contains_WithPointInside_ShouldReturnTrue()
    {
        var rect = new RectI(10, 20, 30, 40);

        Assert.That(rect.Contains(25, 35), Is.True);
        Assert.That(rect.Contains((25, 35)), Is.True);
    }

    [Test]
    public void Contains_WithPointOutside_ShouldReturnFalse()
    {
        var rect = new RectI(10, 20, 30, 40);

        Assert.That(rect.Contains(5, 35), Is.False);
        Assert.That(rect.Contains(45, 35), Is.False);
    }

    [Test]
    public void Intersects_WithOverlappingRectangles_ShouldReturnTrue()
    {
        var rect1 = new RectI(10, 20, 30, 40);
        var rect2 = new RectI(20, 30, 30, 40);

        Assert.That(rect1.Intersects(rect2), Is.True);
        Assert.That(rect2.Intersects(rect1), Is.True);
    }

    [Test]
    public void Intersects_WithNonOverlappingRectangles_ShouldReturnFalse()
    {
        var rect1 = new RectI(10, 20, 20, 30);
        var rect2 = new RectI(40, 60, 20, 30);

        Assert.That(rect1.Intersects(rect2), Is.False);
        Assert.That(rect2.Intersects(rect1), Is.False);
    }

    [Test]
    public void Intersect_WithOverlappingRectangles_ShouldReturnIntersection()
    {
        var rect1 = new RectI(10, 20, 30, 40);
        var rect2 = new RectI(20, 30, 30, 40);

        var intersection = rect1.Intersect(rect2);

        Assert.That(intersection.X, Is.EqualTo(20));
        Assert.That(intersection.Y, Is.EqualTo(30));
        Assert.That(intersection.Width, Is.EqualTo(20));
        Assert.That(intersection.Height, Is.EqualTo(30));
    }

    [Test]
    public void Intersect_WithNonOverlappingRectangles_ShouldReturnEmpty()
    {
        var rect1 = new RectI(10, 20, 20, 30);
        var rect2 = new RectI(40, 60, 20, 30);

        var intersection = rect1.Intersect(rect2);

        Assert.That(intersection, Is.EqualTo(RectI.Empty));
    }

    [Test]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var rect1 = new RectI(10, 20, 30, 40);
        var rect2 = new RectI(10, 20, 30, 40);

        Assert.That(rect1.Equals(rect2), Is.True);
        Assert.That(rect1 == rect2, Is.True);
        Assert.That(rect1 != rect2, Is.False);
    }

    [Test]
    public void ToString_ShouldReturnCorrectFormat()
    {
        var rect = new RectI(10, 20, 30, 40);

        Assert.That(rect.ToString(), Is.EqualTo("RectangleI(X:10, Y:20, W:30, H:40)"));
    }
}
