using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AStar.Dev.Admin.Api.Client.Sdk.Models;
using AStar.Dev.Api.HealthChecks;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Admin.Api.Client.Sdk.AdminApi;

/// <summary>
///     The <see href="AdminApiClient"></see> class.
/// </summary>
public sealed class AdminApiClient(HttpClient httpClient, /*ITokenAcquisition tokenAcquisitionService,*/ ILogger<AdminApiClient> logger) : IApiClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public async Task<HealthStatusResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Checking the {ApiName} Health Status.", Constants.ApiName);

        try
        {
            logger.LogInformation("Checking the {ApiName} Health Status.", Constants.ApiName);

            HttpResponseMessage response = await httpClient.GetAsync("/health/ready", cancellationToken);

            return response.IsSuccessStatusCode
                       ? (await JsonSerializer.DeserializeAsync<HealthStatusResponse>(await response.Content.ReadAsStreamAsync(cancellationToken), JsonSerializerOptions, cancellationToken))!
                       : ReturnLoggedFailure(response);
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return new HealthStatusResponse { Status = $"Could not get a response from the {Constants.ApiName}.", };
        }
    }

    /// <summary>
    ///     The GetSiteConfigurationAsync method will get the User Configuration.
    /// </summary>
    /// <returns>The Site Configuration - populated or empty</returns>
    public async Task<IEnumerable<SiteConfiguration>> GetSiteConfigurationAsync()
    {
        var token = string.Empty; // await tokenAcquisitionService.GetAccessTokenForUserAsync(["api://2ca26585-5929-4aae-86a7-a00c3fc2d061/ToDoList.Write",]);

        // logger.LogDebug("Token: {Token}", token);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await httpClient.GetAsync("site-configurations?version=1.0");

        return (await response.Content.ReadFromJsonAsync<IEnumerable<SiteConfiguration>>())!;
    }

    private HealthStatusResponse ReturnLoggedFailure(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health failed - {FailureReason}.", Constants.ApiName, response.ReasonPhrase);

        return new HealthStatusResponse { Status = $"Health Check failed - {response.ReasonPhrase}.", };
    }

    //
    // /// <summary>
    // ///     The GetModelsToIgnoreAsync method will get the models to ignore.
    // /// </summary>
    // /// <returns>A collection of 0 or more models to ignore</returns>
    // public async Task<IEnumerable<ModelToIgnore>> GetModelsToIgnoreAsync()
    // {
    //     logger.LogInformation("Getting the models-to-ignore.");
    //     var response = await httpClient.GetAsync("models-to-ignore?version=1");
    //
    //     return response.IsSuccessStatusCode
    //                ? (await response.Content.ReadAsStringAsync()).FromJson<IEnumerable<ModelToIgnore>>(Utilities.Constants
    //                                                                                                             .WebDeserialisationSettings)
    //                : [];
    // }
    //
    // /// <summary>
    // ///     The GetScrapeDirectoriesAsync method will get the Scrape Directories.
    // /// </summary>
    // /// <returns>The Scrape Directories - populated or empty</returns>
    // public async Task<ScrapeDirectories> GetScrapeDirectoriesAsync()
    // {
    //     logger.LogInformation("Getting the scrape-directories.");
    //     var response = await httpClient.GetAsync("scrape-directories?version=1");
    //
    //     return response.IsSuccessStatusCode
    //                ? (await response.Content.ReadAsStringAsync()).FromJson<ScrapeDirectories>(Utilities.Constants
    //                                                                                                    .WebDeserialisationSettings)
    //                : new ScrapeDirectories();
    // }
    //
    // /// <summary>
    // ///     The GetSearchConfigurationAsync method will get the Search Configuration.
    // /// </summary>
    // /// <returns>The Search Configuration - populated or empty</returns>
    // public async Task<SearchConfiguration> GetSearchConfigurationAsync()
    // {
    //     logger.LogInformation("Getting the search-configuration.");
    //     var response = await httpClient.GetAsync("search-configuration?version=1");
    //
    //     return response.IsSuccessStatusCode
    //                ? (await response.Content.ReadAsStringAsync()).FromJson<SearchConfiguration>(Utilities.Constants
    //                                                                                                      .WebDeserialisationSettings)
    //                : new SearchConfiguration();
    // }
    //
    // /// <summary>
    // ///     The GetTagsToIgnoreAsync method will get the Tags to Ignore.
    // /// </summary>
    // /// <returns>A collection of 0 or more Tags to Ignore</returns>
    // public async Task<IEnumerable<TagToIgnore>> GetTagsToIgnoreAsync()
    // {
    //     logger.LogInformation("Getting the Tags-to-ignore.");
    //     var response = await httpClient.GetAsync("tags-to-ignore?version=1");
    //
    //     return response.IsSuccessStatusCode
    //                ? (await response.Content.ReadAsStringAsync()).FromJson<IEnumerable<TagToIgnore>>(Utilities.Constants
    //                                                                                                           .WebDeserialisationSettings)
    //                : [];
    // }
    //
    // /// <summary>
    // ///     The GetUserConfigurationAsync method will get the User Configuration.
    // /// </summary>
    // /// <returns>The User Configuration - populated or empty</returns>
    // public async Task<UserConfiguration> GetUserConfigurationAsync()
    // {
    //     logger.LogInformation("Getting the User Configuration.");
    //     var response = await httpClient.GetAsync("user-configuration?version=1");
    //
    //     return response.IsSuccessStatusCode
    //                ? (await response.Content.ReadAsStringAsync()).FromJson<UserConfiguration>(Utilities.Constants
    //                                                                                                    .WebDeserialisationSettings)
    //                : new UserConfiguration();
    // }
}
