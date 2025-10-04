using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Channels;
using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater.Core;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Database.Updater.FileKeywordProcessor;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.ServiceDefaults;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using Serilog;
using static AStar.Dev.Database.Updater.JsonSerializerOptionsExtensions;
using DatabaseUpdaterConfiguration = AStar.Dev.Database.Updater.FileKeywordProcessor.DatabaseUpdaterConfiguration;

namespace AStar.Dev.Database.Updater;

/// <summary>
///     Provides extension methods for configuring and customizing the <see cref="WebApplicationBuilder" />
///     with application services, logging, and background tasks specific to database update functionalities.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    ///     Configures application services for the host application by adding required dependencies,
    ///     options configurations, logging, and background services.
    /// </summary>
    /// <param name="webApplicationBuilder">
    ///     The <see cref="WebApplicationBuilder" /> used to configure services and application behavior.
    /// </param>
    /// <returns>
    ///     The modified <see cref="WebApplicationBuilder" /> instance with configured services and settings.
    /// </returns>
    public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder webApplicationBuilder)
    {
        _ = webApplicationBuilder.AddServiceDefaults();

        _ = webApplicationBuilder.Services.Configure<JsonSerializerOptions>(CreateJsonConfigureOptions);

        _ = webApplicationBuilder.Configuration.AddJsonFile(Configuration.ExternalSettingsFile, false, true);

        _ = webApplicationBuilder.Services
                                 .AddOptions<DatabaseUpdaterConfiguration>()
                                 .Bind(webApplicationBuilder.Configuration.GetSection(DatabaseUpdaterConfiguration.ConfigurationSectionName))
                                 .ValidateDataAnnotations()
                                 .ValidateOnStart();

        _ = webApplicationBuilder.Services.AddSerilog((sp, loggerConfig) => loggerConfig.ReadFrom.Configuration(sp.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>()));

        _ = webApplicationBuilder.Services.AddScoped<FileClassificationsService>();
        webApplicationBuilder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);
        _ = webApplicationBuilder.Services.AddScoped<ClassificationProcessor>();
        _ = webApplicationBuilder.Services.AddScoped<ClassificationRepository>();
        _ = webApplicationBuilder.Services.AddSingleton<ClassificationBuilder>();

        _ = webApplicationBuilder.Services.AddScoped<AddNewFilesService>();
        _ = webApplicationBuilder.Services.AddSingleton<ClassificationsMapper>();
        _ = webApplicationBuilder.Services.AddSingleton<IValidateOptions<DatabaseUpdaterConfiguration>, DatabaseUpdaterConfigurationValidator>();
        _ = webApplicationBuilder.Services.AddSingleton<TimeDelay>();
        _ = webApplicationBuilder.Services.AddSingleton<IFileSystem, FileSystem>();

        // _ = webApplicationBuilder.Services.AddHostedService<AddNewFilesBackgroundService>();
        var tracker = new ThroughputTracker();
        webApplicationBuilder.Services.AddSingleton(tracker);
        _ = webApplicationBuilder.Services.AddScoped<FileScanner>();

// Register health check
        webApplicationBuilder.Services.AddHealthChecks()
                             .AddCheck<ThroughputHealthCheck>("throughput");

// Existing channel backlog check can stay too

        var channel = Channel.CreateUnbounded<FileKeywordMatch>(new() { SingleReader = true, SingleWriter = false });

// Register channel as singleton so both scanner and writer use the same instance
        webApplicationBuilder.Services.AddSingleton(channel);

        webApplicationBuilder.Services.AddHealthChecks()
                             .AddCheck<ChannelBacklogHealthCheck>("channel_backlog");

        webApplicationBuilder.Services.AddOpenTelemetry()
                             .ConfigureResource(r => r.AddService("FileKeywordProcessor"))
                             .WithMetrics(mb => mb.AddMeter("FileScanner", "DatabaseWriter"))
                             .WithTracing(tb => {
                                              tb.AddSource("FileScanner", "DatabaseWriter");
                                              tb.AddSource(Constants.ActivitySourceName);
                                          });

        _ = webApplicationBuilder.Services.AddOpenTelemetry()
                                 .WithTracing(tracing => tracing.AddSource(Constants.ActivitySourceName));

        return webApplicationBuilder;
    }
}
