using Chevron9.Core.Directories;

namespace Chevron9.Tests.Core.Directories;

[TestFixture]
public class DirectoriesConfigTests
{
    [SetUp]
    public void SetUp()
    {
        _tempRootPath = Path.Combine(Path.GetTempPath(), $"DirectoriesConfigTest_{Guid.NewGuid():N}");
        _directoriesConfig = new DirectoriesConfig(_tempRootPath, ["Logs", "Data", "Cache"]);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempRootPath))
        {
            Directory.Delete(_tempRootPath, true);
        }
    }

    private string _tempRootPath;
    private DirectoriesConfig _directoriesConfig;

    [Test]
    public void Constructor_ShouldCreateRootDirectory()
    {
        Assert.That(Directory.Exists(_tempRootPath), Is.True);
    }

    [Test]
    public void Constructor_ShouldCreateSpecifiedDirectories()
    {
        var logsPath = Path.Combine(_tempRootPath, "logs");
        var dataPath = Path.Combine(_tempRootPath, "data");
        var cachePath = Path.Combine(_tempRootPath, "cache");

        Assert.That(Directory.Exists(logsPath), Is.True);
        Assert.That(Directory.Exists(dataPath), Is.True);
        Assert.That(Directory.Exists(cachePath), Is.True);
    }

    [Test]
    public void Root_ShouldReturnRootDirectory()
    {
        Assert.That(_directoriesConfig.Root, Is.EqualTo(_tempRootPath));
    }

    [Test]
    public void StringIndexer_ShouldReturnCorrectPath()
    {
        var logsPath = _directoriesConfig["Logs"];
        var expectedPath = Path.Combine(_tempRootPath, "logs");

        Assert.That(logsPath, Is.EqualTo(expectedPath));
        Assert.That(Directory.Exists(logsPath), Is.True);
    }

    [Test]
    public void StringIndexer_WithNewDirectory_ShouldCreateAndReturnPath()
    {
        var newPath = _directoriesConfig["NewDirectory"];
        var expectedPath = Path.Combine(_tempRootPath, "new_directory");

        Assert.That(newPath, Is.EqualTo(expectedPath));
        Assert.That(Directory.Exists(newPath), Is.True);
    }

    [Test]
    public void EnumIndexer_ShouldReturnCorrectPath()
    {
        var logsPath = _directoriesConfig[TestDirectoryType.Logs];
        var expectedPath = Path.Combine(_tempRootPath, "logs");

        Assert.That(logsPath, Is.EqualTo(expectedPath));
        Assert.That(Directory.Exists(logsPath), Is.True);
    }

    [Test]
    public void GetPath_Generic_ShouldReturnCorrectPath()
    {
        var logsPath = _directoriesConfig.GetPath(TestDirectoryType.Logs);
        var expectedPath = Path.Combine(_tempRootPath, "logs");

        Assert.That(logsPath, Is.EqualTo(expectedPath));
        Assert.That(Directory.Exists(logsPath), Is.True);
    }

    [Test]
    public void GetPath_WithCamelCase_ShouldConvertToSnakeCase()
    {
        var path = _directoriesConfig.GetPath("TempFiles");
        var expectedPath = Path.Combine(_tempRootPath, "temp_files");

        Assert.That(path, Is.EqualTo(expectedPath));
        Assert.That(Directory.Exists(path), Is.True);
    }

    [Test]
    public void ToString_ShouldReturnRootPath()
    {
        Assert.That(_directoriesConfig.ToString(), Is.EqualTo(_tempRootPath));
    }

    private enum TestDirectoryType
    {
        Logs,
        Data,
        Cache
    }
}
