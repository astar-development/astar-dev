using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core.Files;

/// <summary>
///     The <see cref="AddNewFilesBackgroundService" /> class
/// </summary>
/// <param name="serviceProvider">The <see cref="IServiceProvider" /> required by the AddNewFilesBackgroundService</param>
/// <param name="timeDelay"></param>
/// <param name="config"></param>
/// <param name="logger">An instance of the <see cref="ILogger" /> to log status / errors</param>
public class AddNewFilesBackgroundService(IServiceProvider serviceProvider, TimeDelay timeDelay, IOptions<DatabaseUpdaterConfiguration> config, ILogger<AddNewFilesBackgroundService> logger)
    : BackgroundService
{
    /// <summary>
    ///     The StartAsync method is called by the runtime and will update the database with any new files
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();

        while(!stoppingToken.IsCancellationRequested)
        {
            var fileClassificationsService = scope.ServiceProvider.GetRequiredService<FileClassificationsService>();
            var addNewFilesService         = scope.ServiceProvider.GetRequiredService<AddNewFilesService>();

            await timeDelay.CalculateDelayToNextRun(config.Value.NewFilesScheduledTime, config.Value.HonourFirstDelay)
                           .Tap(d => logger.LogInformation("Delay {DelayToNextRun} until next run at: {StartTime}", d.ToString(@"hh\:mm\:ss"), config.Value.NewFilesScheduledTime))
                           .BindAsync(async timeUntilNextRun => await WaitUntilNextRun(timeUntilNextRun, stoppingToken))
                           .TapAsync(_ => logger.LogInformation("Starting addition of new file classifications"))
                           .BindAsync(async _ => await fileClassificationsService.AddNewMappingsToTheDatabase(stoppingToken))
                           .BindAsync(async _ => await addNewFilesService.StartAsync(stoppingToken))
                           .TapAsync(async _ => await Task.Run(() => config.Value.HonourFirstDelay = false, stoppingToken));
        }
    }

    private static async Task<Result<TimeSpan, ErrorResponse>> WaitUntilNextRun(TimeSpan timeDelay, CancellationToken stoppingToken)
    {
        await Task.Delay(timeDelay, stoppingToken);

        return new Result<TimeSpan, ErrorResponse>.Ok(timeDelay);
    }
}
