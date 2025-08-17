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

        Result<TimeSpan, ErrorResponse> delay = config.Value.HonourFirstDelay
                                                    ? timeDelay.CalculateDelayToNextRun(startAtTime)
                                                    : new Result<TimeSpan, ErrorResponse>.Ok(TimeSpan.Zero);

        while(!stoppingToken.IsCancellationRequested)
        {
            var x = await delay.Tap(LogMessage)
                               .BindAsync(async timeSpan => await AwaitDelay(timeSpan, stoppingToken))
                               .BindAsync(async d => await addNewFilesService.StartAsync(d, stoppingToken));

            _ = x.Match<Result<TimeSpan, ErrorResponse>>(_ => timeDelay.CalculateDelayToNextRun(startAtTime),
                                                         _ => new Result<TimeSpan, ErrorResponse>.Error(new("Error.")))
                 .BindAsync(async timeSpan => await AwaitDelay(timeSpan, stoppingToken))
                 .TapErrorAsync(async f => await LogErrorMessage(stoppingToken, f));
        }
    }

    private async Task LogErrorMessage(CancellationToken stoppingToken, ErrorResponse f)
    {
        logger.LogError("Error: {ErrorMessage}", f.Message);
        await Task.Delay(1, stoppingToken);
    }

    private static async Task<Result<TimeSpan, ErrorResponse>> AwaitDelay(TimeSpan timeSpan, CancellationToken stoppingToken)
    {
        await Task.Delay(timeSpan, stoppingToken);

        return new Result<TimeSpan, ErrorResponse>.Ok(timeSpan);
    }

    private void LogMessage(TimeSpan delay)
        => logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database again.", delay.TotalHours);
}
