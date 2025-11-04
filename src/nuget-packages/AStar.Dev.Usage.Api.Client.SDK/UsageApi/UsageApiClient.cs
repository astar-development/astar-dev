using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AStar.Dev.Api.HealthChecks;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Usage.Api.Client.SDK.UsageApi;

/// <summary>
///     The <see href="Images.ApiClient"></see> class
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
[Refactor(5, 10, "This class needs to be refactored / rewritten")]
public sealed class UsageApiClient(HttpClient httpClient, /*ITokenAcquisition tokenAcquisitionService,*/ ILogger<UsageApiClient> logger) : IApiClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public async Task<HealthStatusResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Checking the {ApiName} Health Status.", Constants.ApiName);

            HttpResponseMessage response = await httpClient.GetAsync("/health/live", cancellationToken);

            return response.IsSuccessStatusCode
                ? await ReturnLoggedSuccess(response)
                : ReturnLoggedFailure(response);
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return new HealthStatusResponse { Status = $"Could not get a response from the {Constants.ApiName}." };
        }
    }

    private async Task<HealthStatusResponse> ReturnLoggedSuccess(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health check completed successfully.", Constants.ApiName);

        return (await JsonSerializer.DeserializeAsync<HealthStatusResponse>(await response.Content.ReadAsStreamAsync(),
            JsonSerializerOptions))!;
    }

    private HealthStatusResponse ReturnLoggedFailure(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health failed - {FailureReason}.", Constants.ApiName,
            response.ReasonPhrase);

        return new HealthStatusResponse { Status = $"Health Check failed - {response.ReasonPhrase}." };
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<string, List<ApiUsageEvent>>> GetApiUsageEventsAsync()
    {
        var requestUri = "usage?&version=1.0";
        var token = string.Empty; //await tokenAcquisitionService.GetAccessTokenForUserAsync(["api://54861ab2-fdb0-4e18-a073-c90e7bf9f0c5/ToDoList.Write",]);

        // logger.LogDebug("Token: {Token}", token);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response = await httpClient.GetAsync(requestUri);

        return (response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<Dictionary<string, List<ApiUsageEvent>>>()
            : new Dictionary<string, List<ApiUsageEvent>>())!;
    }
}
