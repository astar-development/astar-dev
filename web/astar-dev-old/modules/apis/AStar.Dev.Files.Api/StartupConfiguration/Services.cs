using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Files.Api.StartupConfiguration;

/// <summary>
/// </summary>
public static class Services
{
    /// <summary>
    ///     The ConfigureDatabase method which does exactly that.
    /// </summary>
    /// <param name="services">
    ///     An instance of the <see cref="IServiceCollection" /> interface that will be used to configure the database context.
    /// </param>
    /// <returns>
    ///     The original instance of the <see href="WebApplicationBuilder"></see> to facilitate method chaining (AKA fluent
    ///     configuration).
    /// </returns>
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
    {
        ServiceProvider sp      = services.BuildServiceProvider();
        FilesContext    context = sp.GetRequiredService<FilesContext>();
        _ = context.Database.EnsureCreated();

        return services;
    }
}
