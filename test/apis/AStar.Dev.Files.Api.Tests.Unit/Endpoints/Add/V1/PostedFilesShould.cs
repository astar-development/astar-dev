using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Test.Helpers.Minimal.Api;
using AStar.Dev.Test.Helpers.Unit;
using AStar.Dev.Utilities;
using NSubstitute;
using FileDetail = AStar.Dev.Infrastructure.FilesDb.Models.FileDetail;
using FilesContext = AStar.Dev.Infrastructure.FilesDb.Data.FilesContext;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

public class PostedFilesShould
{
    [Fact]
    public async Task ReturnBadRequestWhenTooManyFilesPostedAsync()
    {
        var tooManyFiles = Enumerable.Repeat(new FileDetailToAdd(), 101).ToList().AsReadOnly();
        var mockTime     = Substitute.For<TimeProvider>();
        var response     = await PostedFiles.HandleAsync( new() { FilesToAdd = tooManyFiles }, null!, mockTime, "Mock User", CancellationToken.None);

        response.ResultStatusCode().ShouldBe(400);
        response.ResultValue<string>().ShouldBe("Too many files supplied. Please try again with 100 files or less.");
    }

    [Fact]
    public async Task AddFilesToTheContextAsync()
    {
        var store       = new List<FileDetail>();
        var mockTime    = Substitute.For<TimeProvider>();
        var mockContext = DbContextMockFactory.CreateMockDbContext<FilesContext, FileDetail>(store, ctx => ctx.FileDetails);

        _ = await PostedFiles.HandleAsync(new () { FilesToAdd = [new ()] }, mockContext, mockTime, "Mock User", CancellationToken.None);

        await mockContext.FileDetails.Received(1).AddRangeAsync(Arg.Any<List<FileDetail>>());
    }

    [Fact]
    public async Task AddEventsToTheContextAsync()
    {
        var store       = new List<Event>();
        var mockTime    = Substitute.For<TimeProvider>();
        var mockContext = DbContextMockFactory.CreateMockDbContext<FilesContext, Event>(store, ctx => ctx.Events);

        _ = await PostedFiles.HandleAsync(new () { FilesToAdd = [new ()] }, mockContext, mockTime, "Mock User", CancellationToken.None);

        await mockContext.Events.Received(1).AddRangeAsync(Arg.Any<List<Event>>());
    }

    [Fact]
    public async Task ReturnTheExpectedResultObjectAsync()
    {
        var store       = new List<FileDetail>();
        var mockTime    = Substitute.For<TimeProvider>();
        var mockContext = DbContextMockFactory.CreateMockDbContext<FilesContext, FileDetail>(store, ctx => ctx.FileDetails);

        var response = await PostedFiles.HandleAsync(new () { FilesToAdd = [new ()] }, mockContext, mockTime, "Mock User", CancellationToken.None);

        response.ResultStatusCode().ShouldBe(201);
        response.ResultValue<List<AddFilesResponse>>().ToJson().ShouldMatchApproved();
    }
}
