using System.IO.Abstractions;
using System.Text.Json;
using AStar.Dev.Database.Updater.Core;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.ServiceDefaults;
using Microsoft.Extensions.Options;
using Serilog;
using static AStar.Dev.Database.Updater.JsonSerializerOptionsExtensions;

namespace AStar.Dev.Database.Updater;

/// <summary>
///     Provides extension methods for configuring and customizing the <see cref="HostApplicationBuilder" />
///     with application services, logging, and background tasks specific to database update functionalities.
/// </summary>
public static class HostApplicationBuilderExtensions
{
    /// <summary>
    ///     Configures application services for the host application by adding required dependencies,
    ///     options configurations, logging, and background services.
    /// </summary>
    /// <param name="hostApplicationBuilder">
    ///     The <see cref="HostApplicationBuilder" /> used to configure services and application behavior.
    /// </param>
    /// <returns>
    ///     The modified <see cref="HostApplicationBuilder" /> instance with configured services and settings.
    /// </returns>
    public static HostApplicationBuilder ConfigureApplicationServices(this HostApplicationBuilder hostApplicationBuilder)
    {
        _ = hostApplicationBuilder.AddServiceDefaults();

        _ = hostApplicationBuilder.Services.Configure<JsonSerializerOptions>(CreateJsonConfigureOptions);
        _ = hostApplicationBuilder.Configuration.AddJsonFile(Configuration.ExternalSettingsFile, false, true);

        _ = hostApplicationBuilder.Services
                                  .AddOptions<DatabaseUpdaterConfiguration>()
                                  .Bind(hostApplicationBuilder.Configuration.GetSection(DatabaseUpdaterConfiguration.ConfigurationSectionName));

        _ = hostApplicationBuilder.Services.AddSerilog((sp, loggerConfig) => loggerConfig.ReadFrom.Configuration(sp.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>()));

        _ = hostApplicationBuilder.Services.AddSingleton<AddNewFilesService>();
        _ = hostApplicationBuilder.Services.AddSingleton<IValidateOptions<DatabaseUpdaterConfiguration>, DatabaseUpdaterConfigurationValidator>();
        _ = hostApplicationBuilder.Services.AddSingleton<TimeDelay>();
        _ = hostApplicationBuilder.Services.AddSingleton<IFileSystem, FileSystem>();

        _ = hostApplicationBuilder.Services.AddHostedService<NewFilesBackgroundService>();

        return hostApplicationBuilder;
    }
}
