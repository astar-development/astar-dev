using AStar.Dev.Infrastructure.AdminDb.Models;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.Update.V1;

/// <summary>
///     The <see cref="UpdateScrapeDirectoriesResponse" /> class containing the details of the updated Scrape Directories.
/// </summary>
/// <param name="scrapeDirectory"></param>
public sealed class UpdateScrapeDirectoriesResponse(ScrapeDirectory scrapeDirectory)
{
    /// <summary>
    ///     Gets or sets the ScrapeDirectoryId - whilst the name sounds the same, this is the Scrape-directory-specific ID and
    ///     is used as
    ///     part of the Historical modelling for each Scrape Directory contained within the table.
    /// </summary>
    public Guid ScrapeDirectoryId { get; } = scrapeDirectory.ScrapeDirectoryId;

    /// <summary>
    ///     Gets or sets the Root Directory for everything - search and save.
    /// </summary>
    public string RootDirectory { get; } = scrapeDirectory.RootDirectory;

    /// <summary>
    ///     Gets or sets the Base Save Directory for the saving post search - appended to the root directory.
    /// </summary>
    public string BaseSaveDirectory { get; } = scrapeDirectory.BaseSaveDirectory;

    /// <summary>
    ///     Gets or sets the Base Directory for the search checks - appended to the root directory.
    /// </summary>
    public string BaseDirectory { get; } = scrapeDirectory.BaseDirectory;

    /// <summary>
    ///     Gets or sets the Base Directory Famous for the search checks for famous people - appended to the root directory.
    /// </summary>
    public string BaseDirectoryFamous { get; } = scrapeDirectory.BaseDirectoryFamous;

    /// <summary>
    ///     Gets or sets the default subdirectory name for the save - appended to the root directory.
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
