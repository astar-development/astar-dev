using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

public class GetFilesHandlerShould
{
    private readonly TimeProvider mockTimeProvider;

    public GetFilesHandlerShould()
    {
        mockTimeProvider = Substitute.For<TimeProvider>();
        mockTimeProvider.GetUtcNow().Returns(new DateTimeOffset(new (2025, 7, 13, 1, 2, 3, DateTimeKind.Utc)));
    }

    [Fact(Skip = "Once happy with the build running test, come back to this")]
    public async Task ReturnBadRequestWhenDuplicatesAreRequested()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer(connection);
        var mockContext = new FilesContext(optionsBuilder.Options);
        await mockContext.Database.EnsureCreatedAsync();

        var sut = new GetFilesHandler();

        var response = await sut.HandleAsync(new (), mockContext, mockTimeProvider, "Test User", CancellationToken.None);

        response.ShouldBe(Results.BadRequest());
    }
}
