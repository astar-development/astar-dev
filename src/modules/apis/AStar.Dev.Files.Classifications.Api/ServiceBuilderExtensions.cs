using AStar.Dev.Aspire.Common;
using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using AStar.Dev.Files.Classifications.Api.Services;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AStar.Dev.Files.Classifications.Api;

/// <summary>
/// 
/// </summary>
public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Adds the file classifications API services to the specified service collection.
    /// </summary>
    /// <remarks>This method registers the <see cref="Services.IFileClassificationsService2"/> with a scoped
    /// lifetime. Call this method during application startup to enable file classification features via dependency
    /// injection.</remarks>
    /// <param name="services">The service collection to which the file classifications API services will be added.</param>
    /// <param name="builder">The web application builder used to configure the application.</param>
    /// <returns>The same instance of <see cref="IServiceCollection"/> that was provided, to support method chaining.</returns>
    public static IServiceCollection AddFileClassificationsApiServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.AStarDb, settings =>
        {
            settings.CommandTimeout = 120;
            settings.DisableRetry = false;
        });
        _ = services.AddScoped<IFileClassificationsService2, FileClassificationsService2>();
        _ = services.AddScoped<GetFileClassificationsHandler>();
        return services;
    }
}
