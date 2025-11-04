using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.Models;

public sealed class SearchParametersShould
{
    [Fact]
    public void ReturnTheExpectedToString()
        => new SearchParameters().ToString().ShouldMatchApproved();

    [Fact]
    public void ReturnTheExpectedToQueryString()
        => new SearchParameters().ToQueryString().ShouldMatchApproved();
}
