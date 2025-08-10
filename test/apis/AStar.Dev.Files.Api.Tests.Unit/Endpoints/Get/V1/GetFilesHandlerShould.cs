using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using AStar.Dev.Test.Helpers.Minimal.Api;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

public class GetFilesHandlerShould : IClassFixture<FilesContextFixture>
{
    private readonly FilesContextFixture mockFilesContextFactory;
    private readonly TimeProvider        mockTimeProvider;

    public GetFilesHandlerShould(FilesContextFixture mockFilesContextFactory)
    {
        this.mockFilesContextFactory = mockFilesContextFactory;
        mockTimeProvider             = Substitute.For<TimeProvider>();
        mockTimeProvider.GetUtcNow().Returns(new DateTimeOffset(new(2025, 7, 13, 1, 2, 3, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task ReturnBadRequestWhenDuplicatesAreRequested()
    {
        await using var mockContext = mockFilesContextFactory.Sut;
        var             sut         = new GetFilesHandler();

        var response = await sut.HandleAsync(new(), mockContext, mockTimeProvider, "Test User", CancellationToken.None);

        response.ShouldBe(Results.BadRequest());
    }

    [Theory]
    [InlineData(@"\some\directory", true, 0, true, "", SortOrder.NameAscending, SearchType.All, 1, 10)]
    public async Task ReturnOkWhenCalledWithValidParameters(string directoryName, bool recursive, int excludeViewedWithinDays, bool includeMarkedForDeletion, string searchText, SortOrder sortOrder,
                                                            SearchType searchType, int currentPage, int itemsPerPage)
    {
        await using var mockContext = mockFilesContextFactory.Sut; // this is from the integrations, I've messed up and need to find and revert some earlier updates or move the test
        var             sut         = new GetFilesHandler();

        var response = await
                           sut.HandleAsync(new()
                                           {
                                               SearchType               = searchType,
                                               SearchFolder             = directoryName,
                                               Recursive                = recursive,
                                               ExcludeViewedWithinDays  = excludeViewedWithinDays,
                                               IncludeMarkedForDeletion = includeMarkedForDeletion,
                                               SortOrder                = sortOrder,
                                               SearchText               = searchText,
                                               CurrentPage              = currentPage,
                                               ItemsPerPage             = itemsPerPage
                                           }, mockContext, mockTimeProvider, "Test User", CancellationToken.None);

        response.ResultStatusCode().ShouldBe(200);
    }
}
