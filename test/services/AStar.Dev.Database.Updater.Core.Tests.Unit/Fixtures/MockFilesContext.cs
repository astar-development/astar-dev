using System.Text.Json;
using AStar.Dev.Infrastructure.Data;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Core.Fixtures;

public class MockFilesContext : IDisposable
{
    private readonly ConnectionString connectionString = new() { Value = "Filename=:memory:" };
    private readonly FilesContext     context;
    private          bool             disposedValue;

    public MockFilesContext()
    {
        context = new();

        _ = context.Database.EnsureCreated();

        AddMockFiles(context);
        _ = context.SaveChanges();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public FilesContext Context() => context;

    protected virtual void Dispose(bool disposing)
    {
        if(disposedValue)
        {
            return;
        }

        if(disposing)
        {
            context.Dispose();
        }

        disposedValue = true;
    }

    private static void AddMockFiles(FilesContext mockFilesContext)
    {
        var filesAsJson = File.ReadAllText(@"TestFiles\files.json");

        var listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson)!;

        mockFilesContext.AddRange(listFromJson);
    }
}
