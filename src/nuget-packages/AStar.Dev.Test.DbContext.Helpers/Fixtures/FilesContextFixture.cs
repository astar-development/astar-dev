using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Test.DbContext.Helpers.Fixtures;

/// <summary>
/// </summary>
public class FilesContextFixture : IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// </summary>
    public FilesContextFixture()
    {
        Sut = MockFilesContextFactory.CreateMockFilesContextAsync().Result;
        Sut.AddMockFiles();
    }

    /// <summary>
    /// </summary>
    public FilesContext Sut
    {
        get;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if(_disposedValue) return;

        if(disposing) Sut.Dispose();

        _disposedValue = true;
    }
}
