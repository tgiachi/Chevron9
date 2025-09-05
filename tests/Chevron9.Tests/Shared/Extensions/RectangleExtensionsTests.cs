using Chevron9.Shared.Graphics;
using Chevron9.Shared.Extensions;
using Chevron9.Shared.Primitives;

namespace Chevron9.Tests.Shared.Extensions;

[TestFixture]
public class RectangleExtensionsTests
{
    [Test]
    public void ToRectF_ShouldConvertIntegerToFloat()
    {
        var rectI = new RectI(10, 20, 30, 40);
        
        var rectF = rectI.ToRectF();
        
        Assert.That(rectF.X, Is.EqualTo(10f));
        Assert.That(rectF.Y, Is.EqualTo(20f));
        Assert.That(rectF.Width, Is.EqualTo(30f));
        Assert.That(rectF.Height, Is.EqualTo(40f));
    }

    [Test]
    public void ToRectI_ShouldRoundFloatToInteger()
    {
        var rectF = new RectF(10.6f, 20.3f, 30.7f, 40.1f);
        
        var rectI = rectF.ToRectI();
        
        Assert.That(rectI.X, Is.EqualTo(11));
        Assert.That(rectI.Y, Is.EqualTo(20));
        Assert.That(rectI.Width, Is.EqualTo(31));
        Assert.That(rectI.Height, Is.EqualTo(40));
    }

    [Test]
    public void ToRectIFloor_ShouldFloorFloatToInteger()
    {
        var rectF = new RectF(10.9f, 20.9f, 30.9f, 40.9f);
        
        var rectI = rectF.ToRectIFloor();
        
        Assert.That(rectI.X, Is.EqualTo(10));
        Assert.That(rectI.Y, Is.EqualTo(20));
        Assert.That(rectI.Width, Is.EqualTo(30));
        Assert.That(rectI.Height, Is.EqualTo(40));
    }

    [Test]
    public void ToRectICeiling_ShouldCeilFloatToInteger()
    {
        var rectF = new RectF(10.1f, 20.1f, 30.1f, 40.1f);
        
        var rectI = rectF.ToRectICeiling();
        
        Assert.That(rectI.X, Is.EqualTo(11));
        Assert.That(rectI.Y, Is.EqualTo(21));
        Assert.That(rectI.Width, Is.EqualTo(31));
        Assert.That(rectI.Height, Is.EqualTo(41));
    }

    [Test]
    public void ToRectITruncate_ShouldTruncateFloatToInteger()
    {
        var rectF = new RectF(10.9f, 20.9f, 30.9f, 40.9f);
        
        var rectI = rectF.ToRectITruncate();
        
        Assert.That(rectI.X, Is.EqualTo(10));
        Assert.That(rectI.Y, Is.EqualTo(20));
        Assert.That(rectI.Width, Is.EqualTo(30));
        Assert.That(rectI.Height, Is.EqualTo(40));
    }

    [Test]
    public void GetCorners_RectangleF_ShouldReturnCorrectCorners()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        
        var corners = rect.GetCorners();
        
