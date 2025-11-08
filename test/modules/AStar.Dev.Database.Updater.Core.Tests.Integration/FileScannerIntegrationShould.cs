using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core.FileDetailsServices;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.Tests.Integration;

public sealed class FileScannerIntegrationShould : IDisposable
{
    private static readonly TimeSpan      DefaultTimeout = TimeSpan.FromSeconds(60);
    private                 FilesContext? _context;

    public void Dispose() { }

    [Fact(Timeout = 180_000, Skip = "Integration test which doesn't . see https://endjin.com/blog/2025/06/dotnet-aspire-db-testing-use-sqlconnection-from-test and associated parts")]
    public async Task ShouldSaveFileDetailWithExistingClassificationLinkedAsync()
    {
        var stoppingToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<AppHost.AppHost>(stoppingToken);

        _ = appHost.Services.AddLogging(logging =>
                                    {
                                        _ = logging.SetMinimumLevel(LogLevel.Debug);
                                        _ = logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                                        _ = logging.AddFilter("Aspire.", LogLevel.Debug);
                                    });

        var app = await appHost.BuildAsync(stoppingToken).WaitAsync(DefaultTimeout, stoppingToken);
        await app.StartAsync(stoppingToken);

        var resourceNotificationService =
            app.Services.GetRequiredService<ResourceNotificationService>();
        await resourceNotificationService
            .WaitForResourceAsync(AspireConstants.Sql.AStarDb, KnownResourceStates.Running, stoppingToken)
            .WaitAsync(TimeSpan.FromSeconds(30), stoppingToken);

        var db = appHost.Resources
            .OfType<SqlServerDatabaseResource>()
            .Single(r => r.Name == AspireConstants.Sql.AStarDb);
        var sqlConnection = await db.ConnectionStringExpression.GetValueAsync(
            CancellationToken.None);

        var ui = appHost.Resources
            .OfType<ProjectResource>()
            .Single(r => r.Name == AspireConstants.Ui);

        var endpoints = ui.GetEndpoints();
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer(sqlConnection);
        _context = new FilesContext(optionsBuilder.Options);
        //context = app.Services.CreateScope().ServiceProvider.GetRequiredService<FilesContext>();
        _ = await _context.Database.EnsureCreatedAsync(stoppingToken);

        // Arrange: ensure a classification exists in the DB
        var classification = new FileClassification { Name = "IntegrationTest", Celebrity = false, IncludeInSearch = true };
        classification.FileNameParts.Add(new FileNamePart { Text = "integration_keyword" });
        _ = _context.FileClassifications.Add(classification);
        _ = await _context.SaveChangesAsync(stoppingToken);

        // Build a simple keyword provider that returns a single keyword part with its classification
        var keywordProvider = new TestKeywordProvider(["integration_keyword"], "IntegrationTest");

        // Create a FileDetail whose filename contains the keyword
        var fileDetail = new FileDetail
        {
            FileName = new FileName("2025_integration_keyword.jpg"),
            DirectoryName = new DirectoryName { Value = "/tmp" },
            FileSize = 123,
            CreatedDate = DateTimeOffset.UtcNow,
            UpdatedDate = DateTimeOffset.UtcNow,
            FileHandle = FileHandle.Create("test-handle")
        };

        using var testScope                   = app.Services.CreateScope();
        var       fileDetailsProcessorService = testScope.ServiceProvider.GetRequiredService<FileDetailsProcessorService>();
        var       logger                      = testScope.ServiceProvider.GetRequiredService<ILogger<FilesProcessor>>();
        var       filesContext                = testScope.ServiceProvider.GetRequiredService<FilesContext>();

        var scanner = new FilesProcessor(filesContext, keywordProvider, fileDetailsProcessorService, logger);

        // Act
        _ = await scanner.ProcessAsync([fileDetail], stoppingToken);

        // Re-query the file details from the DB and ensure the classification link exists
        var files = await _context.Files.Include(f => f.FileClassifications)
                                  .ToListAsync(stoppingToken);

        var savedFile = files.FirstOrDefault(f => f.FileName.Value == fileDetail.FileName.Value);

        _ = savedFile.ShouldNotBeNull();
        _ = savedFile.FileClassifications.ShouldNotBeNull();
        savedFile.FileClassifications.Any(fc => fc.Name == "IntegrationTest").ShouldBeTrue();
    }

    private class TestKeywordProvider : IKeywordProvider
    {
        private readonly IReadOnlyList<FileNamePartsWithClassifications> _keywords;

        public TestKeywordProvider(IEnumerable<string> keywords, string classificationName)
            => _keywords = keywords.Select(k => new FileNamePartsWithClassifications
            {
                Text = k,
                Name = classificationName,
                Celebrity = false,
                IncludeInSearch = true,
                FileNameParts = [new FileNamePart { Text = k }]
            }).ToList();

        public Task<IReadOnlyList<FileNamePartsWithClassifications>> GetKeywordsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_keywords);
    }
}
