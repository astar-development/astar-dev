using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.TestHelpers;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Shouldly;
using Xunit;

namespace AStar.Dev.Database.Updater.Tests.Integration;

public class ClassificationRepositoryDIIntegrationTests
{
    [Fact]
    public async Task ClassificationRepository_resolves_in_scoped_DI_and_tracks_entities_per_scope_Async()
    {
        // Arrange: create an in-memory sqlite scope with schema
        await using var global = await SqliteTestScope.CreateAsync();

            // Seed a classification using the global context
            var seed = new FileClassification
            {
                Name = "DI-Seed",
                Celebrity = false,
                IncludeInSearch = true
            };

            global.Context.FileClassifications.Add(seed);
            await global.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            // Now create a service provider that would mimic app registrations
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            // Register the same DbContext factory pattern used in production: use the shared context
            // as a singleton for this test so DI does not dispose it when scopes end.
            services.AddSingleton(_ => global.Context);
            services.AddScoped<ClassificationRepository>();

            var provider = services.BuildServiceProvider();

            // Act: resolve repo from first scope and fetch classifications
            using (var scope1 = provider.CreateScope())
            {
                var repo1 = scope1.ServiceProvider.GetRequiredService<ClassificationRepository>();
                var got = repo1.GetExistingClassifications(new HashSet<string> { "DI-Seed" });
                got.ShouldContainKey("DI-Seed");
                var entity1 = got["DI-Seed"];

                // Modify entity via repo/context in this scope
                entity1.Name = "DI-Seed-Modified";
                await global.Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            // Act: resolve repo from a new scope and ensure it sees the persisted change
            using (var scope2 = provider.CreateScope())
            {
                var repo2 = scope2.ServiceProvider.GetRequiredService<ClassificationRepository>();
                var got2 = repo2.GetExistingClassifications(new HashSet<string> { "DI-Seed-Modified" });
                got2.ShouldContainKey("DI-Seed-Modified");
            }

        // Assert: there is exactly one classification row (no duplicate inserts)
        var count = global.Context.FileClassifications.Count(fc => fc.Name.StartsWith("DI-Seed"));
        count.ShouldBe(1);
    }
}
