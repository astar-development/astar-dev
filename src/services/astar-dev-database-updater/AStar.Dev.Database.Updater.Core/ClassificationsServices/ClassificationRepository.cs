using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Database.Updater.Core.ClassificationsServices;

/// <summary>
///     Handles data access for FileClassification entities using EF Core.
/// </summary>
public class ClassificationRepository(FilesContext context)
{
    /// <summary>
    ///     Retrieves existing classifications from the database, including their file name parts.
    /// </summary>
    /// <param name="names">Set of classification names to look up.</param>
    /// <returns>Dictionary of classification name to FileClassification.</returns>
    public Dictionary<string, FileClassification> GetExistingClassifications(HashSet<string> names)
        => context.FileClassifications
                  .Include(fc => fc.FileNameParts)
                  .Where(fc => names.Contains(fc.Name))
                  .ToDictionary(fc => fc.Name, fc => fc);

    /// <summary>
    ///     Retrieves existing classifications from the database, including their file name parts.
    /// </summary>
    /// <returns>Dictionary of classification name to FileClassification.</returns>
    public List<FileClassification> GetExistingClassifications()
        => context.FileClassifications
                  .Include(fc => fc.FileNameParts)
                  .ToList();

    /// <summary>
    ///     Adds new classifications to the database context.
    /// </summary>
    /// <param name="classifications">List of new FileClassification entities.</param>
    public void AddClassifications(IEnumerable<FileClassification> classifications)
        => context.FileClassifications.AddRange(classifications);

    /// <summary>
    /// </summary>
    public async Task SaveChangesAsync(CancellationToken stoppingTask)
        => await context.SaveChangesAsync(stoppingTask);
}
