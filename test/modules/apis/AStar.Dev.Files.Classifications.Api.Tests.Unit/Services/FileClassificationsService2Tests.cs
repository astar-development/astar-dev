using AStar.Dev.Files.Classifications.Api.Services;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;
using DbFileClassification = AStar.Dev.Infrastructure.FilesDb.Models.FileClassification;

namespace AStar.Dev.Files.Classifications.Api.Tests.Unit.Services;

public class FileClassificationsService2Tests
{
    [Fact]
    public async Task GetFileClassificationsAsync_Should_Map_Ef_Entity_To_Record()
    {
        var id = Guid.CreateVersion7();
        var parentId = Guid.CreateVersion7();

        await using var context = BuildContextWith(
            new DbFileClassification
            {
                Id = id,
                SearchLevel = 7,
                ParentId = parentId,
                Name = "Mapped",
                Celebrity = true,
                IncludeInSearch = false
            }
        );

        var sut = new FileClassificationsService2(context);

        var result = await sut.GetFileClassificationsAsync();

        result.Count.ShouldBe(1);

        var item = result.Single();
        item.Id.ShouldBe(id);
        item.SearchLevel.ShouldBe(7);
        item.ParentId.ShouldBe(parentId);
        item.Name.ShouldBe("Mapped");
        item.Celebrity.ShouldBeTrue();
        item.IncludeInSearch.ShouldBeFalse();
    }

    [Fact]
    public async Task GetFileClassificationsAsync_Should_Return_Empty_When_No_Entities()
    {
        await using var context = BuildContextWith();

        var sut = new FileClassificationsService2(context);

        var result = await sut.GetFileClassificationsAsync();

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetFileClassificationsAsync_Should_Map_Multiple_Entities_Including_Null_Parent()
    {
        var id1 = Guid.CreateVersion7();
        var id2 = Guid.CreateVersion7();
        var parentId = Guid.CreateVersion7();

        await using var context = BuildContextWith(
            new DbFileClassification
            {
                Id = id1,
                SearchLevel = 1,
                ParentId = null,
                Name = "Beta",
                Celebrity = false,
                IncludeInSearch = true
            },
            new DbFileClassification
            {
                Id = id2,
                SearchLevel = 2,
                ParentId = parentId,
                Name = "Alpha",
                Celebrity = true,
                IncludeInSearch = false
            }
        );

        var sut = new FileClassificationsService2(context);

        var result = await sut.GetFileClassificationsAsync();

        result.Count.ShouldBe(2);

        // Order results for deterministic assertions
        var ordered = result.OrderBy(x => x.Name).ToArray();

        ordered[0].Name.ShouldBe("Alpha");
        ordered[0].Id.ShouldBe(id2);
        ordered[0].SearchLevel.ShouldBe(2);
        ordered[0].ParentId.ShouldBe(parentId);
        ordered[0].Celebrity.ShouldBeTrue();
        ordered[0].IncludeInSearch.ShouldBeFalse();

        ordered[1].Name.ShouldBe("Beta");
        ordered[1].Id.ShouldBe(id1);
        ordered[1].SearchLevel.ShouldBe(1);
        ordered[1].ParentId.ShouldBeNull();
        ordered[1].Celebrity.ShouldBeFalse();
        ordered[1].IncludeInSearch.ShouldBeTrue();
    }

    private static FilesContext BuildContextWith(params DbFileClassification[] items)
    {
        var options = new DbContextOptionsBuilder<FilesContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var ctx = new FilesContext(options);
        ctx.FileClassifications.AddRange(items);
        ctx.SaveChanges();
        return ctx;
    }
}
