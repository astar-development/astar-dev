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
    public async Task ReturnBadRequestWhenTooManyFilesPosted()
    {
        var tooManyFiles = Enumerable.Repeat(new FileDetailToAdd(), 101).ToList().AsReadOnly();
        var mockTime     = Substitute.For<TimeProvider>();
        var response     = await PostedFiles.Handle( new() { FilesToAdd = tooManyFiles }, null!, mockTime, CancellationToken.None);

        response.ResultStatusCode().ShouldBe(400);
        response.ResultValue<string>().ShouldBe("Too many files supplied. Please try again with 100 files or less.");
    }

    [Fact]
    public async Task AddFilesToTheContext()
    {
        var store       = new List<FileDetail>();
        var mockTime    = Substitute.For<TimeProvider>();
        var mockContext = DbContextMockFactory.CreateMockDbContext<FilesContext, FileDetail>(store, ctx => ctx.Files);

        _ = await PostedFiles.Handle(new () { FilesToAdd = [new ()] }, mockContext, mockTime, CancellationToken.None);

        await mockContext.Received(1).AddRangeAsync(Arg.Any<List<FileDetail>>());
    }

    [Fact]
    public async Task ReturnTheExpectedResultObject()
    {
        var store       = new List<FileDetail>();
        var mockTime    = Substitute.For<TimeProvider>();
        var mockContext = DbContextMockFactory.CreateMockDbContext<FilesContext, FileDetail>(store, ctx => ctx.Files);

        var response = await PostedFiles.Handle(new () { FilesToAdd = [new ()] }, mockContext, mockTime, CancellationToken.None);

        response.ResultStatusCode().ShouldBe(201);
        response.ResultValue<List<AddFilesResponse>>().ToJson().ShouldMatchApproved();
    }
}
