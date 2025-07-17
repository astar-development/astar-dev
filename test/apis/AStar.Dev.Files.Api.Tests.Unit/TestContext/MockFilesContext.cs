using System.Text.Json;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.TestContext;

public class MockFilesContext : IDisposable
{
    private bool disposedValue;

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
        if (disposedValue)
        {
            return;
        }

        if (disposing)
        {
            Context.Dispose();
        }

        disposedValue = true;
    }

    private static void AddMockFiles(FilesContext mockFilesContext)
    {
        var filesAsJson = File.ReadAllText(@"TestData/files.json");

        var listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson)!;

        foreach (var item in listFromJson)
        {
            if (mockFilesContext.FileDetails.FirstOrDefault(f => f.FileName == item.FileName && f.DirectoryName == item.DirectoryName) == null)
            {
                item.FileHandle = $"{item.DirectoryName}-{item.FileName}-{item.Id}";
                mockFilesContext.FileDetails.Add(item);
                mockFilesContext.SaveChanges();
            }
        }
    }
}
