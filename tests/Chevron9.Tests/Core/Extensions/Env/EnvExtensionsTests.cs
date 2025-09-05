using Chevron9.Core.Extensions.Env;

namespace Chevron9.Tests.Core.Extensions.Env;

[TestFixture]
public class EnvExtensionsTests
{
    [Test]
    public void ExpandEnvironmentVariables_WithNullInput_ShouldReturnNull()
    {
        string nullString = null;
        var result = nullString.ExpandEnvironmentVariables();

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ExpandEnvironmentVariables_WithEmptyInput_ShouldReturnEmpty()
    {
        var emptyString = "";
        var result = emptyString.ExpandEnvironmentVariables();

        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void ExpandEnvironmentVariables_WithExistingVariable_ShouldReplace()
    {
        var testVarName = "TEST_EXPAND_VAR";
        var testVarValue = "test_expand_value";
        Environment.SetEnvironmentVariable(testVarName, testVarValue);

        try
        {
            var input = $"Before ${testVarName} After";
            var result = input.ExpandEnvironmentVariables();

            Assert.That(result, Is.EqualTo($"Before {testVarValue} After"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(testVarName, null);
        }
    }

    [Test]
    public void ExpandEnvironmentVariables_WithMultipleVariables_ShouldReplaceAll()
    {
        var testVar1Name = "TEST_VAR1";
        var testVar1Value = "value1";
        var testVar2Name = "TEST_VAR2";
        var testVar2Value = "value2";

        Environment.SetEnvironmentVariable(testVar1Name, testVar1Value);
        Environment.SetEnvironmentVariable(testVar2Name, testVar2Value);

        try
        {
            var input = $"${testVar1Name} and ${testVar2Name}";
            var result = input.ExpandEnvironmentVariables();

            Assert.That(result, Is.EqualTo($"{testVar1Value} and {testVar2Value}"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(testVar1Name, null);
            Environment.SetEnvironmentVariable(testVar2Name, null);
        }
    }

    [Test]
    public void ExpandEnvironmentVariables_WithNonexistentVariable_ShouldReplaceWithEmpty()
    {
        var input = "Before $NONEXISTENT_VAR After";
        var result = input.ExpandEnvironmentVariables();

        Assert.That(result, Is.EqualTo("Before  After"));
    }

    [Test]
    public void ExpandEnvironmentVariables_WithVariableContainingEmptyValue_ShouldReplaceWithEmpty()
    {
        var testVarName = "TEST_EMPTY_VAR";
        Environment.SetEnvironmentVariable(testVarName, "");

        try
        {
            var input = $"Before ${testVarName} After";
            var result = input.ExpandEnvironmentVariables();

            Assert.That(result, Is.EqualTo("Before  After"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(testVarName, null);
        }
    }

    [Test]
    public void ExpandEnvironmentVariables_WithoutDollarVariables_ShouldReturnUnchanged()
    {
        var input = "No variables here";
        var result = input.ExpandEnvironmentVariables();

        Assert.That(result, Is.EqualTo(input));
    }

    [Test]
    public void ExpandEnvironmentVariables_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        var testVarName = "TEST_SPECIAL_VAR";
        var testVarValue = "value with spaces & symbols!";
        Environment.SetEnvironmentVariable(testVarName, testVarValue);

        try
        {
            var input = $"Text ${testVarName} end";
            var result = input.ExpandEnvironmentVariables();

            Assert.That(result, Is.EqualTo($"Text {testVarValue} end"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(testVarName, null);
        }
    }

    [Test]
    public void ExpandEnvironmentVariables_WithRepeatedVariable_ShouldReplaceAll()
    {
        var testVarName = "TEST_REPEAT_VAR";
        var testVarValue = "repeated";
        Environment.SetEnvironmentVariable(testVarName, testVarValue);

        try
        {
            var input = $"${testVarName} and ${testVarName} again";
            var result = input.ExpandEnvironmentVariables();

            Assert.That(result, Is.EqualTo($"{testVarValue} and {testVarValue} again"));
        }
        finally
        {
            Environment.SetEnvironmentVariable(testVarName, null);
        }
    }
}
