using AStar.Dev.Infrastructure.FilesDb.Data;

namespace DbContextHelpers.Fixtures;

public class FilesContextFixture : IDisposable
{
    private bool disposedValue;

    public FilesContext Sut { get; } = MockFilesContextFactory.CreateMockFilesContext().Result;

    public FilesContext SutWithFileDetails
    {
        get
        {
            var c = MockFilesContextFactory.CreateMockFilesContext().Result;
            c.AddMockFiles();

            return c;
        }
    }

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
