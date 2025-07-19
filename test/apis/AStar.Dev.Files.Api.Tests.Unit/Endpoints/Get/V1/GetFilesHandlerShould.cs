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

        var response = await sut.HandleAsync(new (), mockContext, mockTimeProvider, "Test User", CancellationToken.None);

        response.ShouldBe(Results.BadRequest());
    }
}
