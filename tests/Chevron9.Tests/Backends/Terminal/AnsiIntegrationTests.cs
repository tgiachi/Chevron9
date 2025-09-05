using Chevron9.Backends.Terminal.Utils;
using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Backends.Terminal;

/// <summary>
/// Test di integrazione per verificare che i componenti ANSI funzionino insieme correttamente
/// </summary>
[TestFixture]
public class AnsiIntegrationTests
{
    [Test]
    public void AnsiBuilder_WithAnsiEscapeCodes_ShouldBuildCorrectSequences()
    {
        var builder = new AnsiBuilder();

        var result = builder
            .Append(AnsiEscapeCodes.ClearScreen)
            .Append(AnsiEscapeCodes.CursorHome)
            .Append(AnsiEscapeCodes.Red)
            .Append(AnsiEscapeCodes.BgBlack)
            .Append(AnsiEscapeCodes.Bold)
            .Append("Hello World")
            .Append(AnsiEscapeCodes.Reset)
            .Build();

        var expected = "\e[2J\e[H\e[31m\e[40m\e[1mHello World\e[0m";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ColoredText_WithAnsiBuilder_ShouldCreateStyledOutput()
    {
        var styledText = AnsiEscapeCodes.ColoredText("Test", AnsiEscapeCodes.Red, AnsiEscapeCodes.BgWhite, AnsiEscapeCodes.Bold);

        var builder = new AnsiBuilder();
        var result = builder
            .Append(AnsiEscapeCodes.ClearScreen)
            .Append(styledText)
            .Build();

        Assert.That(result, Contains.Substring("\e[2J"));
        Assert.That(result, Contains.Substring("\e[31m"));
        Assert.That(result, Contains.Substring("\e[47m"));
        Assert.That(result, Contains.Substring("\e[1m"));
        Assert.That(result, Contains.Substring("Test"));
        Assert.That(result, Contains.Substring("\e[0m"));
    }

    [Test]
    public void GradientText_WithColorConstants_ShouldApplyGradient()
    {
        var gradient = AnsiEscapeCodes.GradientText("ABC", 1, 3);

        Assert.That(gradient, Contains.Substring("\e[38;5;1mA"));
        Assert.That(gradient, Contains.Substring("\e[38;5;2mB"));
        Assert.That(gradient, Contains.Substring("\e[38;5;3mC"));
        Assert.That(gradient, Contains.Substring("\e[0m"));
    }

    [Test]
    public void ComplexTerminalInterface_ShouldBuildCorrectSequence()
    {
        var builder = new AnsiBuilder();

        var interfaceSequence = builder
            .Append(AnsiEscapeCodes.ClearScreen)
            .Append(AnsiEscapeCodes.CursorHide)
            .Append(AnsiEscapeCodes.EnterAlternateScreen)
            .CursorPosition(1, 1)
            .Append(AnsiEscapeCodes.BrightWhite)
            .Append(AnsiEscapeCodes.BgBlue)
            .Append(AnsiEscapeCodes.Bold)
            .Append("=== CHEVRON9 TERMINAL ===")
            .Reset()
             .CursorPosition(3, 1)
             .FgColor(Color.Green)
             .Append("✓ Terminal inizializzato")
             .CursorPosition(4, 1)
             .FgColor(Color.Yellow)
             .Append("⚠ Controllo colori...")
            .CursorPosition(6, 1)
            .Append(AnsiEscapeCodes.Reset)
            .Build();

        // Verifica che contenga tutte le sequenze necessarie
        Assert.That(interfaceSequence, Contains.Substring("\e[2J")); // Clear screen
        Assert.That(interfaceSequence, Contains.Substring("\e[?25l")); // Hide cursor
        Assert.That(interfaceSequence, Contains.Substring("\e[?1049h")); // Alternate screen
        Assert.That(interfaceSequence, Contains.Substring("\e[1;1H")); // Cursor position
        Assert.That(interfaceSequence, Contains.Substring("\e[97m")); // Bright white
        Assert.That(interfaceSequence, Contains.Substring("\e[44m")); // Blue background
        Assert.That(interfaceSequence, Contains.Substring("\e[1m")); // Bold
        Assert.That(interfaceSequence, Contains.Substring("=== CHEVRON9 TERMINAL ==="));
        Assert.That(interfaceSequence, Contains.Substring("\e[38;2;0;255;0m")); // Green (RGB)
        Assert.That(interfaceSequence, Contains.Substring("✓ Terminal inizializzato"));
        Assert.That(interfaceSequence, Contains.Substring("\e[38;2;255;255;0m")); // Yellow (RGB)
        Assert.That(interfaceSequence, Contains.Substring("⚠ Controllo colori..."));
    }

    [Test]
    public void Hyperlink_WithStyledText_ShouldCreateClickableLink()
    {
        var link = AnsiEscapeCodes.Hyperlink("https://github.com/sst/opencode", "OpenCode Repository");

        var builder = new AnsiBuilder();
        var result = builder
            .FgColor(Color.Blue)
            .Style(AnsiEscapeCodes.Underline)
           .Append(link)
           .Reset()
           .Build();

        Assert.That(result, Contains.Substring("\e[38;2;0;0;255m")); // Blue (RGB)
        Assert.That(result, Contains.Substring("\e[4m")); // Underline
        Assert.That(result, Contains.Substring("\e]8;;https://github.com/sst/opencode\e\\"));
        Assert.That(result, Contains.Substring("OpenCode Repository"));
        Assert.That(result, Contains.Substring("\e]8;;\e\\"));
        Assert.That(result, Contains.Substring("\e[0m")); // Reset
    }

    [Test]
    public void ScrollRegion_WithCursorMovement_ShouldCreateScrollableArea()
    {
        var scrollRegion = AnsiEscapeCodes.ScrollRegion(2, 20);

        var builder = new AnsiBuilder();
        var result = builder
            .Append(scrollRegion)
            .CursorPosition(2, 1)
            .Append("Contenuto scrollabile")
            .Append(AnsiEscapeCodes.ScrollDown)
            .Append(AnsiEscapeCodes.ResetScrollRegion)
            .Build();

        Assert.That(result, Contains.Substring("\e[2;20r")); // Scroll region
        Assert.That(result, Contains.Substring("\e[2;1H")); // Cursor position
        Assert.That(result, Contains.Substring("Contenuto scrollabile"));
        Assert.That(result, Contains.Substring("\e[L")); // Scroll down
        Assert.That(result, Contains.Substring("\e[r")); // Reset scroll region
    }

    [Test]
    public void ColorPaletteDemo_ShouldShowAllColors()
    {
        var builder = new AnsiBuilder();

        var colorDemo = builder
            .Append(AnsiEscapeCodes.ClearScreen)
            .CursorPosition(1, 1)
            .Append("=== PALETTE COLORI ANSI ===\n\n")
            .Build();

        // Aggiungi righe per colori standard
        var colors = new[]
        {
            ("Nero", AnsiEscapeCodes.Black, AnsiEscapeCodes.BgBlack),
            ("Rosso", AnsiEscapeCodes.Red, AnsiEscapeCodes.BgRed),
            ("Verde", AnsiEscapeCodes.Green, AnsiEscapeCodes.BgGreen),
            ("Giallo", AnsiEscapeCodes.Yellow, AnsiEscapeCodes.BgYellow),
            ("Blu", AnsiEscapeCodes.Blue, AnsiEscapeCodes.BgBlue),
            ("Magenta", AnsiEscapeCodes.Magenta, AnsiEscapeCodes.BgMagenta),
            ("Ciano", AnsiEscapeCodes.Cyan, AnsiEscapeCodes.BgCyan),
            ("Bianco", AnsiEscapeCodes.White, AnsiEscapeCodes.BgWhite)
        };

        foreach (var (name, fg, bg) in colors)
        {
            colorDemo += $"{bg}{fg}{name}\e[0m ";
        }

        colorDemo += "\n\nColori RGB comuni:\n";

        // Aggiungi colori RGB comuni
        var rgbColors = new[]
        {
            ("Arancione", AnsiEscapeCodes.ColorOrange),
            ("Viola", AnsiEscapeCodes.ColorPurple),
            ("Rosa", AnsiEscapeCodes.ColorPink),
            ("Grigio", AnsiEscapeCodes.ColorGray)
        };

        foreach (var (name, rgb) in rgbColors)
        {
            var rgbFg = AnsiEscapeCodes.FgColorRgb(rgb.R, rgb.G, rgb.B);
            var rgbBg = AnsiEscapeCodes.BgColorRgb(0, 0, 0);
            colorDemo += $"{rgbBg}{rgbFg}{name}\e[0m ";
        }

        // Verifica che la demo contenga le sequenze corrette
        Assert.That(colorDemo, Contains.Substring("=== PALETTE COLORI ANSI ==="));
        Assert.That(colorDemo, Contains.Substring("\e[40m\e[30mNero")); // Black on black background
        Assert.That(colorDemo, Contains.Substring("\e[41m\e[31mRosso")); // Red on red background
        Assert.That(colorDemo, Contains.Substring("\e[38;2;255;165;0mArancione")); // Orange RGB
        Assert.That(colorDemo, Contains.Substring("\e[38;2;128;0;128mViola")); // Purple RGB
    }

    [Test]
    public void TerminalInitializationSequence_ShouldIncludeAllEssentialSetup()
    {
        var initSequence = TerminalInit.Initialize();

        // Verifica che includa tutte le sequenze essenziali
        Assert.That(initSequence, Contains.Substring("\e[?1049h")); // Enter alternate screen
        Assert.That(initSequence, Contains.Substring("\e[?25l"));   // Hide cursor
        Assert.That(initSequence, Contains.Substring("\e[2J"));     // Clear screen
        Assert.That(initSequence, Contains.Substring("\e[H"));      // Cursor home
        Assert.That(initSequence, Contains.Substring("\e[?1000h")); // Enable mouse tracking
        Assert.That(initSequence, Contains.Substring("\e[?2004h")); // Enable bracketed paste
        Assert.That(initSequence, Contains.Substring("\e[?1004h")); // Enable focus reporting

        // Verifica che sia costruito correttamente con AnsiBuilder
        var builder = new AnsiBuilder();
        var manualSequence = builder
            .Append(AnsiEscapeCodes.EnterAlternateScreen)
            .Append(AnsiEscapeCodes.CursorHide)
            .Append(AnsiEscapeCodes.ClearScreen)
            .Append(AnsiEscapeCodes.CursorHome)
            .Append(AnsiEscapeCodes.EnableMouseTracking)
            .Append(AnsiEscapeCodes.EnableBracketedPaste)
            .Append(AnsiEscapeCodes.EnableFocusReporting)
            .Build();

        Assert.That(initSequence, Is.EqualTo(manualSequence));
    }

    [Test]
    public void ComplexTextFormatting_WithMultipleStyles_ShouldWorkCorrectly()
    {
        var complexText = AnsiEscapeCodes.ColoredText(
            "Testo normale " +
            AnsiEscapeCodes.ColoredText("grassetto", AnsiEscapeCodes.Bold) + " " +
            AnsiEscapeCodes.ColoredText("corsivo", AnsiEscapeCodes.Italic) + " " +
            AnsiEscapeCodes.ColoredText("sottolineato", AnsiEscapeCodes.Underline),
            AnsiEscapeCodes.BrightWhite,
            AnsiEscapeCodes.BgBlack
        );

        var builder = new AnsiBuilder();
        var result = builder
            .Append(AnsiEscapeCodes.ClearScreen)
            .Append(complexText)
            .Build();

        // Verifica la struttura generale
        Assert.That(result, Contains.Substring("\e[2J")); // Clear screen
        Assert.That(result, Contains.Substring("\e[97m")); // Bright white
        Assert.That(result, Contains.Substring("\e[40m")); // Black background
        Assert.That(result, Contains.Substring("Testo normale"));
        Assert.That(result, Contains.Substring("\e[1mgrassetto\e[0m")); // Bold con reset
        Assert.That(result, Contains.Substring("\e[3mcorsivo\e[0m")); // Italic con reset
        Assert.That(result, Contains.Substring("\e[4msottolineato\e[0m")); // Underline con reset
        Assert.That(result, Contains.Substring("\e[0m")); // Reset finale
    }

    [Test]
    public void PerformanceTest_BuildingLargeSequences_ShouldBeEfficient()
    {
        var builder = new AnsiBuilder();

        // Costruisci una sequenza grande con molti elementi
        for (int i = 0; i < 100; i++)
        {
            builder
                .CursorPosition(i % 24 + 1, i % 80 + 1)
                .FgColor256(i % 256)
                .BgColor256((i + 128) % 256)
                .Append($"Elemento {i}")
                .Reset();
        }

        var result = builder.Build();

        // Verifica che contenga elementi rappresentativi
        Assert.That(result, Contains.Substring("\e[38;5;")); // 256-color foreground
        Assert.That(result, Contains.Substring("\e[48;5;")); // 256-color background
        Assert.That(result, Contains.Substring("Elemento 0"));
        Assert.That(result, Contains.Substring("Elemento 99"));
        Assert.That(result.Length, Is.GreaterThan(1000)); // Dovrebbe essere abbastanza grande
    }

    [Test]
    public void ErrorHandling_WithInvalidColors_ShouldHandleGracefully()
    {
        // Testa che i metodi gestiscano valori fuori range
        var fg256 = AnsiEscapeCodes.FgColor256(300); // Oltre 255
        var bg256 = AnsiEscapeCodes.BgColor256(-1);   // Sotto 0

        // Anche se i valori sono fuori range, le sequenze dovrebbero essere valide
        Assert.That(fg256, Does.StartWith("\e[38;5;"));
        Assert.That(bg256, Does.StartWith("\e[48;5;"));
        Assert.That(fg256, Does.EndWith("m"));
        Assert.That(bg256, Does.EndWith("m"));
    }
}
