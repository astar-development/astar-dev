using System.Text.Json;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Bogus;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.FilesDb.Fixtures;

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
        _ = Context.SaveChanges();
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
        var bogus = new Faker<FileDetail>()
            .UseSeed(1234)
            .RuleFor(fileDetail => fileDetail.Id, f=> new() {Value = f.Random.Guid()})
            .RuleFor(fileDetail=>fileDetail.FileName, f => new(f.System.FileName()))
            .RuleFor(fileDetail=>fileDetail.DirectoryName, f => new(f.System.DirectoryPath()));
        
        var testFiles = bogus.Generate(100);
        var filesAsJson = File.ReadAllText(@"TestFiles/files.json");

        var listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson)!;

        foreach(var item in listFromJson)
        {
            if(mockFilesContext.Files.FirstOrDefault(f => f.FileName == item.FileName && f.DirectoryName == item.DirectoryName) == null)
            {
                item.FileHandle = new($"{item.DirectoryName.Value}-{item.FileName.Value}-{item.Id}");
                mockFilesContext.Files.Add(item);
                mockFilesContext.SaveChanges();
            }
        }
    }
}
