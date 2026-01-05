namespace AStar.Dev.Usage.Logger;

public sealed class HalfSecondPeriodicTimer : IPeriodicTimer
{
    private readonly PeriodicTimer timer = new(TimeSpan.FromMilliseconds(500));

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        => await timer.WaitForNextTickAsync(cancellationToken);

    public void Dispose()
        => timer.Dispose();
}
