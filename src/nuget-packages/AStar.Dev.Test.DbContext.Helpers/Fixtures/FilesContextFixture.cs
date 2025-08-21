using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Test.DbContext.Helpers.Fixtures;

/// <summary>
/// </summary>
public class FilesContextFixture : IDisposable
{
    private bool _disposedValue;

    public FilesContextFixture()
    {
        Sut = MockFilesContextFactory.CreateMockFilesContextAsync().Result;
        Sut.AddMockFiles();
    }

    public FilesContext Sut
    {
        get;
    }

    public void Dispose()
    {
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
            Sut.Dispose();
        }

        _disposedValue = true;
    }
}
