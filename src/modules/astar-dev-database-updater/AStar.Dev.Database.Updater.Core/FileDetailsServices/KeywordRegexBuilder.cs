using System.Text;
using System.Text.RegularExpressions;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
///     Provides helpers for building keyword regex patterns.
/// </summary>
public static class KeywordRegexBuilder
{
    /// <summary>
    ///     Builds a regex pattern that matches both single-word and multi-word keywords.
    ///     Single words and multi-word phrases are wrapped in \b boundaries.
    /// </summary>
    public static Regex BuildKeywordPattern(IReadOnlyList<FileNamePartsWithClassifications> keywords)
    {
        if(keywords == null || keywords.Count == 0) return new Regex("()", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var parts = new string[keywords.Count];

        for(var i = 0; i < keywords.Count; i++)
        {
            var text = keywords[i].Text;
            if(string.IsNullOrWhiteSpace(text))
            {
                parts[i] = "\\b\\b"; // unlikely, but keep shape
                continue;
            }

            // Trim once, then replace '-' and '_' with spaces in a single pass to avoid multiple allocations
            var trimmed = text.Trim();
            var sb = new StringBuilder(trimmed.Length);

            for(var j = 0; j < trimmed.Length; j++)
            {
                var c = trimmed[j];
                var outChar = c is '-' or '_' ? ' ' : c;
                _ = sb.Append(outChar);
            }

            var normalized = sb.ToString();
            var escaped = Regex.Escape(normalized);
            parts[i] = "\\b" + escaped + "\\b";
        }

        var pattern = "(" + string.Join("|", parts) + ")";
        return new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
