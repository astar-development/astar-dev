using System.ComponentModel.DataAnnotations;
using AStar.Dev.Technical.Debt.Reporting;
using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Core.Models;

/// <summary>
/// </summary>
[Refactor(1, 1, "Not sure this is / will be used")]
public sealed class ApiConfiguration
{
    /// <summary>
    /// </summary>
    public const string SectionLocation = "ApiConfiguration";

    /// <summary>
    /// </summary>
    [Required]
    public string[] Directories { get; set; } = [];

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.ToJson();
}
