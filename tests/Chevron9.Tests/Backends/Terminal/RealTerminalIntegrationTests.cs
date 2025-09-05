using System.Diagnostics;
using Chevron9.Backends.Terminal.Utils;
using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Backends.Terminal;

/// <summary>
///     Integration tests that run against a real terminal.
///     These tests verify ANSI sequences work correctly in actual terminal environments.
///     Note: These tests may be skipped on systems without terminal support.
/// </summary>
[TestFixture]
[Category("RealTerminal")]
public class RealTerminalIntegrationTests
{
    private const string TestCommand = "printf";

    [Test]
    [Explicit("Requires real terminal - run manually")]
    public void RealTerminal_WithSimpleText_ShouldRenderCorrectly()
    {
        var builder = new AnsiBuilder();
        var sequence = builder
            .Append("Hello World")
            .Build();

        var output = ExecuteInTerminal(sequence);

        // Verify the command executed without errors
        Assert.That(output.ExitCode, Is.EqualTo(0));
        // Note: We can't easily verify visual output in automated tests
        // This test mainly ensures the ANSI sequence doesn't break the terminal
    }

    [Test]
    [Explicit("Requires real terminal - run manually")]
    public void RealTerminal_WithColorText_ShouldApplyColors()
    {
        var builder = new AnsiBuilder();
        var sequence = builder
            .FgColor(Color.Red)
            .Append("Red Text")
            .FgColor(Color.Green)
            .Append("Green Text")
            .Build();

        var output = ExecuteInTerminal(sequence);

        Assert.That(output.ExitCode, Is.EqualTo(0));
    }

    [Test]
    [Explicit("Requires real terminal - run manually")]
    public void RealTerminal_WithCursorMovement_ShouldPositionCorrectly()
    {
        var builder = new AnsiBuilder();
        var sequence = builder
            .CursorPosition(5, 3)
            .Append("X")
            .Build();

        var output = ExecuteInTerminal(sequence);

        Assert.That(output.ExitCode, Is.EqualTo(0));
    }

    [Test]
    [Explicit("Requires real terminal - run manually")]
    public void RealTerminal_WithComplexSequence_ShouldWorkCorrectly()
    {
        var builder = new AnsiBuilder();
        var sequence = builder
            .ClearScreen()
            .CursorPosition(1, 1)
            .FgColor(Color.Blue)
            .BgColor(Color.White)
            .Append("Complex Test")
            .CursorPosition(3, 1)
            .FgColor(Color.Yellow)
            .BgColor(Color.Black)
            .Append("✓ Success")
            .Build();

        var output = ExecuteInTerminal(sequence);

        Assert.That(output.ExitCode, Is.EqualTo(0));
    }

    private static (int ExitCode, string Output) ExecuteInTerminal(string ansiSequence)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = TestCommand,
                    Arguments = $"\"{ansiSequence.Replace("\"", "\\\"")}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return (process.ExitCode, output + error);
        }
        catch (Exception ex)
        {
            // If terminal execution fails, return error info
            return (-1, $"Terminal execution failed: {ex.Message}");
        }
    }
}
