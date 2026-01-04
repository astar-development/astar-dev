using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class SortOrderShould
{
    [Fact]
    public void ContainTheExpectedSizeDescendingValue()
        => Enum.IsDefined(typeof(SortOrder), 0).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedSizeAscendingValue()
        => Enum.IsDefined(typeof(SortOrder), 1).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedNameDescendingValue()
        => Enum.IsDefined(typeof(SortOrder), 2).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedNameAscendingValue()
        => Enum.IsDefined(typeof(SortOrder), 3).ShouldBeTrue();

    [Fact]
    public void ContainTheExpectedCountOfDefinedValues()
        => Enum.GetNames<SortOrder>().Length.ShouldBe(4);
}
