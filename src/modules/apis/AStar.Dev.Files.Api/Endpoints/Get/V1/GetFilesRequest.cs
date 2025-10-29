using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using Microsoft.AspNetCore.Http;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
///     The <see cref="GetFilesRequest" /> contains the search parameters for the Get
/// </summary>
public class GetFilesRequest : IEndpointName
{
    /// <summary>
    /// </summary>
    [Required]
    public string SearchFolder { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    ///     Gets or sets the exclude files viewed with the past N days. 0 should be used to include viewed
    /// </summary>
    public int ExcludeViewedWithinDays { get; set; }

    /// <summary>
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// </summary>
    [Required]
    public int ItemsPerPage { get; set; } = 10;

    /// <summary>
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter<SortOrder>))]
    public SortOrder SortOrder { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter<SearchType>))]
    public SearchType SearchType { get; set; } = SearchType.Duplicates;

    /// <inheritdoc />
    public string Name => EndpointConstants.GetFilesGroupName;

    /// <inheritdoc />
    public string HttpMethod => HttpMethods.Get;
}
