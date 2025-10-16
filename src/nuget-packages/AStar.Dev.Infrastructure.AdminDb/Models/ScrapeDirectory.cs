using AStar.Dev.Infrastructure.Data;

namespace AStar.Dev.Infrastructure.AdminDb.Models;

/// <summary>
///     The <see href="ScrapeDirectories"></see> class containing the Scrape Directories Configuration and extending <see cref="AuditableEntity" />
/// </summary>
public sealed class ScrapeDirectory : AuditableEntity
{
    /// <summary>
    ///     Gets or sets The ID of the configuration
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    ///     Gets or sets the ScrapeDirectoryId - whilst the name sounds the same; this is the Scrape-directory-specific ID and
    ///     is used as part of the Historical modelling for each Scrape Directory contained within the table
    /// </summary>
    public Guid ScrapeDirectoryId { get; init; } = Guid.CreateVersion7();

    /// <summary>
    ///     Gets or sets the Root Directory for everything - search and save
    /// </summary>
    public string RootDirectory { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Save Directory for the saving post search - appended to the root directory
    /// </summary>
    public string BaseSaveDirectory { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Directory for the search checks - appended to the root directory
    /// </summary>
    public string BaseDirectory { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Directory Famous for the search checks for famous people - appended to the root directory
    /// </summary>
    public string BaseDirectoryFamous { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the default subdirectory name for the save - appended to the root directory
    /// </summary>
    public string SubDirectoryName { get; init; } = string.Empty;
}
