using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.TestHelpers;

/// <summary>
/// Provides a disposable test scope that owns an in-memory Sqlite connection and a FilesContext.
/// Use the static CreateAsync helper to obtain an instance.
/// </summary>
public sealed class SqliteTestScope : IAsyncDisposable
{
    public SqliteConnection Connection { get; }
    public FilesContext Context { get; }

    private SqliteTestScope(SqliteConnection connection, FilesContext context)
    {
        Connection = connection;
        Context = context;
    }

    public static async Task<SqliteTestScope> CreateAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FilesContext>()
                      .UseSqlite(connection)
                      .Options;

        var ctx = new FilesContext(options);
        _ = await ctx.Database.EnsureCreatedAsync();

        return new SqliteTestScope(connection, ctx);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        Connection.Close();
        Connection.Dispose();
    }
}
