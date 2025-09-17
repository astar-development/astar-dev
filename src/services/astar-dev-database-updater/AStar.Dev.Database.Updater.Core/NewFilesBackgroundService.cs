using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     The <see cref="NewFilesBackgroundService" /> class
/// </summary>
/// <param name="addNewFilesService">The <see cref="AddNewFilesService" /> required by the AddNewFilesBackgroundService</param>
/// <param name="timeDelay">An instance of <see cref="TimeDelay" /> to control the start time of the process</param>
/// <param name="config">An instance of the <see cref="DatabaseUpdaterConfiguration" /> options used to configure the addition of the new files</param>
/// <param name="logger">An instance of the <see cref="ILogger" /> to log status / errors</param>
public class NewFilesBackgroundService(AddNewFilesService addNewFilesService, TimeDelay timeDelay, IOptions<DatabaseUpdaterConfiguration> config, ILogger<NewFilesBackgroundService> logger)
    : BackgroundService
{
    /// <summary>
    ///     The StartAsync method is called by the runtime and will update the database with any new files
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
        var startAtTime = config.Value.NewFilesScheduledTime;

        var delay = config.Value.HonourFirstDelay
                        ? timeDelay.CalculateDelayToNextRun(startAtTime)
                        : new Result<TimeSpan, ErrorResponse>.Ok(TimeSpan.Zero);

        while(!stoppingToken.IsCancellationRequested)
        {
            _ = await delay.Tap(LogMessage)
                           .BindAsync(async timeSpan => await AwaitDelay(timeSpan, stoppingToken))
                           .BindAsync(timeSpan => addNewFilesService.StartAsync(timeSpan, stoppingToken))
                           .BindAsync(result => Task.FromResult(timeDelay.CalculateDelayToNextRun(startAtTime)))
                           .BindAsync(delayUntilNextRun => AwaitDelay(delayUntilNextRun, stoppingToken))
                           .TapErrorAsync(async error => await LogErrorMessage(stoppingToken, error));
        }
    }

    private async Task LogErrorMessage(CancellationToken stoppingToken, ErrorResponse errorResponse)
    {
        logger.LogError("Error: {ErrorMessage}", errorResponse.Message);
        await Task.Delay(1, stoppingToken);
    }

    private static async Task<Result<TimeSpan, ErrorResponse>> AwaitDelay(TimeSpan timeSpan, CancellationToken stoppingToken)
    {
        await Task.Delay(timeSpan, stoppingToken);

        return new Result<TimeSpan, ErrorResponse>.Ok(timeSpan);
    }

    private void LogMessage(TimeSpan delayToNextRun)
        => logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database again.", delayToNextRun.TotalHours);
}
