namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
/// </summary>
public interface IKeywordProvider
{
    /// <summary>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<FileNamePartsWithClassifications>> GetKeywordsAsync(CancellationToken cancellationToken = default);
}
