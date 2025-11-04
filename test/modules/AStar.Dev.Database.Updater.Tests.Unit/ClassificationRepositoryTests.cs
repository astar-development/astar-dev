using AStar.Dev.Database.Updater.Core.ClassificationsServices;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.TestHelpers;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class ClassificationRepositoryTests
{
    // Use shared SqliteTestScope helper for context lifecycle and in-memory DB

    [Fact]
    public async Task GetExistingClassificationsByNamesReturnsRequestedAsync()
    {
        // Arrange - open shared in-memory sqlite connection
        await using var scope = await SqliteTestScope.CreateAsync();
        var             ctx   = scope.Context;
        var             repo  = new ClassificationRepository(ctx);

        var c1 = new FileClassification { Name = "CatA", Celebrity = false, IncludeInSearch = true };
        c1.FileNameParts.Add(new FileNamePart { Text = "a" });
        var c2 = new FileClassification { Name = "CatB", Celebrity = false, IncludeInSearch = true };
        c2.FileNameParts.Add(new FileNamePart { Text = "b" });
        var c3 = new FileClassification { Name = "CatC", Celebrity = false, IncludeInSearch = true };
        c3.FileNameParts.Add(new FileNamePart { Text = "c" });

        ctx.FileClassifications.AddRange(c1, c2, c3);
        _ = await ctx.SaveChangesAsync(CancellationToken.None);

        // Act
        var names  = new HashSet<string> { "CatA", "CatC" };
        var result = repo.GetExistingClassifications(names);

        // Assert
        result.Count.ShouldBe(2);
        result.ContainsKey("CatA").ShouldBeTrue();
        result.ContainsKey("CatC").ShouldBeTrue();
        result["CatA"].FileNameParts.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetExistingClassificationsNoArgsReturnsAllAsync()
    {
        // Arrange - open shared in-memory sqlite connection
        await using var scope = await SqliteTestScope.CreateAsync();
        var             ctx   = scope.Context;
        var             repo  = new ClassificationRepository(ctx);

        var c1 = new FileClassification { Name = "X", Celebrity = false, IncludeInSearch = true };
        var c2 = new FileClassification { Name = "Y", Celebrity = false, IncludeInSearch = true };

        ctx.FileClassifications.AddRange(c1, c2);
        _ = await ctx.SaveChangesAsync(CancellationToken.None);

        // Act
        var all = repo.GetExistingClassifications();

        // Assert
        all.Count.ShouldBeGreaterThanOrEqualTo(2);
        all.Any(fc => fc.Name == "X").ShouldBeTrue();
        all.Any(fc => fc.Name == "Y").ShouldBeTrue();
    }

    [Fact]
    public async Task AddClassificationsAndSavePersistsEntitiesAsync()
    {
        // Arrange - open shared in-memory sqlite connection
        await using var scope = await SqliteTestScope.CreateAsync();
        var             ctx   = scope.Context;
        var             repo  = new ClassificationRepository(ctx);

        var newClassifications = new[]
                                 {
                                     new FileClassification { Name = "New1", Celebrity = false, IncludeInSearch = true },
                                     new FileClassification { Name = "New2", Celebrity = false, IncludeInSearch = true }
                                 };

        // Act
        repo.AddClassifications(newClassifications);
        await repo.SaveChangesAsync(CancellationToken.None);

        // Assert
        ctx.FileClassifications.Count().ShouldBeGreaterThanOrEqualTo(2);
        ctx.FileClassifications.Any(fc => fc.Name == "New1").ShouldBeTrue();
        ctx.FileClassifications.Any(fc => fc.Name == "New2").ShouldBeTrue();
    }

    [Fact]
    public async Task GetExistingClassificationsEmptyNamesReturnsEmptyAsync()
    {
        await using var scope = await SqliteTestScope.CreateAsync();
        var             ctx   = scope.Context;
        var             repo  = new ClassificationRepository(ctx);

        var result = repo.GetExistingClassifications([]);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetExistingClassificationsReturnsTrackedEntitiesWhenContextIsSameScopeAsync()
    {
        await using var scope = await SqliteTestScope.CreateAsync();
        var             ctx   = scope.Context;

        var classification = new FileClassification { Name = "TrackedTest", Celebrity = false, IncludeInSearch = true };
        _ = ctx.FileClassifications.Add(classification);
        _ = await ctx.SaveChangesAsync(CancellationToken.None);

        var repo            = new ClassificationRepository(ctx);
        var classifications = repo.GetExistingClassifications(["TrackedTest"]);

        var fetched = classifications["TrackedTest"];

        // Modify fetched entity and Save - changes should be tracked by same context
        fetched.Name = "TrackedTestRenamed";
        _ = await ctx.SaveChangesAsync(CancellationToken.None);

        // Re-query directly from context to confirm change persisted
        ctx.FileClassifications.Any(fc => fc.Name == "TrackedTestRenamed").ShouldBeTrue();
    }
}
