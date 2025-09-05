namespace Chevron9.Core.Extensions.Directories;

public static class DirectoriesExtension
{
    public static string ResolvePathAndEnvs(this string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }

        // Only replace if tilde is present to avoid unnecessary allocations
        if (path.Contains('~'))
        {
            path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        // Expand environment variables (support both Windows %VAR% and Unix $VAR syntax)
        path = Environment.ExpandEnvironmentVariables(path);

        // Handle Unix-style $VAR syntax if not already expanded
        if (path.Contains('$'))
        {
            // Simple Unix-style variable expansion
            foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
            {
                var varName = envVar.ToString();
                if (varName is not null)
                {
                    var unixVar = $"${varName}";
                    if (path.Contains(unixVar))
                    {
                        var value = Environment.GetEnvironmentVariable(varName);
                        if (value is not null)
                        {
                            path = path.Replace(unixVar, value);
                        }
                    }
                }
            }
        }

        return path;
    }
}
