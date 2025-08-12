using System.IO.Abstractions;
using AStar.Dev.Database.Updater.Core.Files;
using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Database.Updater.Core.Services;
using AStar.Dev.Logging.Extensions;
using Serilog;
using ILogger = Serilog.ILogger;

var     startTime = DateTime.Now;
ILogger logger    = null!;

try
{
    var builder = WebApplication.CreateBuilder(args);
    _ = builder.Configuration.AddJsonFile(Configuration.ExternalSettingsFile, false, true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    _ = builder.Services
               .AddOptions<DatabaseUpdaterConfiguration>()
               .Bind(builder.Configuration.GetSection(DatabaseUpdaterConfiguration.ConfigurationSectionName))
               .ValidateDataAnnotations()
               .ValidateOnStart();

    _ = builder.Services.AddSerilog((sp, loggerConfig) => loggerConfig.ReadFrom.Configuration(sp.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>()));

    // _ = builder.Services.AddSingleton<FileClassificationsService>();
    _ = builder.Services.AddSingleton<AddNewFilesService>();

    // _ = builder.Services.AddSingleton<SoftDeleteFilesService>();
    // _ = builder.Services.AddSingleton<HardDeleteFilesService>();
    // _ = builder.Services.AddSingleton<DeleteFileService>();
    _ = builder.Services.AddSingleton<ClassificationsMapper>();
    _ = builder.Services.AddSingleton<TimeDelay>();
    _ = builder.Services.AddSingleton<IFileSystem, FileSystem>();

    _ = builder.Services.AddHostedService<NewFilesBackgroundService>();

    // _ = builder.Services.AddHostedService<SoftDeleteFilesBackgroundService>();
    // _ = builder.Services.AddHostedService<HardDeleteFilesBackgroundService>();

    var app = builder.Build();

    logger     = app.Services.GetRequiredService<ILogger>();
    Log.Logger = logger;
    logger.Information("Serilog has been configured.");
    logger.Information("Starting at {startTime}", startTime);

    // var classifications         = app.Services.GetRequiredService<FileClassificationsService>();

    var cancellationTokenSource = new CancellationTokenSource();

    // await classifications.AddNewMappingsToTheDatabase(cancellationTokenSource.Token);

    Console.WriteLine($"Ended after: {DateTime.Now - startTime}");
    logger.Information("Ended at {EndTime} after {ProcessingTimeMilliseconds}", startTime, DateTime.Now - startTime);
    app.MapOpenApi();

    app.UseHttpsRedirection();

    app.Run();
}
catch(Exception exception)
{
    logger.Error(exception, "An error occured while running the application.");
}
finally
{
    logger.Information("Stopped after {ProcessingTimeMilliseconds}", DateTime.Now - startTime);
    Log.CloseAndFlush();
}
