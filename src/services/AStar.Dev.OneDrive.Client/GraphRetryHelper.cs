using Microsoft.Graph;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions; 

namespace AStar.Dev.OneDrive.Client;

public static class RetryHelper
{
    public static async Task<T> ExecuteWithBackoffAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 5,
        int baseDelayMs = 500,
        int maxJitterMs = 250)
    {
        var attempt = 0;
        var rng = new Random();

        while (true)
        {
            try
            {
                return await operation().ConfigureAwait(false);
            }
            catch (ApiException ex) when (IsTransient(ex))
            {
                attempt++;
                if (attempt > maxRetries) throw;

                var delayMs = ComputeDelay(attempt, baseDelayMs, rng.Next(0, maxJitterMs));
                await Task.Delay(delayMs).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                attempt++;
                if (attempt > maxRetries) throw;

                var delayMs = ComputeDelay(attempt, baseDelayMs, rng.Next(0, maxJitterMs));
                await Task.Delay(delayMs).ConfigureAwait(false);
            }
        }
    }

    private static bool IsTransient(ApiException ex)
    {
        // v5 throws ApiException; 429 is auto-retried by middleware.
        // We backoff on common transient server-side failures.
        var status = (int)ex.ResponseStatusCode;
        return status is >= 500 and < 600;
    }

    private static int ComputeDelay(int attempt, int baseDelayMs, int jitterMs)
        => (int)(Math.Pow(2, attempt) * baseDelayMs) + jitterMs;
}
