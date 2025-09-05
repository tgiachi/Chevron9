using Chevron9.Backends.Terminal.Utils;
using Chevron9.Shared.Graphics;

namespace Chevron9.Tests.Backends.Terminal;

[TestFixture]
public class AnsiBuilderTests
{
    [Test]
    public void Constructor_Default_ShouldCreateEmptyBuilder()
    {
        var builder = new AnsiBuilder();
        Assert.That(builder.Length, Is.EqualTo(0));
        Assert.That(builder.Build(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Constructor_WithCapacity_ShouldCreateBuilderWithCapacity()
    {
        var builder = new AnsiBuilder(100);
        Assert.That(builder.Length, Is.EqualTo(0));
    }

    [Test]
    public void Clear_ShouldResetBuilder()
    {
        var builder = new AnsiBuilder();
        builder.Append("test");
        builder.Clear();

        Assert.That(builder.Length, Is.EqualTo(0));
        Assert.That(builder.Build(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Append_String_ShouldAppendText()
    {
        var builder = new AnsiBuilder();
        builder.Append("Hello");

        Assert.That(builder.Build(), Is.EqualTo("Hello"));
    }

    [Test]
    public void Append_Char_ShouldAppendCharacter()
    {
        var builder = new AnsiBuilder();
        builder.Append('A');

        Assert.That(builder.Build(), Is.EqualTo("A"));
    }

    [Test]
    public void Escape_ShouldStartEscapeSequence()
    {
        var builder = new AnsiBuilder();
        builder.Escape();

        Assert.That(builder.Build(), Is.EqualTo("\u001b["));
    }

    [Test]
    public void Param_Single_ShouldAddParameter()
    {
        var builder = new AnsiBuilder();
        builder.Escape().Param(5);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[5"));
    }

    [Test]
    public void Params_Multiple_ShouldAddMultipleParameters()
    {
        var builder = new AnsiBuilder();
        builder.Escape().Params(1, 2, 3);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[1;2;3"));
    }

    [Test]
    public void End_WithDefaultTerminator_ShouldEndSequence()
    {
        var builder = new AnsiBuilder();
        builder.Escape().Param(1).End();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[1m"));
    }

    [Test]
    public void End_WithCustomTerminator_ShouldEndSequence()
    {
        var builder = new AnsiBuilder();
        builder.Escape().Param(10).End('H');

        Assert.That(builder.Build(), Is.EqualTo("\u001b[10H"));
    }

    [Test]
    public void CursorPosition_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        builder.CursorPosition(5, 10);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[5;10H"));
    }

    [Test]
    public void CursorUp_Default_ShouldMoveUpOneLine()
    {
        var builder = new AnsiBuilder();
        builder.CursorUp();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[1A"));
    }

    [Test]
    public void CursorUp_SpecificLines_ShouldMoveUpSpecifiedLines()
    {
        var builder = new AnsiBuilder();
        builder.CursorUp(3);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[3A"));
    }

    [Test]
    public void CursorDown_Default_ShouldMoveDownOneLine()
    {
        var builder = new AnsiBuilder();
        builder.CursorDown();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[1B"));
    }

    [Test]
    public void CursorDown_SpecificLines_ShouldMoveDownSpecifiedLines()
    {
        var builder = new AnsiBuilder();
        builder.CursorDown(5);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[5B"));
    }

    [Test]
    public void CursorForward_Default_ShouldMoveForwardOneColumn()
    {
        var builder = new AnsiBuilder();
        builder.CursorForward();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[1C"));
    }

    [Test]
    public void CursorForward_SpecificColumns_ShouldMoveForwardSpecifiedColumns()
    {
        var builder = new AnsiBuilder();
        builder.CursorForward(7);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[7C"));
    }

    [Test]
    public void CursorBack_Default_ShouldMoveBackOneColumn()
    {
        var builder = new AnsiBuilder();
        builder.CursorBack();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[1D"));
    }

    [Test]
    public void CursorBack_SpecificColumns_ShouldMoveBackSpecifiedColumns()
    {
        var builder = new AnsiBuilder();
        builder.CursorBack(4);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[4D"));
    }

    [Test]
    public void FgColor256_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        builder.FgColor256(123);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[38;5;123m"));
    }

    [Test]
    public void BgColor256_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        builder.BgColor256(200);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[48;5;200m"));
    }

    [Test]
    public void FgColorRgb_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        builder.FgColorRgb(255, 128, 64);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[38;2;255;128;64m"));
    }

    [Test]
    public void BgColorRgb_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        builder.BgColorRgb(100, 150, 200);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[48;2;100;150;200m"));
    }

    [Test]
    public void FgColor_WithColorStruct_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        var color = new Color(255, 128, 64);
        builder.FgColor(color);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[38;2;255;128;64m"));
    }

    [Test]
    public void BgColor_WithColorStruct_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        var color = new Color(100, 150, 200);
        builder.BgColor(color);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[48;2;100;150;200m"));
    }

    [Test]
    public void Colors_WithBothColorStructs_ShouldGenerateCorrectSequence()
    {
        var builder = new AnsiBuilder();
        var fgColor = new Color(255, 255, 255);
        var bgColor = new Color(0, 0, 0);
        builder.Colors(fgColor, bgColor);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[38;2;255;255;255m\u001b[48;2;0;0;0m"));
    }

    [Test]
    public void Style_ShouldApplyStyle()
    {
        var builder = new AnsiBuilder();
        builder.Style(AnsiEscapeCodes.Bold);

        Assert.That(builder.Build(), Is.EqualTo("\u001b[1m"));
    }

    [Test]
    public void Reset_ShouldGenerateResetSequence()
    {
        var builder = new AnsiBuilder();
        builder.Reset();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[0m"));
    }

    [Test]
    public void ClearScreen_ShouldGenerateClearScreenSequence()
    {
        var builder = new AnsiBuilder();
        builder.ClearScreen();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[2J"));
    }

    [Test]
    public void ClearLine_ShouldGenerateClearLineSequence()
    {
        var builder = new AnsiBuilder();
        builder.ClearLine();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[2K"));
    }

    [Test]
    public void CursorHome_ShouldGenerateCursorHomeSequence()
    {
        var builder = new AnsiBuilder();
        builder.CursorHome();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[H"));
    }

    [Test]
    public void HideCursor_ShouldGenerateHideCursorSequence()
    {
        var builder = new AnsiBuilder();
        builder.HideCursor();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[?25l"));
    }

    [Test]
    public void ShowCursor_ShouldGenerateShowCursorSequence()
    {
        var builder = new AnsiBuilder();
        builder.ShowCursor();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[?25h"));
    }

    [Test]
    public void SaveCursor_ShouldGenerateSaveCursorSequence()
    {
        var builder = new AnsiBuilder();
        builder.SaveCursor();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[s"));
    }

    [Test]
    public void RestoreCursor_ShouldGenerateRestoreCursorSequence()
    {
        var builder = new AnsiBuilder();
        builder.RestoreCursor();

        Assert.That(builder.Build(), Is.EqualTo("\u001b[u"));
    }

    [Test]
    public void FluentApi_ShouldChainMethodsCorrectly()
    {
        var builder = new AnsiBuilder();
        var result = builder
            .CursorPosition(1, 1)
            .FgColorRgb(255, 0, 0)
            .BgColorRgb(0, 0, 0)
            .Style(AnsiEscapeCodes.Bold)
            .Append("Hello")
            .Reset()
            .Build();

        Assert.That(result, Is.EqualTo("\u001b[1;1H\u001b[38;2;255;0;0m\u001b[48;2;0;0;0m\u001b[1mHello\u001b[0m"));
    }

    [Test]
    public void ImplicitConversion_ShouldWork()
    {
        var builder = new AnsiBuilder();
        builder.Append("test");

        string result = builder; // Implicit conversion

        Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void ComplexSequence_ShouldBuildCorrectly()
    {
        var builder = new AnsiBuilder();
        builder
            .ClearScreen()
            .CursorHome()
            .FgColor(Color.Red)
            .BgColor(Color.Black)
            .Append("Error: ")
            .FgColor(Color.White)
            .Append("Something went wrong")
            .Reset()
            .CursorPosition(2, 1);

        var expected = "\u001b[2J\u001b[H\u001b[38;2;255;0;0m\u001b[48;2;0;0;0mError: \u001b[38;2;255;255;255mSomething went wrong\u001b[0m\u001b[2;1H";
        Assert.That(builder.Build(), Is.EqualTo(expected));
    }
}