namespace AStar.Dev.Database.Updater.Core.Models;

/// <summary>
///     The <see cref="DatabaseUpdaterConfiguration" /> containing the applicable Database Updater configuration
/// </summary>
public class DatabaseUpdaterConfiguration
{
    /// <summary>
    ///     Gets the name of the configuration section as specified in the appSettings.json
    /// </summary>
    public const string ConfigurationSectionName = "databaseUpdaterConfiguration";

    /// <summary>
    ///     Gets or sets the Root directory - where the updater will start looking for files
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the full path (including the name) of the CSV Mappings file
    /// </summary>
    public string MappingsFilePath { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the time to start the soft-deletion
    /// </summary>
    public TimeOnly SoftDeleteScheduledTime { get; set; }

    /// <summary>
    ///     Gets or sets the time to start the hard-deletion
    /// </summary>
    public TimeOnly HardDeleteScheduledTime { get; set; }

    /// <summary>
    ///     Gets or sets the time to start the new files addition
    /// </summary>
    public TimeOnly NewFilesScheduledTime { get; set; }

    /// <summary>
    /// </summary>
    public bool HonourFirstDelay { get; set; }
}
