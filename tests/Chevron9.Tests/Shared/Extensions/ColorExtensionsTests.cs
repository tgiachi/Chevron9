using Chevron9.Shared.Extensions;
using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Shared.Extensions;

[TestFixture]
public class ColorExtensionsTests
{
    [Test]
    public void Lerp_ShouldInterpolateColors()
    {
        var from = new Color(0, 0, 0, 255);
        var to = new Color(255, 255, 255, 255);

        var result = from.Lerp(to, 0.5f);

        Assert.That(result.R, Is.EqualTo(128));
        Assert.That(result.G, Is.EqualTo(128));
        Assert.That(result.B, Is.EqualTo(128));
        Assert.That(result.A, Is.EqualTo(255));
    }

    [Test]
    public void Lerp_WithClampedT_ShouldClampCorrectly()
    {
        var from = new Color(100, 100, 100, 255);
        var to = new Color(200, 200, 200, 255);

        var lowResult = from.Lerp(to, -0.5f);
        var highResult = from.Lerp(to, 1.5f);

        Assert.That(lowResult, Is.EqualTo(from));
        Assert.That(highResult, Is.EqualTo(to));
    }

    [Test]
    public void BlendColors_WithSingleColor_ShouldReturnThatColor()
    {
        var colors = new[] { new Color(255, 128, 64, 200) };

        var result = colors.BlendColors();

        Assert.That(result, Is.EqualTo(colors[0]));
    }

    [Test]
    public void BlendColors_WithMultipleColors_ShouldReturnAverage()
    {
        var colors = new[]
        {
            new Color(255, 0, 0, 255), // Red
            new Color(0, 255, 0, 255), // Green
            new Color(0, 0, 255, 255)  // Blue
        };

        var result = colors.BlendColors();

        Assert.That(result.R, Is.EqualTo(85));
        Assert.That(result.G, Is.EqualTo(85));
        Assert.That(result.B, Is.EqualTo(85));
        Assert.That(result.A, Is.EqualTo(255));
    }

    [Test]
    public void BlendColors_WithEmptyArray_ShouldThrowArgumentException()
    {
        var emptyColors = Array.Empty<Color>();

        Assert.Throws<ArgumentException>(() => emptyColors.BlendColors());
    }

    [Test]
    public void CreateLinearGradient_ShouldCreateCorrectGradient()
    {
        var from = Color.Black;
        var to = Color.White;
        const int steps = 5;

        var gradient = from.CreateLinearGradient(to, steps);

        Assert.That(gradient.Length, Is.EqualTo(steps));
        Assert.That(gradient[0], Is.EqualTo(from));
        Assert.That(gradient[^1], Is.EqualTo(to));
    }

    [Test]
    public void CreateLinearGradient_WithLessThanTwoSteps_ShouldThrowArgumentException()
    {
        var from = Color.Black;
        var to = Color.White;

        Assert.Throws<ArgumentException>(() => from.CreateLinearGradient(to, 1));
    }

    [Test]
    public void Darken_ShouldDarkenColor()
    {
        var color = new Color(200, 150, 100, 255);

        var darkened = color.Darken(0.5f);

        Assert.That(darkened.R, Is.EqualTo(100));
        Assert.That(darkened.G, Is.EqualTo(75));
        Assert.That(darkened.B, Is.EqualTo(50));
        Assert.That(darkened.A, Is.EqualTo(255));
    }

    [Test]
    public void ToGrayscale_ShouldConvertToGrayscale()
    {
        var color = new Color(255, 0, 0, 255); // Pure red

        var grayscale = color.ToGrayscale();

        Assert.That(grayscale.R, Is.EqualTo(grayscale.G));
        Assert.That(grayscale.G, Is.EqualTo(grayscale.B));
        Assert.That(grayscale.A, Is.EqualTo(255));
    }

    [Test]
    public void Invert_WithoutAlpha_ShouldInvertRGB()
    {
        var color = new Color(100, 150, 200, 128);

        var inverted = color.Invert();

        Assert.That(inverted.R, Is.EqualTo(155));
        Assert.That(inverted.G, Is.EqualTo(105));
        Assert.That(inverted.B, Is.EqualTo(55));
        Assert.That(inverted.A, Is.EqualTo(128));
    }

    [Test]
    public void GetLuminance_ShouldCalculateCorrectLuminance()
    {
        var white = Color.White;
        var black = Color.Black;

        Assert.That(white.GetLuminance(), Is.EqualTo(1f));
        Assert.That(black.GetLuminance(), Is.EqualTo(0f));
    }

    [Test]
    public void IsLight_WithLightColor_ShouldReturnTrue()
    {
        Assert.That(Color.White.IsLight(), Is.True);
        Assert.That(new Color(200, 200, 200, 255).IsLight(), Is.True);
    }

    [Test]
    public void IsDark_WithDarkColor_ShouldReturnTrue()
    {
        Assert.That(Color.Black.IsDark(), Is.True);
        Assert.That(new Color(50, 50, 50, 255).IsDark(), Is.True);
    }

    [Test]
    public void GetContrastRatio_ShouldCalculateCorrectRatio()
    {
        var white = Color.White;
        var black = Color.Black;

        var ratio = white.GetContrastRatio(black);

        Assert.That(ratio, Is.EqualTo(21f).Within(0.01f));
    }
}
