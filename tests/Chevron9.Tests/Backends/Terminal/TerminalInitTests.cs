using Chevron9.Backends.Terminal.Utils;

namespace Chevron9.Tests.Backends.Terminal;

[TestFixture]
public class TerminalInitTests
{
    [Test]
    public void Initialize_ShouldReturnCompleteSetupSequence()
    {
        var sequence = TerminalInit.Initialize();

        // Should contain all the essential setup sequences
        Assert.That(sequence, Contains.Substring("\u001b[?1049h")); // Enter alternate screen
        Assert.That(sequence, Contains.Substring("\u001b[?25l"));   // Hide cursor
        Assert.That(sequence, Contains.Substring("\u001b[2J"));     // Clear screen
        Assert.That(sequence, Contains.Substring("\u001b[H"));      // Cursor home
        Assert.That(sequence, Contains.Substring("\u001b[?1000h")); // Enable mouse tracking
        Assert.That(sequence, Contains.Substring("\u001b[?2004h")); // Enable bracketed paste
        Assert.That(sequence, Contains.Substring("\u001b[?1004h")); // Enable focus reporting
    }

    [Test]
    public void Cleanup_ShouldReturnCompleteCleanupSequence()
    {
        var sequence = TerminalInit.Cleanup();

        // Should contain all the essential cleanup sequences
        Assert.That(sequence, Contains.Substring("\u001b[?1049l")); // Exit alternate screen
        Assert.That(sequence, Contains.Substring("\u001b[?25h"));   // Show cursor
        Assert.That(sequence, Contains.Substring("\u001b[?1006l")); // Disable mouse tracking
        Assert.That(sequence, Contains.Substring("\u001b[?2004l")); // Disable bracketed paste
        Assert.That(sequence, Contains.Substring("\u001b[?1004l")); // Disable focus reporting
        Assert.That(sequence, Contains.Substring("\u001b[0m"));     // Reset formatting
    }

    [Test]
    public void SetupTerminal_WithDefaultTitle_ShouldIncludeTitle()
    {
        var sequence = TerminalInit.SetupTerminal();

        Assert.That(sequence, Contains.Substring("\u001b]0;Chevron9 Game\u0007"));
    }

    [Test]
    public void SetupTerminal_WithCustomTitle_ShouldIncludeCustomTitle()
    {
        var sequence = TerminalInit.SetupTerminal("My Custom Game");

        Assert.That(sequence, Contains.Substring("\u001b]0;My Custom Game\u0007"));
    }

    [Test]
    public void GetTerminalSize_ShouldReturnSizeQuerySequence()
    {
        var sequence = TerminalInit.GetTerminalSize();

        Assert.That(sequence, Is.EqualTo("\u001b[14t"));
    }

    [Test]
    public void SetupHighPerformance_ShouldIncludeColorTests()
    {
        var sequence = TerminalInit.SetupHighPerformance();

        // Should include color capability tests
        Assert.That(sequence, Contains.Substring("\u001b[38;5;0m")); // 256-color test
        Assert.That(sequence, Contains.Substring("\u001b[38;2;0;0;0m")); // True color test
    }

    [Test]
    public void IsAnsiSupported_WithTermVariable_ShouldReturnTrue()
    {
        // Set a common TERM value
        Environment.SetEnvironmentVariable("TERM", "xterm-256color");

        try
        {
            var result = TerminalInit.IsAnsiSupported();
            Assert.That(result, Is.True);
        }
        finally
        {
            Environment.SetEnvironmentVariable("TERM", null);
        }
    }

    [Test]
    public void IsAnsiSupported_WithoutTermVariable_ShouldReturnFalse()
    {
        // Ensure TERM is not set
        Environment.SetEnvironmentVariable("TERM", null);

        var result = TerminalInit.IsAnsiSupported();
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsAnsiSupported_WithUnsupportedTerm_ShouldReturnFalse()
    {
        Environment.SetEnvironmentVariable("TERM", "dumb");

        try
        {
            var result = TerminalInit.IsAnsiSupported();
            Assert.That(result, Is.False);
        }
        finally
        {
            Environment.SetEnvironmentVariable("TERM", null);
        }
    }

    [Test]
    public void GetTerminalType_ShouldReturnTermEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable("TERM", "screen-256color");

        try
        {
            var result = TerminalInit.GetTerminalType();
            Assert.That(result, Is.EqualTo("screen-256color"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("TERM", null);
        }
    }

    [Test]
    public void GetTerminalType_WithoutTermVariable_ShouldReturnUnknown()
    {
        Environment.SetEnvironmentVariable("TERM", null);

        var result = TerminalInit.GetTerminalType();
        Assert.That(result, Is.EqualTo("unknown"));
    }

    [Test]
    public void Supports256Colors_With256ColorTerm_ShouldReturnTrue()
    {
        Environment.SetEnvironmentVariable("TERM", "xterm-256color");

        try
        {
            var result = TerminalInit.Supports256Colors();
            Assert.That(result, Is.True);
        }
        finally
        {
            Environment.SetEnvironmentVariable("TERM", null);
        }
    }

    [Test]
    public void Supports256Colors_WithColorTerm_ShouldReturnTrue()
    {
        Environment.SetEnvironmentVariable("COLORTERM", "256color");

        try
        {
            var result = TerminalInit.Supports256Colors();
            Assert.That(result, Is.True);
        }
        finally
        {
            Environment.SetEnvironmentVariable("COLORTERM", null);
        }
    }

    [Test]
    public void Supports256Colors_Without256ColorSupport_ShouldReturnFalse()
    {
        Environment.SetEnvironmentVariable("TERM", "xterm");
        Environment.SetEnvironmentVariable("COLORTERM", null);

        try
        {
            var result = TerminalInit.Supports256Colors();
            Assert.That(result, Is.False);
        }
        finally
        {
            Environment.SetEnvironmentVariable("TERM", null);
        }
    }

    [Test]
    public void SupportsTrueColor_WithTrueColorTerm_ShouldReturnTrue()
    {
        Environment.SetEnvironmentVariable("COLORTERM", "truecolor");

        try
        {
            var result = TerminalInit.SupportsTrueColor();
            Assert.That(result, Is.True);
        }
        finally
        {
            Environment.SetEnvironmentVariable("COLORTERM", null);
        }
    }

    [Test]
    public void SupportsTrueColor_With24BitTerm_ShouldReturnTrue()
    {
        Environment.SetEnvironmentVariable("COLORTERM", "24bit");

        try
        {
            var result = TerminalInit.SupportsTrueColor();
            Assert.That(result, Is.True);
        }
        finally
        {
            Environment.SetEnvironmentVariable("COLORTERM", null);
        }
    }

    [Test]
    public void SupportsTrueColor_WithoutTrueColorSupport_ShouldReturnFalse()
    {
        Environment.SetEnvironmentVariable("COLORTERM", "256color");

        try
        {
            var result = TerminalInit.SupportsTrueColor();
            Assert.That(result, Is.False);
        }
        finally
        {
            Environment.SetEnvironmentVariable("COLORTERM", null);
        }
    }

    [Test]
    public void GetPlatformInitSequence_ShouldIncludePlatformSpecificOptimizations()
    {
        var sequence = TerminalInit.GetPlatformInitSequence();

        // Should include the base initialization
        Assert.That(sequence, Contains.Substring("\u001b[?1049h")); // Enter alternate screen
        Assert.That(sequence, Contains.Substring("\u001b[?25l"));   // Hide cursor
    }
}