using Chevron9.Backends.Terminal.Utils;

namespace Chevron9.Tests.Backends.Terminal;

[TestFixture]
public class AnsiEscapeCodesTests
{
    [Test]
    public void Escape_ShouldReturnCorrectEscapeSequence()
    {
        Assert.That(AnsiEscapeCodes.Escape, Is.EqualTo("\e["));
    }

    [Test]
    public void Reset_ShouldReturnCorrectResetSequence()
    {
        Assert.That(AnsiEscapeCodes.Reset, Is.EqualTo("\e[0m"));
    }

    [Test]
    public void ClearScreen_ShouldReturnCorrectClearScreenSequence()
    {
        Assert.That(AnsiEscapeCodes.ClearScreen, Is.EqualTo("\e[2J"));
    }

    [Test]
    public void ClearLine_ShouldReturnCorrectClearLineSequence()
    {
        Assert.That(AnsiEscapeCodes.ClearLine, Is.EqualTo("\e[2K"));
    }

    [Test]
    public void CursorHome_ShouldReturnCorrectCursorHomeSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorHome, Is.EqualTo("\e[H"));
    }

    [Test]
    public void CursorHide_ShouldReturnCorrectCursorHideSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorHide, Is.EqualTo("\e[?25l"));
    }

    [Test]
    public void CursorShow_ShouldReturnCorrectCursorShowSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorShow, Is.EqualTo("\e[?25h"));
    }

    [Test]
    public void SaveCursor_ShouldReturnCorrectSaveCursorSequence()
    {
        Assert.That(AnsiEscapeCodes.SaveCursor, Is.EqualTo("\e[s"));
    }

    [Test]
    public void RestoreCursor_ShouldReturnCorrectRestoreCursorSequence()
    {
        Assert.That(AnsiEscapeCodes.RestoreCursor, Is.EqualTo("\e[u"));
    }

    [Test]
    public void Bold_ShouldReturnCorrectBoldSequence()
    {
        Assert.That(AnsiEscapeCodes.Bold, Is.EqualTo("\e[1m"));
    }

    [Test]
    public void Underline_ShouldReturnCorrectUnderlineSequence()
    {
        Assert.That(AnsiEscapeCodes.Underline, Is.EqualTo("\e[4m"));
    }

    [Test]
    public void Black_ShouldReturnCorrectBlackSequence()
    {
        Assert.That(AnsiEscapeCodes.Black, Is.EqualTo("\e[30m"));
    }

    [Test]
    public void Red_ShouldReturnCorrectRedSequence()
    {
        Assert.That(AnsiEscapeCodes.Red, Is.EqualTo("\e[31m"));
    }

    [Test]
    public void Green_ShouldReturnCorrectGreenSequence()
    {
        Assert.That(AnsiEscapeCodes.Green, Is.EqualTo("\e[32m"));
    }

    [Test]
    public void Yellow_ShouldReturnCorrectYellowSequence()
    {
        Assert.That(AnsiEscapeCodes.Yellow, Is.EqualTo("\e[33m"));
    }

    [Test]
    public void Blue_ShouldReturnCorrectBlueSequence()
    {
        Assert.That(AnsiEscapeCodes.Blue, Is.EqualTo("\e[34m"));
    }

    [Test]
    public void Magenta_ShouldReturnCorrectMagentaSequence()
    {
        Assert.That(AnsiEscapeCodes.Magenta, Is.EqualTo("\e[35m"));
    }

    [Test]
    public void Cyan_ShouldReturnCorrectCyanSequence()
    {
        Assert.That(AnsiEscapeCodes.Cyan, Is.EqualTo("\e[36m"));
    }

    [Test]
    public void White_ShouldReturnCorrectWhiteSequence()
    {
        Assert.That(AnsiEscapeCodes.White, Is.EqualTo("\e[37m"));
    }

    [Test]
    public void BgBlack_ShouldReturnCorrectBackgroundBlackSequence()
    {
        Assert.That(AnsiEscapeCodes.BgBlack, Is.EqualTo("\e[40m"));
    }

    [Test]
    public void BgRed_ShouldReturnCorrectBackgroundRedSequence()
    {
        Assert.That(AnsiEscapeCodes.BgRed, Is.EqualTo("\e[41m"));
    }

    [Test]
    public void CursorUp_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorUp(), Is.EqualTo("\e[1A"));
    }

    [Test]
    public void CursorUp_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorUp(5), Is.EqualTo("\e[5A"));
    }

    [Test]
    public void CursorDown_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorDown(), Is.EqualTo("\e[1B"));
    }

    [Test]
    public void CursorDown_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorDown(3), Is.EqualTo("\e[3B"));
    }

    [Test]
    public void CursorForward_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorForward(), Is.EqualTo("\e[1C"));
    }

    [Test]
    public void CursorForward_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorForward(7), Is.EqualTo("\e[7C"));
    }

    [Test]
    public void CursorBack_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorBack(), Is.EqualTo("\e[1D"));
    }

    [Test]
    public void CursorBack_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorBack(2), Is.EqualTo("\e[2D"));
    }

    [Test]
    public void CursorPosition_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorPosition(10, 20), Is.EqualTo("\e[10;20H"));
    }

    [Test]
    public void FgColor256_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.FgColor256(123), Is.EqualTo("\e[38;5;123m"));
    }

    [Test]
    public void BgColor256_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.BgColor256(200), Is.EqualTo("\e[48;5;200m"));
    }

    [Test]
    public void FgColorRgb_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.FgColorRgb(255, 128, 64), Is.EqualTo("\e[38;2;255;128;64m"));
    }

    [Test]
    public void BgColorRgb_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.BgColorRgb(100, 150, 200), Is.EqualTo("\e[48;2;100;150;200m"));
    }

    [Test]
    public void EnterAlternateScreen_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.EnterAlternateScreen, Is.EqualTo("\e[?1049h"));
    }

    [Test]
    public void ExitAlternateScreen_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.ExitAlternateScreen, Is.EqualTo("\e[?1049l"));
    }

    [Test]
    public void EnableMouseTracking_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.EnableMouseTracking, Is.EqualTo("\e[?1000h\e[?1002h\e[?1015h\e[?1006h"));
    }

    [Test]
    public void DisableMouseTracking_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.DisableMouseTracking, Is.EqualTo("\e[?1006l\e[?1015l\e[?1002l\e[?1000l"));
    }

    [Test]
    public void SetWindowTitle_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.SetWindowTitle("Test Title"), Is.EqualTo("\u001b]0;Test Title\u0007"));
    }

    [Test]
    public void EnableBracketedPaste_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.EnableBracketedPaste, Is.EqualTo("\e[?2004h"));
    }

    [Test]
    public void DisableBracketedPaste_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.DisableBracketedPaste, Is.EqualTo("\e[?2004l"));
    }

    [Test]
    public void UnicodeEscapeAndCSharpEscape_ShouldProduceIdenticalResults()
    {
        // Verifica che \u001b e \e producano esattamente lo stesso risultato
        const string unicodeEscape = "\u001b[31m";  // Unicode escape
        const string csharpEscape = "\e[31m";      // C# escape sequence

        Assert.That(unicodeEscape, Is.EqualTo(csharpEscape));
        Assert.That(unicodeEscape.Length, Is.EqualTo(csharpEscape.Length));

        // Verifica che entrambi abbiano la stessa rappresentazione byte
        var unicodeBytes = System.Text.Encoding.UTF8.GetBytes(unicodeEscape);
        var csharpBytes = System.Text.Encoding.UTF8.GetBytes(csharpEscape);
        Assert.That(unicodeBytes, Is.EqualTo(csharpBytes));
    }

    [Test]
    public void EscapeSequencesEquivalence_ShouldWorkForAllCommonCodes()
    {
        // Testa varie sequenze comuni per assicurarsi che siano equivalenti
        var testCases = new[]
        {
            ("\u001b[0m", "\e[0m"),     // Reset
            ("\u001b[31m", "\e[31m"),   // Red
            ("\u001b[2J", "\e[2J"),     // Clear screen
            ("\u001b[H", "\e[H"),       // Cursor home
            ("\u001b[?25l", "\e[?25l"), // Hide cursor
        };

        foreach (var (unicode, csharp) in testCases)
        {
            Assert.That(unicode, Is.EqualTo(csharp), $"Sequences should be equal: {unicode} vs {csharp}");

            // Verifica anche che abbiano gli stessi byte
            var unicodeBytes = System.Text.Encoding.UTF8.GetBytes(unicode);
            var csharpBytes = System.Text.Encoding.UTF8.GetBytes(csharp);
            Assert.That(unicodeBytes, Is.EqualTo(csharpBytes), $"Byte sequences should be equal for: {unicode}");
        }
    }

    [Test]
    public void AdditionalTextStyles_ShouldReturnCorrectSequences()
    {
        Assert.That(AnsiEscapeCodes.Italic, Is.EqualTo("\e[3m"));
        Assert.That(AnsiEscapeCodes.DoubleUnderline, Is.EqualTo("\e[21m"));
        Assert.That(AnsiEscapeCodes.Overline, Is.EqualTo("\e[53m"));
        Assert.That(AnsiEscapeCodes.Superscript, Is.EqualTo("\e[73m"));
        Assert.That(AnsiEscapeCodes.Subscript, Is.EqualTo("\e[74m"));
    }

    [Test]
    public void CommonRgbColors_ShouldHaveCorrectValues()
    {
        Assert.That(AnsiEscapeCodes.ColorOrange, Is.EqualTo((255, 165, 0)));
        Assert.That(AnsiEscapeCodes.ColorPurple, Is.EqualTo((128, 0, 128)));
        Assert.That(AnsiEscapeCodes.ColorPink, Is.EqualTo((255, 192, 203)));
        Assert.That(AnsiEscapeCodes.ColorGray, Is.EqualTo((128, 128, 128)));
    }

    [Test]
    public void TerminalControlCharacters_ShouldReturnCorrectSequences()
    {
        Assert.That(AnsiEscapeCodes.Bell, Is.EqualTo("\a"));
        Assert.That(AnsiEscapeCodes.Backspace, Is.EqualTo("\b"));
        Assert.That(AnsiEscapeCodes.Tab, Is.EqualTo("\t"));
        Assert.That(AnsiEscapeCodes.LineFeed, Is.EqualTo("\n"));
        Assert.That(AnsiEscapeCodes.CarriageReturn, Is.EqualTo("\r"));
    }

    [Test]
    public void Combine_ShouldConcatenateSequences()
    {
        var result = AnsiEscapeCodes.Combine("\e[31m", "\e[1m", "\e[4m");
        Assert.That(result, Is.EqualTo("\e[31m\e[1m\e[4m"));
    }

    [Test]
    public void Style_ShouldCombineColorsAndStyles()
    {
        var result = AnsiEscapeCodes.Style("\e[31m", "\e[40m", "\e[1m", "\e[4m");
        Assert.That(result, Is.EqualTo("\e[31m\e[40m\e[1m\e[4m"));
    }

    [Test]
    public void ColoredText_ShouldApplyStyleAndReset()
    {
        var result = AnsiEscapeCodes.ColoredText("Hello", "\e[31m", "\e[40m", "\e[1m");
        Assert.That(result, Is.EqualTo("\e[31m\e[40m\e[1mHello\e[0m"));
    }

    [Test]
    public void ColoredText_WithoutStyles_ShouldReturnTextAsIs()
    {
        var result = AnsiEscapeCodes.ColoredText("Hello");
        Assert.That(result, Is.EqualTo("Hello"));
    }

    [Test]
    public void GradientText_ShouldApplyColorGradient()
    {
        var result = AnsiEscapeCodes.GradientText("AB", 1, 2);
        Assert.That(result, Is.EqualTo("\e[38;5;1mA\e[38;5;2mB\e[0m"));
    }

    [Test]
    public void GradientText_SingleCharacter_ShouldApplySingleColor()
    {
        var result = AnsiEscapeCodes.GradientText("A", 10, 20);
        Assert.That(result, Is.EqualTo("\e[38;5;10mA\e[0m"));
    }

    [Test]
    public void Hyperlink_ShouldGenerateCorrectSequence()
    {
        var result = AnsiEscapeCodes.Hyperlink("https://example.com", "Click here");
        Assert.That(result, Is.EqualTo("\e]8;;https://example.com\e\\Click here\e]8;;\e\\"));
    }

    [Test]
    public void ScrollRegion_ShouldGenerateCorrectSequence()
    {
        var result = AnsiEscapeCodes.ScrollRegion(1, 10);
        Assert.That(result, Is.EqualTo("\e[1;10r"));
    }

    [Test]
    public void ScreenManipulationConstants_ShouldReturnCorrectSequences()
    {
        Assert.That(AnsiEscapeCodes.SaveScreen, Is.EqualTo("\e[?47h"));
        Assert.That(AnsiEscapeCodes.RestoreScreen, Is.EqualTo("\e[?47l"));
        Assert.That(AnsiEscapeCodes.EnableSyncOutput, Is.EqualTo("\e[?2026h"));
        Assert.That(AnsiEscapeCodes.DisableSyncOutput, Is.EqualTo("\e[?2026l"));
        Assert.That(AnsiEscapeCodes.ResetScrollRegion, Is.EqualTo("\e[r"));
    }
}
