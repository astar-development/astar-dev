using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Bogus;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Fixtures;

public class MockFilesContext : IDisposable
{
    private bool _disposedValue;

    public MockFilesContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var contextOptions = new DbContextOptionsBuilder<FilesContext>()
            .UseSqlite(connection)
            .Options;

        Context = new(contextOptions);

        _ = Context.Database.EnsureCreated();

        AddMockFiles(Context);
    }

    public FilesContext Context { get; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if(_disposedValue)
        {
            return;
        }

        if(disposing)
        {
            Context.Dispose();
        }

        _disposedValue = true;
    }

    private static void AddMockFiles(FilesContext mockFilesContext)
    {
        var fileExtension = new[] { "jpg", "jpeg", "bmp", "png", "txt", "txt", "doc", "xls", "pdf", "html" };
        var bogus = new Faker<FileDetail>()
            .UseSeed(1234)
            .RuleFor(fileDetail => fileDetail.Id, f=> new() {Value = f.Random.Guid()})
            .RuleFor(fileDetail => fileDetail.FileSize, f=>  f.Random.Int(100_000, 500_000))
            .RuleFor(fileDetail => fileDetail.CreatedDate, f=>  f.Person.DateOfBirth)
            .RuleFor(fileDetail => fileDetail.UpdatedDate, f=>  f.Person.DateOfBirth.AddDays(f.Random.Int(1, 1000)))
            .RuleFor(fileDetail => fileDetail.UpdatedOn, f=>  f.Person.DateOfBirth.AddDays(f.Random.Int(300, 500)))
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
        
        var testFiles = bogus.Generate(10_000);
        testFiles.ForEach(item => item.FileHandle = new($"{item.DirectoryName.Value}-{item.FileName.Value}-{item.Id}"));

        mockFilesContext.Files.AddRange(testFiles);
        mockFilesContext.SaveChanges();
    }
}
