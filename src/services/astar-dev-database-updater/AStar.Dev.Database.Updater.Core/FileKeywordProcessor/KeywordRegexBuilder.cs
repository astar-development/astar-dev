using System.Text.RegularExpressions;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
///     Provides helpers for building keyword regex patterns.
/// </summary>
public static class KeywordRegexBuilder
{
    /// <summary>
    ///     Builds a regex pattern that matches both single-word and multi-word keywords.
    ///     Single words and multi-word phrases are wrapped in \b boundaries.
    /// </summary>
    public static string BuildKeywordPattern(IReadOnlyList<FileNamePartsWithClassifications> keywords)
    {
        var parts = keywords.Select(fileNamePart =>
                                    {
                                        // Normalize dashes and underscores to spaces to match the sanitizer used when checking file paths
                                        var normalized = fileNamePart.Text.Replace('-', ' ').Replace('_', ' ').Trim();
                                        var escaped    = Regex.Escape(normalized);

                                        return $@"\b{escaped}\b";
                                    });

        return $"({string.Join("|", parts)})";
    }
}
