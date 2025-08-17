namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     Interface for task delay operations, extracted for better testability
/// </summary>
public interface ITaskDelayer
{
    /// <summary>
    ///     Creates a delay for the specified duration, allowing cancellation using a cancellation token.
    /// </summary>
    /// <param name="delay">The time span for which the delay should last.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the delay operation.</param>
    /// <returns>A task that completes after the delay duration or when the operation is cancelled.</returns>
    Task Delay(TimeSpan delay, CancellationToken cancellationToken);
}
