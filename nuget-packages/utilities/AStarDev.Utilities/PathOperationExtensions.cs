using System.Text.RegularExpressions;

namespace AStarDev.Utilities;

/// <summary>
///     Provides extension methods for common path operations.
/// </summary> <remarks>
///     This class includes methods that enhance the functionality of string paths, such as safely combining multiple segments while preventing rooted paths from overriding the base path.
/// </remarks>
public static class PathOperationExtensions
{
    // CombinePath keeps the classic `this`-parameter form: a `params` array on an extension-block
    // member currently triggers a spurious CS8620 nullability mismatch at every call site (a known
    // C# 14 extension-member limitation), so it can't move into the CleanPath extension block below.

    /// <summary>
    ///     Combines a base path with one or more relative segments while preventing rooted segments from overriding earlier parts.
    /// </summary>
    /// <param name="basePath">The starting path to append segments to.</param>
    /// <param name="segments">The path segments to append. All segments must be relative.</param>
    /// <returns>The combined path.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="basePath" /> or <paramref name="segments" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any segment is rooted.</exception>
    public static string CombinePath(this string basePath, params string[] segments)
    {
        string combined = basePath;

        foreach (string? segment in segments.Where(s => s is not null))
        {
            combined = Path.Join(combined, segment);
        }

        return combined;
    }

    extension(string path)
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public string CleanPath()
        {
            char[] invalidFileChars = Path.GetInvalidPathChars();
            string cleaned = Regex.Replace(path, """[^\u0000-\u007F]+""", string.Empty, RegexOptions.Compiled, TimeSpan.FromSeconds(1));

            foreach (char invalidFileChar in invalidFileChars) cleaned = cleaned.Replace(invalidFileChar, ' ');

            return
                cleaned.Replace("\"", "'", StringComparison.OrdinalIgnoreCase)
                .Replace("|", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("煙", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
        }
    }
}
