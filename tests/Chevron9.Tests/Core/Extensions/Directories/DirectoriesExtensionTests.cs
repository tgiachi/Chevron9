using Chevron9.Core.Extensions.Directories;

namespace Chevron9.Tests.Core.Extensions.Directories;

[TestFixture]
public class DirectoriesExtensionTests
{
    [Test]
    public void ResolvePathAndEnvs_WithTildeHome_ShouldReplaceTilde()
    {
        var path = "~/Documents/test";
        var result = path.ResolvePathAndEnvs();
        
        var expectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents/test");
        Assert.That(result, Is.EqualTo(expectedPath));
    }

    [Test]
    public void ResolvePathAndEnvs_WithEnvironmentVariable_ShouldExpandVariable()
    {
        var testVarName = "TEST_PATH_VAR";
        var testVarValue = "test_value";
        Environment.SetEnvironmentVariable(testVarName, testVarValue);

        try
        {
            var path = $"%{testVarName}%/subfolder";
            var result = path.ResolvePathAndEnvs();
            
            Assert.That(result, Contains.Substring(testVarValue));
        }
        finally
        {
            Environment.SetEnvironmentVariable(testVarName, null);
        }
    }

    [Test]
    public void ResolvePathAndEnvs_WithDollarEnvironmentVariable_ShouldExpandVariable()
    {
        var testVarName = "TEST_DOLLAR_VAR";
        var testVarValue = "dollar_value";
        Environment.SetEnvironmentVariable(testVarName, testVarValue);

        try
        {
            var path = $"${testVarName}/subfolder";
            var result = path.ResolvePathAndEnvs();
            
            Assert.That(result, Contains.Substring(testVarValue));
        }
        finally
        {
            Environment.SetEnvironmentVariable(testVarName, null);
        }
    }

    [Test]
    public void ResolvePathAndEnvs_WithRegularPath_ShouldReturnUnchanged()
    {
        var path = "/regular/absolute/path";
        var result = path.ResolvePathAndEnvs();
        
        Assert.That(result, Is.EqualTo(path));
    }

    [Test]
    public void ResolvePathAndEnvs_WithNullPath_ShouldThrowArgumentException()
    {
        string nullPath = null;
        
        Assert.Throws<ArgumentException>(() => nullPath.ResolvePathAndEnvs());
    }

    [Test]
    public void ResolvePathAndEnvs_WithEmptyPath_ShouldThrowArgumentException()
    {
        var emptyPath = "";
        
        Assert.Throws<ArgumentException>(() => emptyPath.ResolvePathAndEnvs());
    }

    [Test]
    public void ResolvePathAndEnvs_WithWhitespacePath_ShouldThrowArgumentException()
    {
        var whitespacePath = "   ";
        
        Assert.Throws<ArgumentException>(() => whitespacePath.ResolvePathAndEnvs());
    }

    [TestCase("C:\\Windows\\System32")]
    [TestCase("/usr/local/bin")]
    [TestCase("./relative/path")]
    [TestCase("../parent/path")]
    public void ResolvePathAndEnvs_WithVariousPathFormats_ShouldNotThrow(string path)
    {
        Assert.DoesNotThrow(() => path.ResolvePathAndEnvs());
    }
}