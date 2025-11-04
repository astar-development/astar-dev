using AStar.Dev.Technical.Debt.Reporting;
using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Core.Models;

/// <summary>
///     The <see cref="DirectoryChange" /> to control the directory change
/// </summary>
/// <param name="Old">The name of the old directory</param>
/// <param name="New">The name of the new directory</param>
[Refactor(1, 1, "Not sure this is / will ever be used. If it is / will be - doesn't it need the file too?")]
public record DirectoryChange(string Old, string New)
{
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => this.ToJson();
}
