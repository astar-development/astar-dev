using System.Text.Json;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.FilesDb.Fixtures;

public class MockFilesContext : IDisposable
{
    private bool disposedValue;

    public MockFilesContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        DbContextOptions<FilesContext> contextOptions = new DbContextOptionsBuilder<FilesContext>()
                                                       .UseSqlite(connection)
                                                       .Options;

        Context = new FilesContext(contextOptions);

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
        if(disposedValue) return;

        if(disposing) Context.Dispose();

        disposedValue = true;
    }

    private static void AddMockFiles(FilesContext mockFilesContext)
    {
        var filesAsJson = File.ReadAllText(@"TestFiles/files.json");

        IEnumerable<FileDetail>? listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson)!;

        foreach(FileDetail item in listFromJson)
        {
            if(mockFilesContext.Files.FirstOrDefault(f => f.FileName == item.FileName && f.DirectoryName == item.DirectoryName) == null)
            {
                item.FileHandle = new FileHandle($"{item.DirectoryName.Value}-{item.FileName.Value}-{item.Id}");
                mockFilesContext.Files.Add(item);
                mockFilesContext.SaveChanges();
            }
        }
    }
}
