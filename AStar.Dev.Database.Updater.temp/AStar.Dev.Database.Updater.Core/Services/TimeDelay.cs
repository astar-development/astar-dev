using System.Globalization;
using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.Database.Updater.Core.Services;

/// <summary>
///     The <see cref="TimeDelay" /> class
/// </summary>
public class TimeDelay
{
    /// <summary>
    ///     The CalculateDelayToNextRun will return the <see cref="TimeSpan" /> reresenting the delay until the next scheduled run time
    /// </summary>
    /// <param name="targetTime">The target time to delay until</param>
    /// <returns>The <see cref="TimeSpan" /> reresenting the delay until the next scheduled run time</returns>
    public Result<TimeSpan, ErrorResponse> CalculateDelayToNextRun(TimeOnly targetTime)
    {
        var duration = DateTime.Parse(targetTime.ToShortTimeString(), CultureInfo.CurrentCulture).Subtract(DateTime.Now);

        if(duration < TimeSpan.Zero)
        {
            duration = duration.Add(TimeSpan.FromHours(24));
        }

        return new Result<TimeSpan, ErrorResponse>.Ok(duration); // ToDo - this implementation doesn't need the ErrorResponse so doesn't "need" the result at all
    }
}
