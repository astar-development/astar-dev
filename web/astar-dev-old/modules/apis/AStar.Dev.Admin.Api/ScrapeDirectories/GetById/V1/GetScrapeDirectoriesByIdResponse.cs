using AStar.Dev.Infrastructure.AdminDb.Models;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.GetById.V1;

/// <summary>
///     The <see cref="GetScrapeDirectoriesByIdResponse" /> class used to return the Site Configuration to the caller.
/// </summary>
/// <param name="scrapeDirectory">The instance of <see cref="ScrapeDirectory" /> to map to the response</param>
public sealed class GetScrapeDirectoriesByIdResponse(ScrapeDirectory scrapeDirectory)
{
    /// <summary>
    ///     Gets the ID for the Configuration.
    /// </summary>
    public int Id { get; } = scrapeDirectory.Id;

    /// <summary>
    ///     Gets the ScrapeDirectoryId.
    /// </summary>
    public Guid ScrapeDirectoryId { get; } = scrapeDirectory.ScrapeDirectoryId;

    /// <summary>
    ///     Gets the RootDirectory.
    /// </summary>
    public string RootDirectory { get; } = scrapeDirectory.RootDirectory;

    /// <summary>
    ///     Gets the BaseSaveDirectory.
    /// </summary>
    public string BaseSaveDirectory { get; } = scrapeDirectory.BaseSaveDirectory;

    /// <summary>
    ///     Gets the BaseDirectory.
    /// </summary>
    public string BaseDirectory { get; } = scrapeDirectory.BaseDirectory;

    /// <summary>
    ///     Gets the BaseDirectoryFamous.
    /// </summary>
    public string BaseDirectoryFamous { get; } = scrapeDirectory.BaseDirectoryFamous;

    /// <summary>
    ///     Gets the SubDirectoryName.
    /// </summary>
    public string SubDirectoryName { get; } = scrapeDirectory.SubDirectoryName;

    /// <summary>
    ///     Gets the UpdatedBy.
    /// </summary>
    public string UpdatedBy { get; } = scrapeDirectory.UpdatedBy;

    /// <summary>
    ///     Gets the UpdatedOn.
    /// </summary>
    public DateTime UpdatedOn { get; } = scrapeDirectory.UpdatedOn;
}
