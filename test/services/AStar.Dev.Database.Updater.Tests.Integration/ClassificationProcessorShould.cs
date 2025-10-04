using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater;

public class ClassificationProcessorShould : IDisposable
{
    private static readonly TimeSpan      DefaultTimeout = TimeSpan.FromSeconds(60);
    private                 FilesContext? _context;

    /// <inheritdoc />
    public void Dispose()
        => _context?.Database.EnsureDeleted();

    [Fact(Timeout = 120_000)]
    public async Task Should_Add_New_Classifications_And_FileNameParts()
    {
        try
        {
            // Arrange
            var stoppingToken = TestContext.Current.CancellationToken;
            var appHost       = await DistributedApplicationTestingBuilder.CreateAsync<AppHost.AppHost>(stoppingToken);

            appHost.Services.AddLogging(logging =>
                                        {
                                            logging.SetMinimumLevel(LogLevel.Debug);

                                            // Override the logging filters from the app's configuration
                                            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                                            logging.AddFilter("Aspire.",                           LogLevel.Debug);

                                            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
                                        });

            var db = appHost.Resources
                            .OfType<SqlServerDatabaseResource>()
                            .Single(r => r.Name == AspireConstants.Sql.FilesDb);

            var connectionString        = await db.ConnectionStringExpression.GetValueAsync(stoppingToken);
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<FilesContext>();
            dbContextOptionsBuilder.UseSqlServer(connectionString);
            appHost.Services.AddScoped<FilesContext>(_ => new(dbContextOptionsBuilder.Options));

            var app = await appHost.BuildAsync(stoppingToken).WaitAsync(DefaultTimeout, stoppingToken);
            await app.StartAsync(stoppingToken);

            _context = app.Services.CreateScope().ServiceProvider.GetRequiredService<FilesContext>();
            await _context.Database.EnsureCreatedAsync(stoppingToken);

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

            var classifications = _context.FileClassifications.Include(x => x.FileNameParts).ToList();
            classifications.Count.ShouldBe(2);

            var alpha = classifications.First(c => c.Name == "Alpha");
            alpha.Celebrity.ShouldBeFalse();
            alpha.FileNameParts.Count.ShouldBe(2);
            alpha.FileNameParts.Select(p => p.Text).ShouldContain("alpha_01");
            alpha.FileNameParts.Select(p => p.Text).ShouldContain("alpha_02");

            var beta = classifications.First(c => c.Name == "Beta");
            beta.Celebrity.ShouldBeTrue();
            beta.FileNameParts.Count.ShouldBe(1);
            beta.FileNameParts.First().Text.ShouldBe("beta_01");
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
