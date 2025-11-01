using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;
using DbFileClassification = AStar.Dev.Infrastructure.FilesDb.Models.FileClassification;

namespace AStar.Dev.Files.Classifications.Api.Tests.Unit.Endpoints.FileClassifications.V1;

public class GetFileClassificationsHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Cap_ItemsPerPage_To_50()
    {
        using var context = BuildContextWith(Enumerable.Range(1, 100)
            .Select(i => new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = $"Item {i:D3}",
                Celebrity = i % 3 == 0,
                IncludeInSearch = i % 2 == 0
            }).ToArray());

        var handler = new GetFileClassificationsHandler();
        var request = new GetFileClassificationRequest(1, 100);

        var result = await handler.HandleAsync(request, context, CancellationToken.None);

        result.Count.ShouldBe(50);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Ordered_By_Name()
    {
        using var context = BuildContextWith(
            new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = "CHARLIE",
                Celebrity = false,
                IncludeInSearch = true
            },
            new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = "ALPHA",
                Celebrity = true,
                IncludeInSearch = false
            },
            new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = "BRAVO",
                Celebrity = true,
                IncludeInSearch = true
            }
        );

        var handler = new GetFileClassificationsHandler();
        var request = new GetFileClassificationRequest(1, 10);

        var result = await handler.HandleAsync(request, context, CancellationToken.None);

        result.Select(x => x.Name).ShouldBe(new[] { "ALPHA", "BRAVO", "CHARLIE" });
    }

    [Fact]
    public async Task HandleAsync_Should_Use_CurrentPage_Minus_One_For_Skip()
    {
        using var context = BuildContextWith(
            new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = "A",
                Celebrity = false,
                IncludeInSearch = true
            },
            new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = "B",
                Celebrity = true,
                IncludeInSearch = false
            },
            new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = "C",
                Celebrity = true,
                IncludeInSearch = true
            },
            new DbFileClassification
            {
                Id = Guid.CreateVersion7(),
                SearchLevel = 0,
                ParentId = null,
                Name = "D",
                Celebrity = false,
                IncludeInSearch = false
            }
        );

        var handler = new GetFileClassificationsHandler();
        var request = new GetFileClassificationRequest(2, 2);

        var result = await handler.HandleAsync(request, context, CancellationToken.None);

        // With Skip(CurrentPage - 1), page 2 should skip only one item, starting from "B"
        result.Select(x => x.Name).ShouldBe(new[] { "B", "C" });

        // Map fields are preserved
        var first = result.First();
        first.IncludeInSearch.ShouldBeFalse();
        first.Celebrity.ShouldBeTrue();
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
