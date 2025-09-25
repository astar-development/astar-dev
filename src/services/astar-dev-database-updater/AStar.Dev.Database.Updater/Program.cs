using System.IO.Abstractions;
using AStar.Dev.Aspire.Common;
using AStar.Dev.Database.Updater;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Database.Updater.Core.Files;
using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Database.Updater.Core.Shared;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.ServiceDefaults;
using Serilog;
using ILogger = Serilog.ILogger;

var startTime = DateTime.Now;

ILogger? logger = null;

try
{
    var builder = Host.CreateApplicationBuilder(args);
    builder.AddServiceDefaults();
    _ = builder.Configuration.AddJsonFile(Configuration.ExternalSettingsFile, false, true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    // builder.Services.AddOpenApi();

    _ = builder.Services
               .AddOptions<DatabaseUpdaterConfiguration>()
               .Bind(builder.Configuration.GetSection(DatabaseUpdaterConfiguration.ConfigurationSectionName))
               .ValidateDataAnnotations()
               .ValidateOnStart();

    _ = builder.Services.AddSerilog((sp, loggerConfig) => loggerConfig.ReadFrom.Configuration(sp.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>()));

    _ = builder.Services.AddScoped<FileClassificationsService>();
    builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);

    // alternatively:
    // builder.Services.AddDbContext<FilesContext>(options =>
    //                                                     options.UseSqlServer(builder.Configuration.GetConnectionString(AspireConstants.Sql.SqlServer)
    //                                                                          ?? throw new InvalidOperationException($"Connection string '{AspireConstants.Sql.SqlServer}' not found.")));
    //_ = builder.Services.AddDbContext<FilesContext>(options => options.UseSqlServer(AspireConstants.Sql.SqlServer));

    //_ = builder.Services.AddSingleton<FilesContext>();
    _ = builder.Services.AddSingleton<AddNewFilesService>();
    _ = builder.Services.AddSingleton<ClassificationProcessor>();
    _ = builder.Services.AddSingleton<ClassificationRepository>();
    _ = builder.Services.AddSingleton<ClassificationBuilder>();

    // _ = builder.Services.AddSingleton<SoftDeleteFilesService>();
    // _ = builder.Services.AddSingleton<HardDeleteFilesService>();
    // _ = builder.Services.AddSingleton<DeleteFileService>();
    // _ = builder.Services.AddHttpClient<FilesApiClient>();
    _ = builder.Services.AddScoped<ClassificationsMapper>();
    _ = builder.Services.AddSingleton<TimeDelay>();
    _ = builder.Services.AddSingleton<IFileSystem, FileSystem>();

    _ = builder.Services.AddHostedService<AddNewFilesBackgroundService>();

    // _ = builder.Services.AddHostedService<SoftDeleteFilesBackgroundService>();
    // _ = builder.Services.AddHostedService<HardDeleteFilesBackgroundService>();

    builder.Services.AddOpenTelemetry()
           .WithTracing(tracing => tracing.AddSource(Constants.ActivitySourceName));

    var app = builder.Build();

    logger     = app.Services.GetRequiredService<ILogger>();
    Log.Logger = logger;

    app.Run();
}
catch(Exception exception)
{
    logger?.Fatal(exception, "An error occured while running the application. Message: {ErrorMessage}", exception.Message);
}
finally
{
    logger?.Information("Stopped after {ProcessingTimeMilliseconds}", DateTime.Now - startTime);
    Log.CloseAndFlush();
}

public partial class Program
{
}
