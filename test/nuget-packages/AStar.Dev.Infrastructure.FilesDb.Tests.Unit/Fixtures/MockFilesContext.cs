using System.Text.Json;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Infrastructure.FilesDb.Fixtures;

public class MockFilesContext : IDisposable
{
    private bool disposedValue;

    public MockFilesContext()
    {
        Context = new(new ());

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
        var filesAsJson = File.ReadAllText(@"TestFiles\files.json");

        var listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson)!;

        foreach (var item in listFromJson)
        {
            if (mockFilesContext.Files.FirstOrDefault(f => f.FileName == item.FileName && f.DirectoryName == item.DirectoryName) == null)
            {
                mockFilesContext.Files.Add(item);
                mockFilesContext.SaveChanges();
            }
        }
    }
}
