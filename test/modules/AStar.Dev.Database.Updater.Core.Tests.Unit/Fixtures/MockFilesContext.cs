using System.Text.Json;
using AStar.Dev.Infrastructure.Data;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Core.Tests.Unit.Fixtures;

public class MockFilesContext : IDisposable
{
    private readonly ConnectionString _connectionString = new() { Value = "Filename=:memory:" };
    private readonly FilesContext     _context;
    private          bool             _disposedValue;

    public MockFilesContext()
    {
        _context = new FilesContext();

        _ = _context.Database.EnsureCreated();

        AddMockFiles(_context);
        _ = _context.SaveChanges();
    }

    public FilesContext Context() => _context;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in the 'Dispose(bool disposing)' method below
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
            _context.Dispose();
        }

        _disposedValue = true;
    }

    private static void AddMockFiles(FilesContext mockFilesContext)
    {
        var filesAsJson = File.ReadAllText(@"TestFiles\files.json");

        var listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson)!;

        mockFilesContext.AddRange(listFromJson);
    }
}
