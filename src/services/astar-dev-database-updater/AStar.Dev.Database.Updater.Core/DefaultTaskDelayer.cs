namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     Default implementation of ITaskDelayer that uses Task.Delay
/// </summary>
public class DefaultTaskDelayer : ITaskDelayer
{
    /// <inheritdoc />
    public Task Delay(TimeSpan delay, CancellationToken cancellationToken) => Task.Delay(delay, cancellationToken);
}
