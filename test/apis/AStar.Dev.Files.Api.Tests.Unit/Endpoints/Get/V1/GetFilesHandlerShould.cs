using AStar.Dev.Test.Helpers.Minimal.Api;
using AStar.Dev.Utilities;
using DbContextHelpers.Fixtures;
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
        mockTimeProvider.GetUtcNow().Returns(new DateTimeOffset(new (2025, 7, 13, 1, 2, 3, DateTimeKind.Utc)));
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
    [InlineData(@"c:\temp", true,  0, true,  "",  SortOrder.NameAscending,  SearchType.All, 1, 10, 10)]
    public async Task ReturnTheExpectedStructureWhenCalledWithValidParameters(string    directoryName, bool recursive, int excludeViewedWithinDays, bool includeMarkedForDeletion, string searchText,
                                                                              SortOrder sortOrder,     SearchType searchType, int currentPage, int itemsPerPage, int
                                                                                  expectedCount)
    {
        await using var mockContext = mockFilesContextFactory.SutWithFileDetails;
        var             sut         = new GetFilesHandler();

        var response =
            await
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

    [Theory]
    [InlineData(@"c:\temp", false,  0, true,  "",  SortOrder.NameAscending,  SearchType.All, 1, 10, 10)]
    public async Task ReturnTheExpectedStructureWhenCalledWithValidParameters2(string    directoryName, bool recursive, int excludeViewedWithinDays, bool includeMarkedForDeletion, string searchText,
                                                                               SortOrder sortOrder,     SearchType searchType, int currentPage, int itemsPerPage, int
                                                                                   expectedCount)
    {
        await using var mockContext = mockFilesContextFactory.SutWithFileDetails;
        var             sut         = new GetFilesHandler();

        var response =
            await
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

    [Theory]
    [InlineData(@"c:\temp", true,  5, true,  "",  SortOrder.NameAscending,  SearchType.All, 1, 10, 10)]
    public async Task ReturnTheExpectedStructureWhenCalledWithValidParameters3(string    directoryName, bool recursive, int excludeViewedWithinDays, bool includeMarkedForDeletion, string searchText,
                                                                               SortOrder sortOrder,     SearchType searchType, int currentPage, int itemsPerPage, int
                                                                                   expectedCount)
    {
        await using var mockContext = mockFilesContextFactory.SutWithFileDetails;
        var             sut         = new GetFilesHandler();

        var response =
            await
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

    [Theory]
    [InlineData(@"c:\temp", false, 5, true,  "",  SortOrder.NameAscending,  SearchType.All, 1, 10, 10)]
    public async Task ReturnTheExpectedStructureWhenCalledWithValidParameters4(string    directoryName, bool recursive, int excludeViewedWithinDays, bool includeMarkedForDeletion, string searchText,
                                                                               SortOrder sortOrder,     SearchType searchType, int currentPage, int itemsPerPage, int
                                                                                   expectedCount)
    {
        await using var mockContext = mockFilesContextFactory.SutWithFileDetails;
        var             sut         = new GetFilesHandler();

        var response =
            await
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
