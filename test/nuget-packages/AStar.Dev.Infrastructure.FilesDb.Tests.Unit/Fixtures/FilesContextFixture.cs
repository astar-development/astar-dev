using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Fixtures;

public class FilesContextFixture : IAsyncDisposable, IDisposable
{
    private bool _disposedValue;
    private readonly MockFilesContext _mockContext;

    public FilesContextFixture()
    {
        _mockContext = new MockFilesContext();
        Sut = _mockContext.Context;
    }

    public FilesContext Sut { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCoreAsync();
        Dispose(false);
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
            _mockContext?.Dispose();
        }

        _disposedValue = true;
    }

    protected virtual async ValueTask DisposeAsyncCoreAsync()
    {
        if(!_disposedValue && _mockContext != null)
        {
            await _mockContext.DisposeAsync();
        }
    }
}
