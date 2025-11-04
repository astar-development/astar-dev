using AStar.Dev.Database.Updater.Core.ClassificationsServices;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.TestHelpers;

namespace AStar.Dev.Database.Updater.Tests.Integration;

public class ClassificationRepositoryDiIntegrationTests
{
    [Fact]
    public async Task ClassificationRepositoryResolvesInScopedDiAndTracksEntitiesPerScopeAsync()
    {
        // Arrange: create an in-memory sqlite scope with schema
        await using var global = await SqliteTestScope.CreateAsync();

        // Seed a classification using the global context
        var seed = new FileClassification { Name = "DI-Seed", Celebrity = false, IncludeInSearch = true };

        _ = global.Context.FileClassifications.Add(seed);
        _ = await global.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Now create a service provider that would mimic app registrations
        var services = new ServiceCollection();

        // Register the same DbContext factory pattern used in production: use the shared context
        // as a singleton for this test so DI does not dispose it when scopes end.
        _ = services.AddSingleton(_ => global.Context);
        _ = services.AddScoped<ClassificationRepository>();

        var provider = services.BuildServiceProvider();

        // Act: resolve repo from first scope and fetch classifications
        using(var scope1 = provider.CreateScope())
        {
            var repo1 = scope1.ServiceProvider.GetRequiredService<ClassificationRepository>();
            var got   = repo1.GetExistingClassifications(["DI-Seed"]);
            got.ShouldContainKey("DI-Seed");
            var entity1 = got["DI-Seed"];

            // Modify entity via repo/context in this scope
            entity1.Name = "DI-Seed-Modified";
            _ = await global.Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Act: resolve repo from a new scope and ensure it sees the persisted change
        using(var scope2 = provider.CreateScope())
        {
            var repo2 = scope2.ServiceProvider.GetRequiredService<ClassificationRepository>();
            var got2  = repo2.GetExistingClassifications(["DI-Seed-Modified"]);
            got2.ShouldContainKey("DI-Seed-Modified");
        }

        // Assert: there is exactly one classification row (no duplicate inserts)
        var count = global.Context.FileClassifications.Count(fc => fc.Name.StartsWith("DI-Seed"));
        count.ShouldBe(1);
    }
}
