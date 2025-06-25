using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Infrastructure.FilesDb.Fixtures;

public class FilesContextFixture : IDisposable
{
    private bool disposedValue;

    public FilesContext Sut { get; } = new MockFilesContext().Context;

    public void Dispose()
    {
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
            Sut.Dispose();
        }

        disposedValue = true;
    }
}
