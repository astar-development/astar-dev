using System.Text.RegularExpressions;
using AStar.Dev.Database.Updater.Core.FileDetailsServices;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class RegexBuilderTests
{
    [Fact]
    public void SingleWordKeyword_ShouldMatchWholeWordOnly()
    {
        var pattern = KeywordRegexBuilder.BuildKeywordPattern([new() { Text = "car" }]);
        var regex   = new Regex(pattern, RegexOptions.IgnoreCase);

        regex.IsMatch("I bought a car yesterday").ShouldBeTrue();
        regex.IsMatch("car").ShouldBeTrue();

        // Negative cases
        regex.IsMatch("carrot").ShouldBeFalse();
        regex.IsMatch("scary").ShouldBeFalse();
    }

    [Fact]
    public void MultiWordKeyword_ShouldMatchPhrase()
    {
        var pattern = KeywordRegexBuilder.BuildKeywordPattern([new() { Text = "red car" }]);
        var regex   = new Regex(pattern, RegexOptions.IgnoreCase);

        regex.IsMatch("I saw a red car in the street").ShouldBeTrue();
        regex.IsMatch("red car").ShouldBeTrue();

        // Negative cases
        regex.IsMatch("red carpet").ShouldBeFalse();
        regex.IsMatch("car red").ShouldBeFalse();
    }

    [Fact]
    public void MixedKeywords_ShouldHandleBothSingleAndMultiWord()
    {
        var pattern = KeywordRegexBuilder.BuildKeywordPattern([new() { Text = "car" }, new() { Text = "red car" }, new() { Text = "blue truck" }]);
        var regex   = new Regex(pattern, RegexOptions.IgnoreCase);

        regex.IsMatch("The car is fast").ShouldBeTrue();
        regex.IsMatch("I saw a red car").ShouldBeTrue();
        regex.IsMatch("He drove a blue truck").ShouldBeTrue();

        // Negative cases
        regex.IsMatch("carrot soup").ShouldBeFalse();
        regex.IsMatch("red carpet event").ShouldBeFalse();
        regex.IsMatch("truck stop").ShouldBeFalse();
    }

    [Fact]
    public void CaseInsensitive_ShouldMatchRegardlessOfCase()
    {
        var pattern = KeywordRegexBuilder.BuildKeywordPattern([new() { Text = "red car" }]);
        var regex   = new Regex(pattern, RegexOptions.IgnoreCase);

        regex.IsMatch("RED CAR").ShouldBeTrue();
        regex.IsMatch("Red Car").ShouldBeTrue();
        regex.IsMatch("rEd cAr").ShouldBeTrue();
    }

    [Fact]
    public void BuildKeywordPattern_NormalizesUnderscoresAndDashes()
    {
        var inputs = new[] { new FileNamePartsWithClassifications { Text = "integration_keyword" }, new FileNamePartsWithClassifications { Text = "multi-word-key" } };

        var pattern = KeywordRegexBuilder.BuildKeywordPattern(inputs);

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        // Ensure the pattern matches the normalized phrases
        Assert.Matches(regex, "integration keyword");
        Assert.Matches(regex, "multi word key");
    }
}
