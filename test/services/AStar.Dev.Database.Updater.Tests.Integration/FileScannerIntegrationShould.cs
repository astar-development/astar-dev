using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Database.Updater.Core.FileKeywordProcessor;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater;

public class FileScannerIntegrationShould : IDisposable
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
    private FilesContext? _context;

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
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        // Register ClassificationRepository in the test host DI so integration tests resolve the same way as production
        // Resolve the FilesContext from the same scope when ClassificationRepository is requested.
        appHost.Services.AddScoped<ClassificationRepository>(sp => new ClassificationRepository(sp.GetRequiredService<FilesContext>()));

        var db = appHost.Resources.OfType<SqlServerDatabaseResource>().Single(r => r.Name == AspireConstants.Sql.FilesDb);

        // Register FilesContext as a factory that resolves the connection string from the Aspire resource
        // at resolve-time. This defers the call to ConnectionStringExpression until the DI container actually
        // needs a FilesContext instance (which is after the distributed application has started and endpoints
        // have been allocated), avoiding timing issues.
        appHost.Services.AddScoped(_ =>
        {
            var conn = db.ConnectionStringExpression.GetValueAsync(stoppingToken).GetAwaiter().GetResult();
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
        classification.FileNameParts.Add(new FileNamePart { Text = "integration_keyword" });
        _context.FileClassifications.Add(classification);
        await _context.SaveChangesAsync(stoppingToken);

    // Build a simple keyword provider that returns a single keyword part with its classification
    var keywordProvider = new TestKeywordProvider(new[] { "integration_keyword" }, "IntegrationTest");

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

    var writer = System.Threading.Channels.Channel.CreateUnbounded<FileDetail>();

    var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

        // Resolve scoped services from a scope
    using var testScope = app.Services.CreateScope();
    // Resolve the ClassificationRepository from DI so the test mirrors production wiring
    var classificationRepository = testScope.ServiceProvider.GetRequiredService<ClassificationRepository>();
    var logger = testScope.ServiceProvider.GetRequiredService<ILogger<FileScanner>>();
    var tracker = new ThroughputTracker(System.TimeProvider.System);

    var channelWriter = writer.Writer;

    var scanner = new FileScanner(scopeFactory, classificationRepository, keywordProvider, channelWriter, tracker, logger);

        // Act
        await scanner.ScanFilesAsync(new List<FileDetail> { fileDetail }, stoppingToken);

        // Re-query the file details from the DB and ensure the classification link exists
    var files = await _context.Files.Include(f => f.FileClassifications)
                    .ToListAsync(stoppingToken);

    // FileScanner generates/truncates FileHandle values when scanning, so don't rely on the original
    // test-provided handle. Find the saved file by FileName instead.
    var savedFile = files.FirstOrDefault(f => f.FileName.Value == fileDetail.FileName.Value);

    savedFile.ShouldNotBeNull();
        savedFile!.FileClassifications.ShouldNotBeNull();
        savedFile.FileClassifications.Any(fc => fc.Name == "IntegrationTest").ShouldBeTrue();
    }

    // Minimal test keyword provider
    private class TestKeywordProvider : IKeywordProvider
    {
        private readonly IReadOnlyList<FileNamePartsWithClassifications> _keywords;

        public TestKeywordProvider(IEnumerable<string> keywords, string classificationName)
        {
            _keywords = keywords.Select(k => new FileNamePartsWithClassifications
                                             {
                                                 Text = k,
                                                 Name = classificationName,
                                                 Celebrity = false,
                                                 IncludeInSearch = true,
                                                 FileNameParts = new List<AStar.Dev.Infrastructure.FilesDb.Models.FileNamePart> { new() { Text = k } }
                                             }).ToList();
        }

        public Task<IReadOnlyList<FileNamePartsWithClassifications>> GetKeywordsAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_keywords);
    }
}
