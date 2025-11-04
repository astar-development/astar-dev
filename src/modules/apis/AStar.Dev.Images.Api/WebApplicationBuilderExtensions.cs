using AStar.Dev.Aspire.Common;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace AStar.Dev.Images.Api;

/// <summary>
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddImageApiApplicationServices(this WebApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.AStarDb, settings =>
        {
            settings.CommandTimeout = 120;
            settings.DisableRetry = false;
        });

        return builder;
    }
}
