using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Database.Updater.Core.FileKeywordProcessor;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater;

public class FileScannerIntegrationShould : IDisposable
{
    private static readonly TimeSpan      DefaultTimeout = TimeSpan.FromSeconds(60);
    private                 FilesContext? _context;

    public void Dispose()
        => _context?.Database.EnsureDeleted();

    [Fact(Timeout = 120_000)]
    public async Task Should_Save_FileDetail_With_Existing_Classification_LinkedAsync()
    {
        var stoppingToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<AppHost.AppHost>(stoppingToken);

        appHost.Services.AddLogging(logging =>
                                    {
                                        logging.SetMinimumLevel(LogLevel.Debug);
                                        logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                                        logging.AddFilter("Aspire.",                           LogLevel.Debug);
                                    });

        appHost.Services.AddScoped<ClassificationRepository>(sp => new(sp.GetRequiredService<FilesContext>()));

        var db = appHost.Resources.OfType<SqlServerDatabaseResource>().Single(r => r.Name == AspireConstants.Sql.FilesDb);

        appHost.Services.AddScoped(_ =>
                                   {
                                       var conn           = db.ConnectionStringExpression.GetValueAsync(stoppingToken).GetAwaiter().GetResult();
                                       var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
                                       optionsBuilder.UseSqlServer(conn);

                                       return new FilesContext(optionsBuilder.Options);
                                   });

        var app = await appHost.BuildAsync(stoppingToken).WaitAsync(DefaultTimeout, stoppingToken);
        await app.StartAsync(stoppingToken);

        _context = app.Services.CreateScope().ServiceProvider.GetRequiredService<FilesContext>();
        await _context.Database.EnsureCreatedAsync(stoppingToken);

        // Arrange: ensure a classification exists in the DB
        var classification = new FileClassification { Name = "IntegrationTest", Celebrity = false, IncludeInSearch = true };
        classification.FileNameParts.Add(new() { Text      = "integration_keyword" });
        _context.FileClassifications.Add(classification);
        await _context.SaveChangesAsync(stoppingToken);

        // Build a simple keyword provider that returns a single keyword part with its classification
        var keywordProvider = new TestKeywordProvider(["integration_keyword"], "IntegrationTest");

        // Create a FileDetail whose filename contains the keyword
        var fileDetail = new FileDetail
                         {
                             FileName      = new("2025_integration_keyword.jpg"),
                             DirectoryName = new() { Value = "/tmp" },
                             FileSize      = 123,
                             CreatedDate   = DateTimeOffset.UtcNow,
                             UpdatedDate   = DateTimeOffset.UtcNow,
                             FileHandle    = FileHandle.Create("test-handle")
                         };

        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

        using var testScope                = app.Services.CreateScope();
        var       classificationRepository = testScope.ServiceProvider.GetRequiredService<ClassificationRepository>();
        var       logger                   = testScope.ServiceProvider.GetRequiredService<ILogger<FileScanner>>();

        var scanner = new FileScanner(scopeFactory, classificationRepository, keywordProvider, logger);

        // Act
        await scanner.ScanFilesAsync([fileDetail], stoppingToken);

        // Re-query the file details from the DB and ensure the classification link exists
        var files = await _context.Files.Include(f => f.FileClassifications)
                                  .ToListAsync(stoppingToken);

        var savedFile = files.FirstOrDefault(f => f.FileName.Value == fileDetail.FileName.Value);

        savedFile.ShouldNotBeNull();
        savedFile!.FileClassifications.ShouldNotBeNull();
        savedFile.FileClassifications.Any(fc => fc.Name == "IntegrationTest").ShouldBeTrue();
    }

    private class TestKeywordProvider : IKeywordProvider
    {
        private readonly IReadOnlyList<FileNamePartsWithClassifications> _keywords;

        public TestKeywordProvider(IEnumerable<string> keywords, string classificationName) => _keywords = keywords.Select(k => new FileNamePartsWithClassifications
                                                                                                                                {
                                                                                                                                    Text            = k,
                                                                                                                                    Name            = classificationName,
                                                                                                                                    Celebrity       = false,
                                                                                                                                    IncludeInSearch = true,
                                                                                                                                    FileNameParts   = [new() { Text = k }]
                                                                                                                                }).ToList();

        public Task<IReadOnlyList<FileNamePartsWithClassifications>> GetKeywordsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_keywords);
    }
}
