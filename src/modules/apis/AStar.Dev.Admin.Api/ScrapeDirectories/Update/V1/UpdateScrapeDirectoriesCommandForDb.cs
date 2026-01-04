using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.Update.V1;

/// <summary>
///     The UpdateScrapeDirectoriesCommandForDb method defines the update class used to update the actual database table.
///     <para>
///         The class implements the <see cref="IRequest{IResult}" /> as defined within MediatR.
///     </para>
/// </summary>
/// <param name="scrapeDirectoryId">The ScrapeDirectoryId used to uniquely identify the Scrape Directories</param>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
/// <param name="updateScrapeDirectoriesCommand">
///     The <see cref="UpdateScrapeDirectoriesCommand" /> class containing the command details for the Update.
/// </param>
public sealed class UpdateScrapeDirectoriesCommandForDb(
    Guid                           scrapeDirectoryId,
    ClaimsPrincipal                user,
    UpdateScrapeDirectoriesCommand updateScrapeDirectoriesCommand) : IEndpointName
{
    /// <summary>
    ///     Gets the ScrapeDirectoryId.
    /// </summary>
    public Guid ScrapeDirectoryId { get; } = scrapeDirectoryId;

    /// <summary>
    ///     Gets the RootDirectory.
    /// </summary>
    public string RootDirectory { get; } = updateScrapeDirectoriesCommand.RootDirectory;

    /// <summary>
    ///     Gets the BaseSaveDirectory.
    /// </summary>
    public string BaseSaveDirectory { get; } = updateScrapeDirectoriesCommand.BaseSaveDirectory;

    /// <summary>
    ///     Gets the BaseDirectory.
    /// </summary>
    public string BaseDirectory { get; } = updateScrapeDirectoriesCommand.BaseDirectory;

    /// <summary>
    ///     Gets the BaseDirectoryFamous.
    /// </summary>
    public string BaseDirectoryFamous { get; } = updateScrapeDirectoriesCommand.BaseDirectoryFamous;

    /// <summary>
    ///     Gets the SubDirectoryName.
    /// </summary>
    public string SubDirectoryName { get; } = updateScrapeDirectoriesCommand.SubDirectoryName;

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
