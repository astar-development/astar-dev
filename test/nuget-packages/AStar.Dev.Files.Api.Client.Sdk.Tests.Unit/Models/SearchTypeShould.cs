using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class SearchTypeShould
{
    [Fact]
    public void ContainTheExpectedImagesValue() =>
        Enum.IsDefined(typeof(SearchType), 0).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedAllValue() =>
        Enum.IsDefined(typeof(SearchType), 1).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedDuplicatesValue() =>
        Enum.IsDefined(typeof(SearchType), 2).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedDuplicateImagesValue() =>
        Enum.IsDefined(typeof(SearchType), 3).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedCountOfDefinedValues() =>
        Enum.GetNames<SearchType>().Length.ShouldBe(4);
}
