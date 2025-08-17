using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using AStar.Dev.Database.Updater;
using AStar.Dev.Database.Updater.Core;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.ServiceDefaults;
using Microsoft.Extensions.Options;
using Serilog;
using ILogger = Serilog.ILogger;

var     startTime = Stopwatch.GetTimestamp();
ILogger logger    = null!;

try
{
    Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    logger     = Log.Logger;
    var builder = Host.CreateApplicationBuilder(args);

    _ = builder.Services.Configure<JsonSerializerOptions>(options =>
                                                          {
                                                              options.PropertyNamingPolicy        = JsonNamingPolicy.CamelCase;
                                                              options.DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull;
                                                              options.WriteIndented               = true;
                                                              options.Encoder                     = JavaScriptEncoder.Default;
                                                              options.AllowTrailingCommas         = true;
                                                              options.NumberHandling              = JsonNumberHandling.AllowReadingFromString;
                                                              options.PropertyNameCaseInsensitive = true;
                                                              options.ReferenceHandler            = ReferenceHandler.IgnoreCycles;
                                                          });

    _ = builder.AddServiceDefaults();
    _ = builder.Configuration.AddJsonFile(Configuration.ExternalSettingsFile, false, true);

    _ = builder.Services
               .AddOptions<DatabaseUpdaterConfiguration>()
               .Bind(builder.Configuration.GetSection(DatabaseUpdaterConfiguration.ConfigurationSectionName));

    _ = builder.Services.AddSerilog((sp, loggerConfig) => loggerConfig.ReadFrom.Configuration(sp.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>()));

    _ = builder.Services.AddSingleton<AddNewFilesService>();
    _ = builder.Services.AddSingleton<IValidateOptions<DatabaseUpdaterConfiguration>, DatabaseUpdaterConfigurationValidator>();
    _ = builder.Services.AddSingleton<TimeDelay>();
    _ = builder.Services.AddSingleton<IFileSystem, FileSystem>();

    _ = builder.Services.AddHostedService<NewFilesBackgroundService>();

    var host = builder.Build();

    await host.RunAsync();
}
catch(Exception ex)
{
    logger.Error(ex, "An error occured while running the application.");
}
finally
{
    var elapsedTime = Stopwatch.GetElapsedTime(startTime).TotalMilliseconds;
    logger.Information("Stopped after {ProcessingTimeMilliseconds} ms", elapsedTime);
    await Log.CloseAndFlushAsync();
}
