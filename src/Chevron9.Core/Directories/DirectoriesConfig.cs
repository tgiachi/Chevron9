using Chevron9.Core.Extensions.Strings;

namespace Chevron9.Core.Directories;

/// <summary>
/// Configuration class for managing application directory structure
/// Automatically creates directories and provides path resolution with snake_case conversion
/// </summary>
public class DirectoriesConfig
{
    private readonly string[] _directories;

    /// <summary>
    /// Initializes a new DirectoriesConfig with root directory and subdirectory list
    /// </summary>
    /// <param name="rootDirectory">Root directory path</param>
    /// <param name="directories">Array of subdirectory names to create</param>
    public DirectoriesConfig(string rootDirectory, string[] directories)
    {
        _directories = directories;
        Root = rootDirectory;

        Init();
    }

    /// <summary>
    /// Gets the root directory path
    /// </summary>
    public string Root { get; }

    /// <summary>
    /// Gets directory path by string key (converted to snake_case)
    /// </summary>
    /// <param name="directoryType">Directory type name</param>
    /// <returns>Full path to directory</returns>
    public string this[string directoryType] => GetPath(directoryType);


    /// <summary>
    /// Gets directory path by enum value (converted to snake_case)
    /// </summary>
    /// <param name="directoryType">Directory type enum</param>
    /// <returns>Full path to directory</returns>
    public string this[Enum directoryType] => GetPath(directoryType.ToString());

    /// <summary>
    /// Gets directory path by strongly-typed enum value
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <param name="value">Enum value</param>
    /// <returns>Full path to directory</returns>
    public string GetPath<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return GetPath(Enum.GetName(value));
    }

    /// <summary>
    /// Gets directory path by name, creating directory if it doesn't exist
    /// </summary>
    /// <param name="directoryType">Directory type name (will be converted to snake_case)</param>
    /// <returns>Full path to directory</returns>
    public string GetPath(string directoryType)
    {
        var path = Path.Combine(Root, directoryType.ToSnakeCase());

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private void Init()
    {
        if (!Directory.Exists(Root))
        {
            Directory.CreateDirectory(Root);
        }

        var directoryTypes = _directories.ToList();


        foreach (var path in directoryTypes.Select(GetPath)
                     .Where(path => !Directory.Exists(path)))
        {
            Directory.CreateDirectory(path);
        }
    }

    public override string ToString()
    {
        return Root;
    }
}
