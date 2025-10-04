using System.IO.Abstractions;
using System.Threading.Channels;
using AStar.Dev.Database.Updater;
using AStar.Dev.Database.Updater.FileKeywordProcessor;
using AStar.Dev.Infrastructure.FilesDb.Data;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;
using ILogger = Serilog.ILogger;

var startTime = DateTime.Now;

ILogger? logger = null;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureApplicationServices();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    if(app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    Log.Logger = app.Services.GetRequiredService<ILogger>();
    app.MapHealthChecks("/health"); // Aspire will pick this up

    app.MapGet("/process-files", async (FileScanner  fileScanner, ChannelWriter<FileKeywordMatch> writer, IOptions<DatabaseUpdaterConfiguration> config, IFileSystem fileSystem,
                                        FilesContext filesContext) => {
                                     var                 enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
                                     IEnumerable<string> keywords           = filesContext.FileNameParts.Select(fp => fp.Text);
                                     var                 filePaths          = fileSystem.Directory.EnumerateFiles(config.Value.RootDirectory, "*", enumerationOptions);
                                     await fileScanner.ScanFilesAsync(keywords, filePaths, writer);
                                 });

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

namespace AStar.Dev.Database.Updater
{
    [UsedImplicitly]
    public class Program
    {
    }
}
