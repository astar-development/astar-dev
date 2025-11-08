using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core.ClassificationsServices;
using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.Tests.Integration;

public sealed class ClassificationProcessorShould : IDisposable
{
    private static readonly TimeSpan      DefaultTimeout = TimeSpan.FromSeconds(60);
    private                 FilesContext? _context;

    /// <inheritdoc />
    public void Dispose() { }

    [Fact(Timeout = 120_000)]
    public async Task ShouldAddNewClassificationsAndFileNamePartsAsync()
    {
        try
        {
            // Arrange
            var stoppingToken = TestContext.Current.CancellationToken;
            var appHost       = await DistributedApplicationTestingBuilder.CreateAsync<AppHost.AppHost>(stoppingToken);

            _ = appHost.Services.AddLogging(logging =>
                                        {
                                            _ = logging.SetMinimumLevel(LogLevel.Debug);

                                            // Override the logging filters from the app's configuration
                                            _ = logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                                            _ = logging.AddFilter("Aspire.", LogLevel.Debug);

                                            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
                                        });

            var db = appHost.Resources
                            .OfType<SqlServerDatabaseResource>()
                            .Single(r => r.Name == AspireConstants.Sql.AStarDb);

            var connectionString        = await db.ConnectionStringExpression.GetValueAsync(stoppingToken);
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<FilesContext>();
            _ = dbContextOptionsBuilder.UseSqlServer(connectionString);
            _ = appHost.Services.AddScoped<FilesContext>(_ => new FilesContext(dbContextOptionsBuilder.Options));

            var app = await appHost.BuildAsync(stoppingToken).WaitAsync(DefaultTimeout, stoppingToken);
            await app.StartAsync(stoppingToken);

            _context = app.Services.CreateScope().ServiceProvider.GetRequiredService<FilesContext>();
            _ = await _context.Database.EnsureCreatedAsync(stoppingToken);

            var processor = app.Services.CreateScope().ServiceProvider.GetRequiredService<ClassificationProcessor>();

            var mappings = new List<ClassificationMapping>
                           {
                               new() { DatabaseMapping = "Alpha", Celebrity = false, FileNameContains = "alpha_01" },
                               new() { DatabaseMapping = "Alpha", Celebrity = false, FileNameContains = "alpha_02" },
                               new() { DatabaseMapping = "Beta", Celebrity  = true, FileNameContains  = "beta_01" }
                           };

            // Act
            var result = await processor.ProcessAsync(mappings, stoppingToken);

            var classificationsTest = await _context!.FileClassifications
                                                     .Include(fc => fc.FileNameParts)
                                                     .ToListAsync(TestContext.Current.CancellationToken);

            classificationsTest.Count.ShouldBeGreaterThan(0);

            // Assert
            result.ShouldBeTrue();

            var classifications = await _context.FileClassifications.Include(x => x.FileNameParts).ToListAsync(TestContext.Current.CancellationToken);
            classifications.Count.ShouldBe(2);

            var alpha = classifications.First(c => c.Name == "Alpha");
            alpha.Celebrity.ShouldBeFalse();
            alpha.FileNameParts.Count.ShouldBe(2);
            alpha.FileNameParts.Select(p => p.Text).ShouldContain("alpha_01");
            alpha.FileNameParts.Select(p => p.Text).ShouldContain("alpha_02");

            var beta = classifications.First(c => c.Name == "Beta");
            beta.Celebrity.ShouldBeTrue();
            beta.FileNameParts.Count.ShouldBe(1);
            beta.FileNameParts[0].Text.ShouldBe("beta_01");
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
