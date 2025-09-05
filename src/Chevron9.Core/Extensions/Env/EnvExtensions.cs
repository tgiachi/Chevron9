using System.Collections;
using System.Text.RegularExpressions;

namespace Chevron9.Core.Extensions.Env;

public static class EnvExtensions
{
    public static string ExpandEnvironmentVariables(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // First expand existing environment variables
        foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
        {
            var key = $"${env.Key}";
            var value = env.Value?.ToString() ?? "";
            input = input.Replace(key, value);
        }

        // Then replace any remaining $VAR patterns with empty strings
        input = Regex.Replace(input, @"\$\w+", "");

        return input;
    }
}
