namespace AStar.Dev.Admin.Api.Client.Sdk.Models;

/// <summary>
///     The <see href="ScrapeDirectories"></see> class containing the Scrape Directories Configuration.
/// </summary>
public sealed class ScrapeDirectories
{
    /// <summary>
    ///     Gets or sets The ID of the configuration.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the Root Directory for everything - search and save.
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Save Directory for the saving post search - appended to the root directory.
    /// </summary>
    public string BaseSaveDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Directory for the search checks - appended to the root directory.
    /// </summary>
    public string BaseDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Directory Famous for the search checks for famous people - appended to the root directory.
    /// </summary>
    public string BaseDirectoryFamous { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the default subdirectory name for the save - appended to the root directory.
    /// </summary>
    public string SubDirectoryName { get; set; } = string.Empty;
}
