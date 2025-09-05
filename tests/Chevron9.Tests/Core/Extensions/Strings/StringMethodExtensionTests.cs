using Chevron9.Core.Extensions.Strings;

namespace Chevron9.Tests.Core.Extensions.Strings;

[TestFixture]
public class StringMethodExtensionTests
{
    [TestCase("HelloWorld", "hello_world")]
    [TestCase("APIResponse", "api_response")]
    [TestCase("userId", "user_id")]
    [TestCase("A", "a")]
    public void ToSnakeCase_ShouldConvertCorrectly(string input, string expected)
    {
        var result = input.ToSnakeCase();
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("HelloWorld", "HELLO_WORLD")]
    [TestCase("apiResponse", "API_RESPONSE")]
    [TestCase("user-id", "USER_ID")]
    [TestCase("A", "A")]
    public void ToSnakeCaseUpper_ShouldConvertCorrectly(string input, string expected)
    {
        var result = input.ToSnakeCaseUpper();
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("HelloWorld", "helloWorld")]
    [TestCase("API_RESPONSE", "apiResponse")]
    [TestCase("user-id", "userId")]
    [TestCase("A", "a")]
    public void ToCamelCase_ShouldConvertCorrectly(string input, string expected)
    {
        var result = input.ToCamelCase();
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("hello_world", "HelloWorld")]
    [TestCase("api-response", "ApiResponse")]
    [TestCase("userId", "UserId")]
    [TestCase("a", "A")]
    public void ToPascalCase_ShouldConvertCorrectly(string input, string expected)
    {
        var result = input.ToPascalCase();
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("HelloWorld", "hello-world")]
    [TestCase("API_RESPONSE", "api-response")]
    [TestCase("userId", "user-id")]
    [TestCase("A", "a")]
    public void ToKebabCase_ShouldConvertCorrectly(string input, string expected)
    {
        var result = input.ToKebabCase();
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("hello_world", "Hello World")]
    [TestCase("API_RESPONSE", "Api Response")]
    [TestCase("user-id", "User Id")]
    [TestCase("camelCase", "Camel Case")]
    [TestCase("A", "A")]
    public void ToTitleCase_ShouldConvertCorrectly(string input, string expected)
    {
        var result = input.ToTitleCase();
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void AllExtensionMethods_WithNullString_ShouldThrow()
    {
        string nullString = null;

        Assert.Throws<ArgumentNullException>(() => nullString.ToSnakeCase());
        Assert.Throws<ArgumentNullException>(() => nullString.ToSnakeCaseUpper());
        Assert.Throws<ArgumentNullException>(() => nullString.ToCamelCase());
        Assert.Throws<ArgumentNullException>(() => nullString.ToPascalCase());
        Assert.Throws<ArgumentNullException>(() => nullString.ToKebabCase());
        Assert.Throws<ArgumentNullException>(() => nullString.ToTitleCase());
    }

    [Test]
    public void AllExtensionMethods_WithEmptyString_ShouldThrow()
    {
        var emptyString = "";

        Assert.Throws<ArgumentNullException>(() => emptyString.ToSnakeCase());
        Assert.Throws<ArgumentNullException>(() => emptyString.ToSnakeCaseUpper());
        Assert.Throws<ArgumentNullException>(() => emptyString.ToCamelCase());
        Assert.Throws<ArgumentNullException>(() => emptyString.ToPascalCase());
        Assert.Throws<ArgumentNullException>(() => emptyString.ToKebabCase());
        Assert.Throws<ArgumentNullException>(() => emptyString.ToTitleCase());
    }
}
