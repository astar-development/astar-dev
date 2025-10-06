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
                                        var escaped = Regex.Escape(fileNamePart.Text.Replace('-', ' ').Trim());

                                        return $@"\b{escaped}\b";
                                    });

        return $"({string.Join("|", parts)})";
    }
}
