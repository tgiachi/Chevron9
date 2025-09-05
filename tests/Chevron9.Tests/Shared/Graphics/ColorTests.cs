using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Shared.Graphics;

[TestFixture]
public class ColorTests
{
    [Test]
    public void Constructor_WithRGBA_ShouldSetProperties()
    {
        var color = new Color(255, 128, 64, 200);
        
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(128));
        Assert.That(color.B, Is.EqualTo(64));
        Assert.That(color.A, Is.EqualTo(200));
    }

    [Test]
    public void Constructor_WithRGB_ShouldSetAlphaTo255()
    {
        var color = new Color(255, 128, 64);
        
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(128));
        Assert.That(color.B, Is.EqualTo(64));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void FromFloat_WithValidValues_ShouldConvertCorrectly()
    {
        var color = Color.FromFloat(1.0f, 0.5f, 0.25f, 0.8f);

        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(127));
        Assert.That(color.B, Is.EqualTo(63));
        Assert.That(color.A, Is.EqualTo(204));
    }

    [Test]
    public void FromFloat_WithClampedValues_ShouldClampCorrectly()
    {
        var color = Color.FromFloat(1.5f, -0.5f, 0.5f, 2.0f);
        
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(0));
        Assert.That(color.B, Is.EqualTo(127));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void FromHex_WithRGB_ShouldParseCorrectly()
    {
        var color = Color.FromHex("#F80");
        
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(136));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void FromHex_WithRRGGBB_ShouldParseCorrectly()
    {
        var color = Color.FromHex("#FF8800");
        
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(136));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void FromHex_WithoutHashPrefix_ShouldParseCorrectly()
    {
        var color = Color.FromHex("FF8800");
        
        Assert.That(color.R, Is.EqualTo(255));
        Assert.That(color.G, Is.EqualTo(136));
        Assert.That(color.B, Is.EqualTo(0));
        Assert.That(color.A, Is.EqualTo(255));
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("   ")]
    public void FromHex_WithInvalidInput_ShouldThrowArgumentException(string? invalidHex)
    {
        Assert.Throws<ArgumentException>(() => Color.FromHex(invalidHex));
    }

    [TestCase("#GGG")]
    [TestCase("#12")]
    [TestCase("#12345")]
    [TestCase("#123456789")]
    public void FromHex_WithInvalidLength_ShouldThrowArgumentException(string invalidHex)
    {
        Assert.Throws<ArgumentException>(() => Color.FromHex(invalidHex));
    }

    [Test]
    public void ToHex_WithoutAlpha_ShouldReturnCorrectFormat()
    {
        var color = new Color(255, 136, 0);
        var hex = color.ToHex();
        
        Assert.That(hex, Is.EqualTo("#FF8800"));
    }

    [Test]
    public void ToHex_WithAlpha_ShouldReturnCorrectFormat()
    {
        var color = new Color(255, 136, 0, 128);
        var hex = color.ToHex(true);
        
        Assert.That(hex, Is.EqualTo("#80FF8800"));
    }

    [Test]
    public void FloatProperties_ShouldReturnCorrectValues()
    {
        var color = new Color(255, 128, 64, 200);
        
        Assert.That(color.RedFloat, Is.EqualTo(1.0f).Within(0.01f));
        Assert.That(color.GreenFloat, Is.EqualTo(0.502f).Within(0.01f));
        Assert.That(color.BlueFloat, Is.EqualTo(0.251f).Within(0.01f));
        Assert.That(color.AlphaFloat, Is.EqualTo(0.784f).Within(0.01f));
    }

    [Test]
    public void WithAlpha_WithByte_ShouldReturnNewColorWithAlpha()
    {
        var color = new Color(255, 128, 64, 255);
        var newColor = color.WithAlpha(128);
        
        Assert.That(newColor.R, Is.EqualTo(255));
        Assert.That(newColor.G, Is.EqualTo(128));
        Assert.That(newColor.B, Is.EqualTo(64));
        Assert.That(newColor.A, Is.EqualTo(128));
        
        Assert.That(color.A, Is.EqualTo(255));
    }

    [Test]
    public void WithAlpha_WithFloat_ShouldReturnNewColorWithAlpha()
    {
        var color = new Color(255, 128, 64, 255);
        var newColor = color.WithAlpha(0.5f);

        Assert.That(newColor.A, Is.EqualTo(127));
    }

    [Test]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var color1 = new Color(255, 128, 64, 200);
        var color2 = new Color(255, 128, 64, 200);
        
        Assert.That(color1.Equals(color2), Is.True);
        Assert.That(color1 == color2, Is.True);
        Assert.That(color1 != color2, Is.False);
    }

    [Test]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        var color1 = new Color(255, 128, 64, 200);
        var color2 = new Color(255, 128, 64, 199);
        
        Assert.That(color1.Equals(color2), Is.False);
        Assert.That(color1 == color2, Is.False);
        Assert.That(color1 != color2, Is.True);
    }

    [Test]
    public void GetHashCode_WithSameValues_ShouldReturnSameHash()
    {
        var color1 = new Color(255, 128, 64, 200);
        var color2 = new Color(255, 128, 64, 200);
        
        Assert.That(color1.GetHashCode(), Is.EqualTo(color2.GetHashCode()));
    }

    [Test]
    public void ToString_ShouldReturnCorrectFormat()
    {
        var color = new Color(255, 128, 64, 200);
        var str = color.ToString();
        
        Assert.That(str, Is.EqualTo("Color(R:255, G:128, B:64, A:200)"));
    }

    [Test]
    public void CommonColors_ShouldHaveCorrectValues()
    {
        Assert.That(Color.Black, Is.EqualTo(new Color(0, 0, 0, 255)));
        Assert.That(Color.White, Is.EqualTo(new Color(255, 255, 255, 255)));
        Assert.That(Color.Red, Is.EqualTo(new Color(255, 0, 0, 255)));
        Assert.That(Color.Green, Is.EqualTo(new Color(0, 255, 0, 255)));
        Assert.That(Color.Blue, Is.EqualTo(new Color(0, 0, 255, 255)));
        Assert.That(Color.Transparent, Is.EqualTo(new Color(0, 0, 0, 0)));
    }
}