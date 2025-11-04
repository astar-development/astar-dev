using AStar.Dev.Aspire.Common;
using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using AStar.Dev.Files.Classifications.Api.Services;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AStar.Dev.Files.Classifications.Api;

/// <summary>
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddFileClassificationsApplicationServices(this WebApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.AStarDb, settings =>
        {
            settings.CommandTimeout = 120;
            settings.DisableRetry = false;
        });

        _ = builder.Services.AddScoped<IFileClassificationsService2, FileClassificationsService2>();
        _ = builder.Services.AddScoped<GetFileClassificationsHandler>();

        return builder;
    }
}
