using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Fixtures;

public sealed class MockFilesContext : IAsyncDisposable, IDisposable
{
    private bool _disposedValue;
    private readonly MsSqlContainer? _sqlContainer;

    public MockFilesContext()
    {
        // Configure SQL Server container
        _sqlContainer = new MsSqlBuilder()
            .WithPassword("YourStrong@Passw0rd")
            .WithCleanUp(true)
            .Build();

        // Start container and initialize context
        InitializeAsync().GetAwaiter().GetResult();
    }

    public FilesContext Context { get; private set; } = null!;

    private async Task InitializeAsync()
    {
        // Start the SQL Server container
        if(_sqlContainer != null)
        {
            await _sqlContainer.StartAsync();

            // Create EF Core context with container connection string
            var options = new DbContextOptionsBuilder<FilesContext>()
                .UseSqlServer(_sqlContainer.GetConnectionString())
                .Options;

            Context = new(options);
        }

        // Create database schema and populate with test data
        await Context.Database.EnsureCreatedAsync();
        AddMockFiles(Context);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCoreAsync();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if(_disposedValue)
        {
            return;
        }

        if(disposing)
        {
            Context?.Dispose();
            _sqlContainer?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        _disposedValue = true;
    }

    private async ValueTask DisposeAsyncCoreAsync()
    {
        if(!_disposedValue)
        {
            Context?.Dispose();
            if(_sqlContainer != null)
            {
                await _sqlContainer.DisposeAsync();
            }
        }
    }

    private static void AddMockFiles(FilesContext context)
    {
        var testFiles = GenerateMockFiles();
        context.Files.AddRange(testFiles);
        context.SaveChanges();
    }

    private static List<FileDetail> GenerateMockFiles()
    {
        var fileExtension = new[] { "jpg", "jpeg", "bmp", "png", "txt", "txt", "doc", "xls", "pdf", "html" };
        var bogus = new Faker<FileDetail>()
            .UseSeed(1234)
            .RuleFor(fileDetail => fileDetail.Id, f=> new() {Value = f.Random.Guid()})
            .RuleFor(fileDetail => fileDetail.FileSize, f=>  f.Random.Int(100_000, 500_000))
            .RuleFor(fileDetail => fileDetail.CreatedDate, f => f.Date.Recent(10, new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc)))
            .RuleFor(fileDetail => fileDetail.UpdatedDate, f => f.Date.Recent(5, new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc)))
            .RuleFor(fileDetail => fileDetail.UpdatedOn, f => f.Date.Recent(2, new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc)))
            .RuleFor(fileDetail => fileDetail.FileAccessDetail,
                f => new()
                {
                    DetailsLastUpdated = f.Date.Recent(0, new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc)),
                    LastViewed =
                        f.Random.Bool()
                            ? f.Date.Recent(3, new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc))
                            : f.Date.Recent(30, new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc)), // Mix of recent (within 3 days) and old (within 30 days ago)
                    UpdatedBy = "Test",
                    UpdatedOn = f.Date.Recent(2, new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc)),
                    MoveRequired = f.Random.Bool()
                })
            .RuleFor(fileDetail => fileDetail.FileName, f => new(f.System.FileName(f.PickRandom(fileExtension))))
            .RuleFor(fileDetail=>fileDetail.DirectoryName, f => new(f.System.DirectoryPath())).RuleFor(f => f.DeletionStatus, (f, fd) =>
            {
                var status = new DeletionStatus();

                // ~5% chance that one date gets set
                if(!(f.Random.Double() < 0.05))
                {
                    return status;
                }

                var chosen = f.PickRandom("SoftDeletePending", "Hard", "SoftDeleted");
                var randomDate = f.Date.Past(1);
                switch (chosen)
                {
                    case "Soft":
                        status.SoftDeletePending = randomDate;
                        break;
                    case "SoftDeleted":
                        status.SoftDeleted = randomDate;
                        break;
                    default:
                        status.HardDeletePending = randomDate;
                        break;
                }

                return status;

            })
            ;

        var testFiles = bogus.Generate(15_000);
        testFiles.ForEach(item => item.FileHandle = new($"{item.DirectoryName.Value}-{item.FileName.Value}-{item.Id}"));

        return testFiles;
    }
}
