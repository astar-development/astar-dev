using System.IO.Abstractions;
using AStar.Dev.Database.Updater.Core.Files;
using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Database.Updater.Core.Services;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.ServiceDefaults;
using Serilog;
using ILogger = Serilog.ILogger;

var     startTime = DateTime.Now;
ILogger logger    = null!;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddServiceDefaults();
    _ = builder.Configuration.AddJsonFile(Configuration.ExternalSettingsFile, false, true);

    _ = builder.Services
               .AddOptions<DatabaseUpdaterConfiguration>()
               .Bind(builder.Configuration.GetSection(DatabaseUpdaterConfiguration.ConfigurationSectionName))
               .ValidateDataAnnotations()
               .ValidateOnStart();

    _ = builder.Services.AddSerilog((sp, loggerConfig) => loggerConfig.ReadFrom.Configuration(sp.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>()));

    _ = builder.Services.AddSingleton<AddNewFilesService>();
    _ = builder.Services.AddSingleton<TimeDelay>();
    _ = builder.Services.AddSingleton<IFileSystem, FileSystem>();

    builder.Services.AddHostedService<NewFilesBackgroundService>();

    var host = builder.Build();

    await host.RunAsync();
}
catch(Exception ex)
{
    logger.Error(ex, "An error occured while running the application.");
}
finally
{
    logger.Information("Stopped after {ProcessingTimeMilliseconds}", DateTime.Now - startTime);
    await Log.CloseAndFlushAsync();
}
