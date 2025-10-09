using System.ComponentModel.DataAnnotations;
using AStar.Dev.Technical.Debt.Reporting;
using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Core.Models;

/// <summary>
///     The <see cref="DirectoryChanges" /> used to contain the list of available directories - not sure if this will be used
/// </summary>
[Refactor(1, 1, "Not sure this is / will ever be used")]
public sealed class DirectoryChanges
{
    /// <summary>
    /// </summary>
    public const string SectionLocation = "DirectoryChanges";

    /// <summary>
    /// </summary>
    [Required]
    public DirectoryChange[] Directories { get; set; } = [];

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => this.ToJson();
}
