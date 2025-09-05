using Chevron9.Core.Args;
using Chevron9.Core.Types;

namespace Chevron9.Tests.Core.Types;

[TestFixture]
public class ResizeEventArgsTests
{
    [Test]
    public void Constructor_WithValidDimensions_SetsPropertiesCorrectly()
    {
        const int expectedWidth = 800;
        const int expectedHeight = 600;

        var args = new ResizeEventArgs(expectedWidth, expectedHeight);

        Assert.That(args.Width, Is.EqualTo(expectedWidth));
        Assert.That(args.Height, Is.EqualTo(expectedHeight));
    }

    [Test]
    public void Constructor_WithZeroDimensions_SetsPropertiesCorrectly()
    {
        const int expectedWidth = 0;
        const int expectedHeight = 0;

        var args = new ResizeEventArgs(expectedWidth, expectedHeight);

        Assert.That(args.Width, Is.EqualTo(expectedWidth));
        Assert.That(args.Height, Is.EqualTo(expectedHeight));
    }

    [Test]
    public void Constructor_WithNegativeDimensions_SetsPropertiesCorrectly()
    {
        const int expectedWidth = -100;
        const int expectedHeight = -50;

        var args = new ResizeEventArgs(expectedWidth, expectedHeight);

        Assert.That(args.Width, Is.EqualTo(expectedWidth));
        Assert.That(args.Height, Is.EqualTo(expectedHeight));
    }
}
