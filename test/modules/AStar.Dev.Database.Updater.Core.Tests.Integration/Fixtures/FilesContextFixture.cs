using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Database.Updater.Core.Tests.Integration.Fixtures;

public class FilesContextFixture : IDisposable
{
    private bool _disposedValue;

    public FilesContext Sut { get; } = new MockFilesContext().Context();

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
