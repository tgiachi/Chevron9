using Chevron9.Backends.Terminal.Utils;

namespace Chevron9.Tests.Backends.Terminal;

[TestFixture]
public class AnsiEscapeCodesTests
{
    [Test]
    public void Escape_ShouldReturnCorrectEscapeSequence()
    {
        Assert.That(AnsiEscapeCodes.Escape, Is.EqualTo("\u001b["));
    }

    [Test]
    public void Reset_ShouldReturnCorrectResetSequence()
    {
        Assert.That(AnsiEscapeCodes.Reset, Is.EqualTo("\u001b[0m"));
    }

    [Test]
    public void ClearScreen_ShouldReturnCorrectClearScreenSequence()
    {
        Assert.That(AnsiEscapeCodes.ClearScreen, Is.EqualTo("\u001b[2J"));
    }

    [Test]
    public void ClearLine_ShouldReturnCorrectClearLineSequence()
    {
        Assert.That(AnsiEscapeCodes.ClearLine, Is.EqualTo("\u001b[2K"));
    }

    [Test]
    public void CursorHome_ShouldReturnCorrectCursorHomeSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorHome, Is.EqualTo("\u001b[H"));
    }

    [Test]
    public void CursorHide_ShouldReturnCorrectCursorHideSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorHide, Is.EqualTo("\u001b[?25l"));
    }

    [Test]
    public void CursorShow_ShouldReturnCorrectCursorShowSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorShow, Is.EqualTo("\u001b[?25h"));
    }

    [Test]
    public void SaveCursor_ShouldReturnCorrectSaveCursorSequence()
    {
        Assert.That(AnsiEscapeCodes.SaveCursor, Is.EqualTo("\u001b[s"));
    }

    [Test]
    public void RestoreCursor_ShouldReturnCorrectRestoreCursorSequence()
    {
        Assert.That(AnsiEscapeCodes.RestoreCursor, Is.EqualTo("\u001b[u"));
    }

    [Test]
    public void Bold_ShouldReturnCorrectBoldSequence()
    {
        Assert.That(AnsiEscapeCodes.Bold, Is.EqualTo("\u001b[1m"));
    }

    [Test]
    public void Underline_ShouldReturnCorrectUnderlineSequence()
    {
        Assert.That(AnsiEscapeCodes.Underline, Is.EqualTo("\u001b[4m"));
    }

    [Test]
    public void Black_ShouldReturnCorrectBlackSequence()
    {
        Assert.That(AnsiEscapeCodes.Black, Is.EqualTo("\u001b[30m"));
    }

    [Test]
    public void Red_ShouldReturnCorrectRedSequence()
    {
        Assert.That(AnsiEscapeCodes.Red, Is.EqualTo("\u001b[31m"));
    }

    [Test]
    public void Green_ShouldReturnCorrectGreenSequence()
    {
        Assert.That(AnsiEscapeCodes.Green, Is.EqualTo("\u001b[32m"));
    }

    [Test]
    public void Yellow_ShouldReturnCorrectYellowSequence()
    {
        Assert.That(AnsiEscapeCodes.Yellow, Is.EqualTo("\u001b[33m"));
    }

    [Test]
    public void Blue_ShouldReturnCorrectBlueSequence()
    {
        Assert.That(AnsiEscapeCodes.Blue, Is.EqualTo("\u001b[34m"));
    }

    [Test]
    public void Magenta_ShouldReturnCorrectMagentaSequence()
    {
        Assert.That(AnsiEscapeCodes.Magenta, Is.EqualTo("\u001b[35m"));
    }

    [Test]
    public void Cyan_ShouldReturnCorrectCyanSequence()
    {
        Assert.That(AnsiEscapeCodes.Cyan, Is.EqualTo("\u001b[36m"));
    }

    [Test]
    public void White_ShouldReturnCorrectWhiteSequence()
    {
        Assert.That(AnsiEscapeCodes.White, Is.EqualTo("\u001b[37m"));
    }

    [Test]
    public void BgBlack_ShouldReturnCorrectBackgroundBlackSequence()
    {
        Assert.That(AnsiEscapeCodes.BgBlack, Is.EqualTo("\u001b[40m"));
    }

    [Test]
    public void BgRed_ShouldReturnCorrectBackgroundRedSequence()
    {
        Assert.That(AnsiEscapeCodes.BgRed, Is.EqualTo("\u001b[41m"));
    }

    [Test]
    public void CursorUp_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorUp(), Is.EqualTo("\u001b[1A"));
    }

    [Test]
    public void CursorUp_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorUp(5), Is.EqualTo("\u001b[5A"));
    }

    [Test]
    public void CursorDown_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorDown(), Is.EqualTo("\u001b[1B"));
    }

    [Test]
    public void CursorDown_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorDown(3), Is.EqualTo("\u001b[3B"));
    }

    [Test]
    public void CursorForward_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorForward(), Is.EqualTo("\u001b[1C"));
    }

    [Test]
    public void CursorForward_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorForward(7), Is.EqualTo("\u001b[7C"));
    }

    [Test]
    public void CursorBack_WithDefaultParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorBack(), Is.EqualTo("\u001b[1D"));
    }

    [Test]
    public void CursorBack_WithSpecificParameter_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorBack(2), Is.EqualTo("\u001b[2D"));
    }

    [Test]
    public void CursorPosition_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.CursorPosition(10, 20), Is.EqualTo("\u001b[10;20H"));
    }

    [Test]
    public void FgColor256_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.FgColor256(123), Is.EqualTo("\u001b[38;5;123m"));
    }

    [Test]
    public void BgColor256_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.BgColor256(200), Is.EqualTo("\u001b[48;5;200m"));
    }

    [Test]
    public void FgColorRgb_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.FgColorRgb(255, 128, 64), Is.EqualTo("\u001b[38;2;255;128;64m"));
    }

    [Test]
    public void BgColorRgb_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.BgColorRgb(100, 150, 200), Is.EqualTo("\u001b[48;2;100;150;200m"));
    }

    [Test]
    public void EnterAlternateScreen_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.EnterAlternateScreen, Is.EqualTo("\u001b[?1049h"));
    }

    [Test]
    public void ExitAlternateScreen_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.ExitAlternateScreen, Is.EqualTo("\u001b[?1049l"));
    }

    [Test]
    public void EnableMouseTracking_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.EnableMouseTracking, Is.EqualTo("\u001b[?1000h\u001b[?1002h\u001b[?1015h\u001b[?1006h"));
    }

    [Test]
    public void DisableMouseTracking_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.DisableMouseTracking, Is.EqualTo("\u001b[?1006l\u001b[?1015l\u001b[?1002l\u001b[?1000l"));
    }

    [Test]
    public void SetWindowTitle_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.SetWindowTitle("Test Title"), Is.EqualTo("\u001b]0;Test Title\u0007"));
    }

    [Test]
    public void EnableBracketedPaste_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.EnableBracketedPaste, Is.EqualTo("\u001b[?2004h"));
    }

    [Test]
    public void DisableBracketedPaste_ShouldReturnCorrectSequence()
    {
        Assert.That(AnsiEscapeCodes.DisableBracketedPaste, Is.EqualTo("\u001b[?2004l"));
    }
}
