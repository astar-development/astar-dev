using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class SearchParametersShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new SearchParameters
            {
                CurrentPage              = 1,
                ExcludedViewSettings     = new () { ExcludeViewed = true, ExcludeViewedPeriodInDays = 12 },
                IncludeMarkedForDeletion = true,
                IncludeSoftDeleted       = true,
                ItemsPerPage             = 12,
                MaximumSizeOfImage       = 123,
                MaximumSizeOfThumbnail   = 456,
                Recursive                = true,
                SearchFolder             = "MockSearchFolder",
                SearchText               = "MockSearchText",
                SearchType               = SearchType.All,
                SortOrder                = SortOrder.NameAscending
            }
            .ToString()
            .ShouldMatchApproved();

    [Fact]
    public void ReturnTheExpectedToQueryString() =>
        new SearchParameters
            {
                CurrentPage              = 1,
                ExcludedViewSettings     = new () { ExcludeViewed = true, ExcludeViewedPeriodInDays = 12 },
                IncludeMarkedForDeletion = true,
                IncludeSoftDeleted       = true,
                ItemsPerPage             = 12,
                MaximumSizeOfImage       = 123,
                MaximumSizeOfThumbnail   = 456,
                Recursive                = true,
                SearchFolder             = "MockSearchFolder",
                SearchText               = "MockSearchText",
                SearchType               = SearchType.All,
                SortOrder                = SortOrder.NameAscending
            }
            .ToQueryString()
            .ShouldMatchApproved();
}
