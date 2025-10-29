using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core.ClassificationsServices;
using AStar.Dev.Database.Updater.Core.FileDetailsServices;
using AStar.Dev.FileServices.Common;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Tests.Integration;

public sealed class FileScannerIntegrationShould : IDisposable
{
    private static readonly TimeSpan      DefaultTimeout = TimeSpan.FromSeconds(60);
    private                 FilesContext? _context;

    public void Dispose() { }

    [Fact(Timeout = 180_000)]
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

        _ = appHost.Services.AddScoped<ClassificationRepository>(sp => new(sp.GetRequiredService<FilesContext>()));
        _ = appHost.Services.AddScoped<FileHandleService>();

        var conn = await appHost.Resources.OfType<SqlServerDatabaseResource>().Single(r => r.Name == AspireConstants.Sql.AStarDb).ConnectionStringExpression.GetValueAsync(stoppingToken);

        _ = appHost.Services.AddScoped(sp =>
                                   {
                                       var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
                                       _ = optionsBuilder.UseSqlServer(conn);

                                       return new FilesContext(optionsBuilder.Options);
                                   });

        var app = await appHost.BuildAsync(stoppingToken).WaitAsync(DefaultTimeout, stoppingToken);
        await app.StartAsync(stoppingToken);

        _context = app.Services.CreateScope().ServiceProvider.GetRequiredService<FilesContext>();
        _ = await _context.Database.EnsureCreatedAsync(stoppingToken);

        // Arrange: ensure a classification exists in the DB
        var classification = new FileClassification { Name = "IntegrationTest", Celebrity = false, IncludeInSearch = true };
        classification.FileNameParts.Add(new() { Text = "integration_keyword" });
        _ = _context.FileClassifications.Add(classification);
        _ = await _context.SaveChangesAsync(stoppingToken);

        // Build a simple keyword provider that returns a single keyword part with its classification
        var keywordProvider = new TestKeywordProvider(["integration_keyword"], "IntegrationTest");

        // Create a FileDetail whose filename contains the keyword
        var fileDetail = new FileDetail
        {
            FileName = new("2025_integration_keyword.jpg"),
            DirectoryName = new() { Value = "/tmp" },
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
                FileNameParts = [new() { Text = k }]
            }).ToList();

        public Task<IReadOnlyList<FileNamePartsWithClassifications>> GetKeywordsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_keywords);
    }
}
