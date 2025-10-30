using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     The <see cref="TimeDelay" /> class
/// </summary>
public class TimeDelay(TimeProvider timeProvider, ILogger<TimeDelay> logger)
{
    /// <summary>
    ///     The CalculateDelayToNextRun will return the <see cref="TimeSpan" /> reresenting the delay until the next scheduled run time
    ///     This implementation doesn't need the ErrorResponse so doesn't "need" the result at all
    /// </summary>
    /// <param name="targetTime">The target time to delay until</param>
    /// <param name="valueHonourFirstDelay"></param>
    /// <returns>The <see cref="TimeSpan" /> reresenting the delay until the next scheduled run time</returns>
    public Result<TimeSpan, ErrorResponse> CalculateDelayToNextRun(TimeOnly targetTime, bool valueHonourFirstDelay)
    {
        TimeOnly currentTime = GetCurrentTime();

        TimeSpan timeDelay = valueHonourFirstDelay ? TimeToNextOccurrence(targetTime, currentTime) : TimeSpan.Zero;

        logger.LogInformation("Next scheduled run at: {NextScheduledRunTime} - time delay: {TimeDelay}", targetTime, timeDelay);

        return new Result<TimeSpan, ErrorResponse>.Ok(timeDelay);
    }

    private TimeOnly GetCurrentTime() => TimeOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

    private static TimeSpan TimeToNextOccurrence(TimeOnly targetTime, TimeOnly currentTime)
        => targetTime < currentTime
            ? targetTime.Add(TimeSpan.FromDays(1)) - currentTime
            : targetTime - currentTime;
}
