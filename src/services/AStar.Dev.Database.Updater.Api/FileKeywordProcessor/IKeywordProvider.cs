namespace AStar.Dev.Database.Updater.Api.FileKeywordProcessor;

public interface IKeywordProvider
{
    Task<IReadOnlyList<string>> GetKeywordsAsync(CancellationToken cancellationToken = default);
}
