using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Database.Updater.Tests.Unit.TestHelpers;

public static class SqliteTestHelper
{
    /// <summary>
    ///     Create and open a new in-memory SqliteConnection. Caller is responsible for disposing it.
    /// </summary>
    public static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        return connection;
    }

    /// <summary>
    ///     Create a new FilesContext using the provided open SqliteConnection and ensure database is created.
    ///     Caller is responsible for disposing the returned context.
    /// </summary>
    public static async Task<FilesContext> CreateContextAsync(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<FilesContext>()
            .UseSqlite(connection)
            .Options;

        var ctx = new FilesContext(options);
        await ctx.Database.EnsureCreatedAsync();

        return ctx;
    }
}
