using AStar.Dev.Database.Updater.Core.FileDetailsServices;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class RegexBuilderTests
{
    [Fact]
    public void SingleWordKeywordShouldMatchWholeWordOnly()
    {
        var regex = KeywordRegexBuilder.BuildKeywordPattern([new FileNamePartsWithClassifications { Text = "car" }]);

        regex.IsMatch("I bought a car yesterday").ShouldBeTrue();
        regex.IsMatch("car").ShouldBeTrue();

        // Negative cases
        regex.IsMatch("carrot").ShouldBeFalse();
        regex.IsMatch("scary").ShouldBeFalse();
    }

    [Fact]
    public void MultiWordKeywordShouldMatchPhrase()
    {
        var regex = KeywordRegexBuilder.BuildKeywordPattern([new FileNamePartsWithClassifications { Text = "red car" }]);

        regex.IsMatch("I saw a red car in the street").ShouldBeTrue();
        regex.IsMatch("red car").ShouldBeTrue();

        // Negative cases
        regex.IsMatch("red carpet").ShouldBeFalse();
        regex.IsMatch("car red").ShouldBeFalse();
    }

    [Fact]
    public void MixedKeywordsShouldHandleBothSingleAndMultiWord()
    {
        var regex = KeywordRegexBuilder.BuildKeywordPattern([
            new FileNamePartsWithClassifications { Text = "car" }, new FileNamePartsWithClassifications { Text = "red car" }, new FileNamePartsWithClassifications { Text = "blue truck" }
        ]);

        regex.IsMatch("The car is fast").ShouldBeTrue();
        regex.IsMatch("I saw a red car").ShouldBeTrue();
        regex.IsMatch("He drove a blue truck").ShouldBeTrue();

        // Negative cases
        regex.IsMatch("carrot soup").ShouldBeFalse();
        regex.IsMatch("red carpet event").ShouldBeFalse();
        regex.IsMatch("truck stop").ShouldBeFalse();
    }

    [Fact]
    public void CaseInsensitiveShouldMatchRegardlessOfCase()
    {
        var regex = KeywordRegexBuilder.BuildKeywordPattern([new FileNamePartsWithClassifications { Text = "red car" }]);

        regex.IsMatch("RED CAR").ShouldBeTrue();
        regex.IsMatch("Red Car").ShouldBeTrue();
        regex.IsMatch("rEd cAr").ShouldBeTrue();
    }

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
