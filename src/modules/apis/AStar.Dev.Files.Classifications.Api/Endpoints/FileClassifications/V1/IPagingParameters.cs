namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
/// Defines the parameters necessary to support pagination functionality, including the current page
/// and the number of items per page.
/// Objects implementing this interface provide a standardized way to manage paginated data retrieval.
/// </summary>
public interface IPagingParameters
{
    /// <summary>
    /// Gets the current page number for pagination purposes.
    /// This property specifies which page of results should be retrieved.
    /// </summary>
    int CurrentPage { get; }

    /// <summary>
    /// Gets the number of items to be displayed per page in a paginated request.
    /// </summary>
    int ItemsPerPage { get; }
}
