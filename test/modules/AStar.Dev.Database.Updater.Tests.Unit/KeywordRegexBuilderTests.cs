using AStar.Dev.Database.Updater.Core.FileDetailsServices;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class KeywordRegexBuilderTests
{
    [Fact]
    public void BuildKeywordPatternNormalizesUnderscoresAndDashes()
    {
        var inputs = new[] { new FileNamePartsWithClassifications { Text = "integration_keyword" }, new FileNamePartsWithClassifications { Text = "multi-word-key" } };

        var regex = KeywordRegexBuilder.BuildKeywordPattern(inputs);

        // Ensure the pattern matches the normalized phrases
        Assert.Matches(regex, "integration keyword");
        Assert.Matches(regex, "multi word key");
    }
}
