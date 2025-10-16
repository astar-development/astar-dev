using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AStar.Dev.Database.Updater.Core;

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
    [JsonPropertyName("rootDirectory")]
    [Required]
    public required string RootDirectory { get; set; }

    /// <summary>
    ///     Gets or sets whether to honour (i.e.: not skip) the first delay of the scheduled run.
    /// </summary>
    [Required]
    public required bool HonourFirstDelay { get; set; }

    /// <summary>
    ///     Gets or sets whether to run the new files service
    /// </summary>
    [Required]
    public required bool RunNewFilesService { get; set; }

    /// <summary>
    ///     Gets or sets the full path (including the name) of the CSV Mappings file
    /// </summary>
    [Required]
    public required string MappingsFilePath { get; set; }

    /// <summary>
    ///     Gets or sets the time to start the soft-deletion
    /// </summary>
    [Required]
    public required TimeOnly SoftDeleteScheduledTime { get; set; }

    /// <summary>
    ///     Gets or sets the time to start the hard-deletion
    /// </summary>
    [Required]
    public required TimeOnly HardDeleteScheduledTime { get; set; }

    /// <summary>
    ///     Gets or sets the time to start the new files addition
    /// </summary>
    [Required]
    public required TimeOnly NewFilesScheduledTime { get; set; }
}