        Assert.That(corners.TopLeft.X, Is.EqualTo(10f));
        Assert.That(corners.TopLeft.Y, Is.EqualTo(20f));
        Assert.That(corners.TopRight.X, Is.EqualTo(40f));
        Assert.That(corners.TopRight.Y, Is.EqualTo(20f));
        Assert.That(corners.BottomLeft.X, Is.EqualTo(10f));
        Assert.That(corners.BottomLeft.Y, Is.EqualTo(60f));
        Assert.That(corners.BottomRight.X, Is.EqualTo(40f));
        Assert.That(corners.BottomRight.Y, Is.EqualTo(60f));
    }

    [Test]
    public void GetCorners_RectangleI_ShouldReturnCorrectCorners()
    {
        var rect = new RectI(10, 20, 30, 40);
        
        var corners = rect.GetCorners();
        
        Assert.That(corners.TopLeft.X, Is.EqualTo(10));
        Assert.That(corners.TopLeft.Y, Is.EqualTo(20));
        Assert.That(corners.TopRight.X, Is.EqualTo(40));
        Assert.That(corners.TopRight.Y, Is.EqualTo(20));
        Assert.That(corners.BottomLeft.X, Is.EqualTo(10));
        Assert.That(corners.BottomLeft.Y, Is.EqualTo(60));
        Assert.That(corners.BottomRight.X, Is.EqualTo(40));
        Assert.That(corners.BottomRight.Y, Is.EqualTo(60));
    }

    [Test]
    public void Scale_WithSingleFactor_ShouldScaleAllDimensions()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        
        var scaled = rect.Scale(2f);
        
        Assert.That(scaled.X, Is.EqualTo(20f));
        Assert.That(scaled.Y, Is.EqualTo(40f));
        Assert.That(scaled.Width, Is.EqualTo(60f));
        Assert.That(scaled.Height, Is.EqualTo(80f));
    }

    [Test]
    public void Scale_WithDifferentFactors_ShouldScaleIndependently()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        
        var scaled = rect.Scale(2f, 3f);
        
        Assert.That(scaled.X, Is.EqualTo(20f));
        Assert.That(scaled.Y, Is.EqualTo(60f));
        Assert.That(scaled.Width, Is.EqualTo(60f));
        Assert.That(scaled.Height, Is.EqualTo(120f));
    }

    [Test]
    public void Scale_WithPosition_ShouldScaleByPositionComponents()
    {
        var rect = new RectF(10f, 20f, 30f, 40f);
        var scale = new Position(2f, 3f);
        
        var scaled = rect.Scale(scale);
        
        Assert.That(scaled.X, Is.EqualTo(20f));
        Assert.That(scaled.Y, Is.EqualTo(60f));
        Assert.That(scaled.Width, Is.EqualTo(60f));
        Assert.That(scaled.Height, Is.EqualTo(120f));
    }

    [Test]
    public void FitWithin_ShouldPreserveAspectRatio()
    {
        var container = new RectF(0f, 0f, 100f, 50f);
        var content = new RectF(0f, 0f, 200f, 100f); // 2:1 aspect ratio
        
        var fitted = container.FitWithin(content);
        
        Assert.That(fitted.Width, Is.EqualTo(100f));
        Assert.That(fitted.Height, Is.EqualTo(50f));
        Assert.That(fitted.X, Is.EqualTo(0f));
        Assert.That(fitted.Y, Is.EqualTo(0f));
    }

    [Test]
    public void FitWithin_WithTallContent_ShouldFitByHeight()
    {
        var container = new RectF(0f, 0f, 100f, 50f);
        var content = new RectF(0f, 0f, 100f, 200f); // 1:2 aspect ratio (tall)
        
        var fitted = container.FitWithin(content);
        
        Assert.That(fitted.Height, Is.EqualTo(50f));
        Assert.That(fitted.Width, Is.EqualTo(25f));
        Assert.That(fitted.X, Is.EqualTo(37.5f)); // Centered horizontally
        Assert.That(fitted.Y, Is.EqualTo(0f));
    }

    [Test]
    public void FitWithin_WithEmptyContainer_ShouldReturnEmpty()
    {
        var container = RectF.Empty;
        var content = new RectF(0f, 0f, 100f, 50f);
        
        var fitted = container.FitWithin(content);
        
        Assert.That(fitted, Is.EqualTo(RectF.Empty));
    }

    [Test]
    public void FillWithin_ShouldFillEntireContainer()
    {
        var container = new RectF(0f, 0f, 100f, 50f);
        var content = new RectF(0f, 0f, 200f, 100f); // 2:1 aspect ratio
        
        var filled = container.FillWithin(content);
        
        Assert.That(filled.Height, Is.EqualTo(50f));
        Assert.That(filled.Width, Is.EqualTo(100f));
        // Content extends beyond container bounds to maintain aspect ratio
    }

    [Test]
    public void ClampToBounds_RectangleF_ShouldClampPosition()
    {
        var rect = new RectF(150f, 75f, 50f, 25f);
        var bounds = new RectF(0f, 0f, 100f, 50f);
        
        var clamped = rect.ClampToBounds(bounds);
        
        Assert.That(clamped.X, Is.EqualTo(50f)); // 100 - 50 (width)
        Assert.That(clamped.Y, Is.EqualTo(25f)); // 50 - 25 (height)
        Assert.That(clamped.Width, Is.EqualTo(50f));
        Assert.That(clamped.Height, Is.EqualTo(25f));
    }

    [Test]
    public void ClampToBounds_RectangleI_ShouldClampPosition()
    {
        var rect = new RectI(150, 75, 50, 25);
        var bounds = new RectI(0, 0, 100, 50);
        
        var clamped = rect.ClampToBounds(bounds);
        
        Assert.That(clamped.X, Is.EqualTo(50)); // 100 - 50 (width)
        Assert.That(clamped.Y, Is.EqualTo(25)); // 50 - 25 (height)
        Assert.That(clamped.Width, Is.EqualTo(50));
        Assert.That(clamped.Height, Is.EqualTo(25));
    }

    [Test]
    public void ClampToBounds_WithOversizedRectangle_ShouldClampSize()
    {
        var rect = new RectF(10f, 10f, 150f, 100f);
        var bounds = new RectF(0f, 0f, 100f, 50f);
        
        var clamped = rect.ClampToBounds(bounds);
        
        Assert.That(clamped.X, Is.EqualTo(0f));
        Assert.That(clamped.Y, Is.EqualTo(0f));
        Assert.That(clamped.Width, Is.EqualTo(100f)); // Clamped to bounds width
        Assert.That(clamped.Height, Is.EqualTo(50f)); // Clamped to bounds height
    }

    [Test]
    public void RectangleRoundTrip_ShouldMaintainValues()
    {
        var originalI = new RectI(10, 20, 30, 40);
        
        var convertedF = originalI.ToRectF();
        var convertedBackI = convertedF.ToRectI();
        
        Assert.That(convertedBackI, Is.EqualTo(originalI));
    }

    [Test]
    public void ConversionMethods_WithNegativeValues_ShouldHandleCorrectly()
    {
        var rectF = new RectF(-10.7f, -20.3f, 30.1f, 40.9f);
        
        var floor = rectF.ToRectIFloor();
        var ceiling = rectF.ToRectICeiling();
        var truncate = rectF.ToRectITruncate();
        var round = rectF.ToRectI();
        
        Assert.That(floor.X, Is.EqualTo(-11));
        Assert.That(ceiling.X, Is.EqualTo(-10));
        Assert.That(truncate.X, Is.EqualTo(-10));
        Assert.That(round.X, Is.EqualTo(-11));
    }
}