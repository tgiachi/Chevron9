using Chevron9.Bootstrap.Types;

namespace Chevron9.Bootstrap.Data.Configs;

public class Chevron9Config
{
    public string RootDirectory { get; set; }

    public string[] DefaultDirectories { get; set; } = [];

    public string Title { get; set; }

    public LogLevelType LogLevel { get; set; } = LogLevelType.Information;


    public bool LogToFile { get; set; } = true;

    public bool LogToConsole { get; set; }
}
