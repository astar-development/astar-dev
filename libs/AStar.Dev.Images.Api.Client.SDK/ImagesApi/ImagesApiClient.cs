using System.Net.Http.Headers;
using System.Text.Json;
using AStar.Dev.Api.HealthChecks;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Images.Api.Client.SDK.ImagesApi;

/// <summary>
///     The <see href="Images.ApiClient"></see> class
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
[Refactor(5, 10, "This class needs to be refactored / rewritten")]
public sealed class ImagesApiClient(HttpClient httpClient, /*ITokenAcquisition tokenAcquisitionService,*/ ILogger<ImagesApiClient> logger) : IApiClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public async Task<HealthStatusResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Checking the {ApiName} Health Status", Constants.ApiName);

            HttpResponseMessage response = await httpClient.GetAsync("/health/ready", cancellationToken);

            return response.IsSuccessStatusCode
                       ? await ReturnLoggedSuccess(response)
                       : ReturnLoggedFailure(response);
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return new HealthStatusResponse { Status = $"Could not get a response from the {Constants.ApiName}", };
        }
    }

    private async Task<HealthStatusResponse> ReturnLoggedSuccess(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health check completed successfully", Constants.ApiName);

        return (await JsonSerializer.DeserializeAsync<HealthStatusResponse>(await response.Content.ReadAsStreamAsync(),
                                                                            JsonSerializerOptions))!;
    }

    private HealthStatusResponse ReturnLoggedFailure(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health failed - {FailureReason}", Constants.ApiName,
                              response.ReasonPhrase);

        return new HealthStatusResponse { Status = $"Health Check failed - {response.ReasonPhrase}", };
    }

    /// <summary>
    /// </summary>
    /// <param name="imagePath"></param>
    /// <param name="maximumSizeInPixels"></param>
    /// <param name="thumbnail"></param>
    /// <returns></returns>
    public async Task<Stream> GetImageAsync(string imagePath, int maximumSizeInPixels, bool thumbnail)
    {
        try

        {
            logger.LogDebug("imagePath: {ImagePath}", imagePath);
            var requestUri = $"image?imagePath={Uri.EscapeDataString(imagePath.Replace("\\", "/"))}&maximumSizeInPixels={maximumSizeInPixels}&thumbnail={thumbnail}&version=1.0";
            logger.LogDebug("requestUri: {RequestUri}", requestUri);
            var token = string.Empty; // await tokenAcquisitionService.GetAccessTokenForUserAsync(["api://54861ab2-fdb0-4e18-a073-c90e7bf9f0c5/ToDoList.Write",]);

            logger.LogDebug("requestUri: {RequestUri}", requestUri);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await httpClient.GetAsync(requestUri);

            return response.IsSuccessStatusCode
                       ? await response.Content.ReadAsStreamAsync()
                       : CreateNotFoundMemoryStream(imagePath);
        }
        catch
        {
            var memoryStream = new MemoryStream();

            return memoryStream;
        }
    }

    private MemoryStream CreateNotFoundMemoryStream(string fileName)
    {
        logger.LogInformation("Could not find: {FileName}", fileName);

        return new MemoryStream();
    }
}
