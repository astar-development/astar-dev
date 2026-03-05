using System.IO.Abstractions;
using AStar.Dev.Images.Api.Services;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Images.Api.StartupConfiguration;

/// <summary>
/// </summary>
[Refactor(1, 1, "Obsolete?")]
public static class Services
{
    /// <summary>
    /// </summary>
    public static IServiceCollection Configure(IServiceCollection services)
    {
        _ = services.AddSingleton<IFileSystem, FileSystem>();
        _ = services.AddSingleton<IImageService, ImageService>();

        ServiceProvider sp      = services.BuildServiceProvider();
        FilesContext    context = sp.GetRequiredService<FilesContext>();
        _ = context.Database.EnsureCreated();

        return services;
    }

    /// <summary>
    ///     The ConfigureDatabase method which does exactly that.
    /// </summary>
    /// <param name="services">
    ///     An instance of the <see cref="IServiceCollection" /> interface that will be used to configure the database context.
    /// </param>
    /// <param name="configuration">An instance of <see cref="IConfiguration" /> used to configure the database</param>
    /// <returns>
    ///     The original instance of the <see href="WebApplicationBuilder"></see> to facilitate method chaining (AKA fluent
    ///     configuration).
    /// </returns>
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        ServiceProvider sp      = services.BuildServiceProvider();
        FilesContext    context = sp.GetRequiredService<FilesContext>();
        _ = context.Database.EnsureCreated();

        return services;
    }
}
