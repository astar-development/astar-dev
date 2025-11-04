using System.IO.Abstractions;
using System.Text.Json;
using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core;
using AStar.Dev.Database.Updater.Core.ClassificationsServices;
using AStar.Dev.Database.Updater.Core.FileDetailsServices;
using AStar.Dev.FileServices.Common;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
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
    /// <param name="builder">
    ///     The <see cref="WebApplicationBuilder" /> used to configure services and application behavior.
    /// </param>
    /// <returns>
    ///     The modified <see cref="WebApplicationBuilder" /> instance with configured services and settings.
    /// </returns>
    public static HostApplicationBuilder ConfigureApplicationServices(this HostApplicationBuilder builder)
    {
        _ = builder.AddServiceDefaults();

        _ = builder.Services.Configure<JsonSerializerOptions>(CreateJsonConfigureOptions);

        _ = builder.Configuration.AddJsonFile(Configuration.ExternalSettingsFile, false, true);

        _ = builder.Services
                   .AddOptions<DatabaseUpdaterConfiguration>()
                   .Bind(builder.Configuration.GetSection(DatabaseUpdaterConfiguration.ConfigurationSectionName))
                   .ValidateDataAnnotations()
                   .ValidateOnStart();

        _ = builder.Services.AddSerilog((sp, loggerConfig) => loggerConfig.ReadFrom.Configuration(sp.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>()));

        _ = builder.Services.AddHostedService<FileClassificationsBackgroundService>();
        _ = builder.Services.AddHostedService<FileKeywordProcessorBackgroundService>();
        _ = builder.Services.AddScoped<FileClassificationsBackgroundService>();

        builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.AStarDb, settings =>
                                                                                 {
                                                                                     settings.CommandTimeout = 120;
                                                                                     settings.DisableRetry = false;
                                                                                 });

        _ = builder.Services.AddScoped<FileListService>();
        _ = builder.Services.AddScoped<IFileListService, FileListService>();
        _ = builder.Services.AddScoped<FileDetailsProcessorService>();
        _ = builder.Services.AddScoped<ClassificationProcessor>();
        _ = builder.Services.AddScoped<ClassificationRepository>();
        _ = builder.Services.AddSingleton<ClassificationBuilder>();

        _ = builder.Services.AddSingleton(TimeProvider.System);
        _ = builder.Services.AddSingleton<ClassificationsMapper>();
        _ = builder.Services.AddSingleton<IValidateOptions<DatabaseUpdaterConfiguration>, DatabaseUpdaterConfigurationValidator>();
        _ = builder.Services.AddSingleton<TimeDelay>();
        _ = builder.Services.AddSingleton<IFileSystem, FileSystem>();

        _ = builder.Services.AddScoped<FilesProcessor>();
        _ = builder.Services.AddScoped<IFilesProcessor, FilesProcessor>();
        _ = builder.Services.AddScoped<FileHandleService>();
        _ = builder.Services.AddScoped<IKeywordProvider, EfKeywordProvider>();

        _ = builder.Services.AddHealthChecks();

        _ = builder.Services.AddOpenTelemetry()
               .ConfigureResource(r => r.AddService("FileKeywordProcessor"))
               .WithMetrics(mb => mb.AddMeter("FileScanner", "DatabaseWriter"))
               .WithTracing(tb =>
                            {
                                _ = tb.AddSource("FileScanner", "DatabaseWriter");
                                _ = tb.AddSource(Constants.ActivitySourceName);
                            });

        _ = builder.Services.AddOpenTelemetry()
                   .WithTracing(tracing => tracing.AddSource(Constants.ActivitySourceName));

        return builder;
    }
}
