using Chevron9.Core.Utils;

namespace Chevron9.Tests.Core.Utils;

[TestFixture]
public class StringUtilsTests
{
    [TestCase("HelloWorld", "hello_world")]
    [TestCase("APIResponse", "api_response")]
    [TestCase("userId", "user_id")]
    [TestCase("XMLParser", "xml_parser")]
    [TestCase("HTTPSConnection", "https_connection")]
    [TestCase("A", "a")]
    [TestCase("AB", "ab")]
    [TestCase("camelCase", "camel_case")]
    [TestCase("PascalCase", "pascal_case")]
    [TestCase("already_snake_case", "already_snake_case")]
    [TestCase("kebab-case", "kebab_case")]
    [TestCase("mixed_Format-Case", "mixed__format__case")]
    public void ToSnakeCase_WithValidInput_ShouldConvertCorrectly(string input, string expected)
    {
        var result = StringUtils.ToSnakeCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToSnakeCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToSnakeCase(null));
    }

    [Test]
    public void ToSnakeCase_WithEmptyInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToSnakeCase(""));
    }

    [TestCase("HelloWorld", "helloWorld")]
    [TestCase("API_RESPONSE", "apiResponse")]
    [TestCase("user-id", "userId")]
    [TestCase("snake_case_string", "snakeCaseString")]
    [TestCase("kebab-case-string", "kebabCaseString")]
    [TestCase("mixed_Format-Case", "mixedFormatCase")]
    [TestCase("A", "a")]
    [TestCase("AB", "ab")]
    [TestCase("UPPERCASE", "uppercase")]
    public void ToCamelCase_WithValidInput_ShouldConvertCorrectly(string input, string expected)
    {
        var result = StringUtils.ToCamelCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToCamelCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToCamelCase(null));
    }

    [Test]
    public void ToCamelCase_WithEmptyInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToCamelCase(""));
    }

    [TestCase("hello_world", "HelloWorld")]
    [TestCase("api-response", "ApiResponse")]
    [TestCase("userId", "UserId")]
    [TestCase("snake_case_string", "SnakeCaseString")]
    [TestCase("kebab-case-string", "KebabCaseString")]
    [TestCase("mixed_Format-Case", "MixedFormatCase")]
    [TestCase("A", "A")]
    [TestCase("AB", "Ab")]
    [TestCase("lowercase", "Lowercase")]
    public void ToPascalCase_WithValidInput_ShouldConvertCorrectly(string input, string expected)
    {
        var result = StringUtils.ToPascalCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToPascalCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToPascalCase(null));
    }

    [Test]
    public void ToPascalCase_WithEmptyInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToPascalCase(""));
    }

    [TestCase("HelloWorld", "hello-world")]
    [TestCase("API_RESPONSE", "api-response")]
    [TestCase("userId", "user-id")]
    [TestCase("snake_case_string", "snake-case-string")]
    [TestCase("PascalCaseString", "pascal-case-string")]
    [TestCase("mixed_Format-Case", "mixed-format-case")]
    [TestCase("A", "a")]
    [TestCase("AB", "ab")]
    public void ToKebabCase_WithValidInput_ShouldConvertCorrectly(string input, string expected)
    {
        var result = StringUtils.ToKebabCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToKebabCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToKebabCase(null));
    }

    [Test]
    public void ToKebabCase_WithEmptyInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToKebabCase(""));
    }

    [TestCase("HelloWorld", "HELLO_WORLD")]
    [TestCase("apiResponse", "API_RESPONSE")]
    [TestCase("user-id", "USER_ID")]
    [TestCase("snake_case_string", "SNAKE_CASE_STRING")]
    [TestCase("PascalCaseString", "PASCAL_CASE_STRING")]
    [TestCase("A", "A")]
    [TestCase("AB", "AB")]
    public void ToUpperSnakeCase_WithValidInput_ShouldConvertCorrectly(string input, string expected)
    {
        var result = StringUtils.ToUpperSnakeCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToUpperSnakeCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToUpperSnakeCase(null));
    }

    [TestCase("hello_world", "Hello World")]
    [TestCase("API_RESPONSE", "Api Response")]
    [TestCase("user-id", "User Id")]
    [TestCase("camelCase", "Camel Case")]
    [TestCase("PascalCase", "Pascal Case")]
    [TestCase("mixed_Format-Case", "Mixed Format Case")]
    [TestCase("A", "A")]
    [TestCase("AB", "Ab")]
    public void ToTitleCase_WithValidInput_ShouldConvertCorrectly(string input, string expected)
    {
        var result = StringUtils.ToTitleCase(input);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToTitleCase_WithNullInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToTitleCase(null));
    }

    [Test]
    public void ToTitleCase_WithEmptyInput_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtils.ToTitleCase(""));
    }

    [Test]
    public void AllCaseConversions_ShouldBeConsistent()
    {
        var testStrings = new[]
        {
            "HelloWorld",
            "userName",
            "API_RESPONSE",
            "kebab-case-string",
            "snake_case_string",
            "PascalCaseString"
        };

        foreach (var testString in testStrings)
        {
            var snake = StringUtils.ToSnakeCase(testString);
            var camel = StringUtils.ToCamelCase(testString);
            var pascal = StringUtils.ToPascalCase(testString);
            var kebab = StringUtils.ToKebabCase(testString);
            var upperSnake = StringUtils.ToUpperSnakeCase(testString);
            var title = StringUtils.ToTitleCase(testString);

            Assert.That(snake, Is.Not.Null.And.Not.Empty);
            Assert.That(camel, Is.Not.Null.And.Not.Empty);
            Assert.That(pascal, Is.Not.Null.And.Not.Empty);
            Assert.That(kebab, Is.Not.Null.And.Not.Empty);
            Assert.That(upperSnake, Is.Not.Null.And.Not.Empty);
            Assert.That(title, Is.Not.Null.And.Not.Empty);

            Assert.That(snake.All(c => !char.IsUpper(c)), Is.True);
            Assert.That(char.IsLower(camel[0]), Is.True);
            Assert.That(char.IsUpper(pascal[0]), Is.True);
            Assert.That(kebab.All(c => !char.IsUpper(c)), Is.True);
            Assert.That(upperSnake.All(c => !char.IsLower(c)), Is.True);
        }
    }
}
