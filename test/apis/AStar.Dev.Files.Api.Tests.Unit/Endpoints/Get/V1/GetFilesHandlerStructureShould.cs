using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using AStar.Dev.Test.Helpers.Minimal.Api;
using AStar.Dev.Utilities;
using NSubstitute;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

public class GetFilesHandlerStructureShould : IClassFixture<FilesContextFixture>
{
    private readonly FilesContextFixture mockFilesContextFactory;
    private readonly TimeProvider        mockTimeProvider;

    public GetFilesHandlerStructureShould(FilesContextFixture mockFilesContextFactory)
    {
        this.mockFilesContextFactory = mockFilesContextFactory;
        mockTimeProvider             = Substitute.For<TimeProvider>();
        mockTimeProvider.GetUtcNow().Returns(new DateTimeOffset(new(2025, 7, 13, 1, 2, 3, DateTimeKind.Utc)));
    }

    [Theory]
    [InlineData(@"\some\directory", true, 0, true, "", SortOrder.NameAscending, SearchType.All, 1, 10, 685)]
    public async Task ReturnTheExpectedStructureWhenCalledWithValidParameters(string    directoryName, bool recursive, int excludeViewedWithinDays, bool includeMarkedForDeletion, string searchText,
                                                                              SortOrder sortOrder,     SearchType searchType, int currentPage, int itemsPerPage, int
                                                                                  expectedCount)
    {
        await using var mockContext = mockFilesContextFactory.Sut;
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
        var getFilesResponses = response.ResultValue<IList<GetFilesResponse>>()!;
        getFilesResponses.Count.ShouldBe(expectedCount);
        getFilesResponses.ToJson().ShouldMatchApproved();
    }
}
