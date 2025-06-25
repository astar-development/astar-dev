using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
///     The <see href="ExcludedViewSettings"></see> class.
/// </summary>
public sealed class ExcludedViewSettings
{
    /// <summary>
    ///     Gets or sets the excluded viewed items period (in days) for the search. The default is 7
    /// </summary>
    public int ExcludeViewedPeriodInDays { get; set; } = 7;

    /// <summary>
    ///     Gets or sets the Exclude Viewed flag. The time period is configurable via the
    ///     <see href="ExcludeViewedPeriodInDays"></see> property
    /// </summary>
    public bool ExcludeViewed { get; set; } = true;

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>This object serialized as a JSON object</returns>
    public override string ToString() =>
        this.ToJson();
}
