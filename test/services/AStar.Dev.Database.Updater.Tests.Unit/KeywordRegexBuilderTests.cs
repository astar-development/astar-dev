using AStar.Dev.Database.Updater.Core.FileKeywordProcessor;
using Xunit;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class KeywordRegexBuilderTests
{
    [Fact]
    public void BuildKeywordPattern_NormalizesUnderscoresAndDashes()
    {
        var inputs = new[] { new FileNamePartsWithClassifications { Text = "integration_keyword" },
                             new FileNamePartsWithClassifications { Text = "multi-word-key" } };

        var pattern = KeywordRegexBuilder.BuildKeywordPattern(inputs);

    var regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    // Ensure the pattern matches the normalized phrases
    Assert.Matches(regex, "integration keyword");
    Assert.Matches(regex, "multi word key");
    }
}
