using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SearchConfiguration.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchConfigurationCommandForDb" /> class used to update the actual database table.
///     <para>
///         The class implements the <see cref="IRequest{IResult}" /> as defined within MediatR.
///     </para>
/// </summary>
/// <param name="slug">The ScrapeDirectoryId used to uniquely identify the Site Configuration</param>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
/// <param name="update">
///     The <see cref="UpdateSearchConfigurationCommand" /> class containing the
///     command details for the Update.
/// </param>
public sealed class UpdateSearchConfigurationCommandForDb(
    string                           slug,
    ClaimsPrincipal                  user,
    UpdateSearchConfigurationCommand update) : IEndpointName
{
    /// <summary>
    ///     Gets the SiteConfigurationSlug for the Site Configuration.
    /// </summary>
    public string SiteConfigurationSlug { get; } = slug;

    /// <summary>
    ///     Gets the Base Url for the login and search.
    /// </summary>
    public string SearchString { get; } = update.SearchString;

    /// <summary>
    ///     Gets the Login Url.
    /// </summary>
    public string Subscriptions { get; } = update.Subscriptions;

    /// <summary>
    ///     Gets the SearchStringPrefix.
    /// </summary>
    public string SearchStringPrefix { get; } = update.SearchStringPrefix;

    /// <summary>
    ///     Gets the TopWallpapers.
    /// </summary>
    public string TopWallpapers { get; } = update.TopWallpapers;

    /// <summary>
    ///     Gets the TotalPages.
    /// </summary>
    public int TotalPages { get; } = update.TotalPages;

    /// <summary>
    ///     Gets the Image Pause in Seconds value
    /// </summary>
    public int ImagePauseInSeconds { get; } = update.ImagePauseInSeconds;

    /// <summary>
    /// </summary>
    public string SearchStringSuffix { get; } = update.SearchStringSuffix;

    /// <summary>
    ///     Gets the StartingPageNumber value
    /// </summary>
    public int StartingPageNumber { get; } = update.StartingPageNumber;

    /// <summary>
    ///     Gets the UseHeadless value
    /// </summary>
    public bool UseHeadless { get; } = update.UseHeadless;

    /// <summary>
    ///     Gets the SubscriptionsStartingPageNumber value
    /// </summary>
    public int SubscriptionsStartingPageNumber { get; } = update.SubscriptionsStartingPageNumber;

    /// <summary>
    ///     Gets the SubscriptionsTotalPages value
    /// </summary>
    public int SubscriptionsTotalPages { get; } = update.SubscriptionsTotalPages;

    /// <summary>
    ///     Gets the TopWallpapersTotalPages value
    /// </summary>
    public int TopWallpapersTotalPages { get; } = update.TopWallpapersTotalPages;

    /// <summary>
    ///     Gets the TopWallpapersStartingPageNumber value
    /// </summary>
    public int TopWallpapersStartingPageNumber { get; } = update.TopWallpapersStartingPageNumber;

    /// <summary>
    ///     Gets the SlowMotionDelayInMilliseconds value
    /// </summary>
    public int SlowMotionDelayInMilliseconds { get; } = update.SlowMotionDelayInMilliseconds;

    /// <summary>
    ///     Gets the UpdatedBy (the user performing the action) value
    /// </summary>
    public string UpdatedBy => user.Identity?.Name ?? "me";

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.SiteConfigurationsEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Put;
}
