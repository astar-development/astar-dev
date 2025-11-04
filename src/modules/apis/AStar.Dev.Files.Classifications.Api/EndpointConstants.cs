namespace AStar.Dev.Files.Classifications.Api;

/// <summary>
///     Provides constant values related to API endpoints
///     for file classifications in the application.
/// </summary>
/// <remarks>
///     This class contains static string constants that are used
///     across the application for defining and interacting with
///     the file classifications API. These constants include endpoint
///     names, group names, and tags.
/// </remarks>
/// <example>
///     The following demonstrates the usage of <see cref="EndpointConstants" /> in
///     an endpoint registration and handler:
///     <code>
/// endpointRouteBuilder.NewVersionedApi(EndpointConstants.GetFileClassificationsGroupName)
/// .MapGroup(EndpointConstants.FileClassificationsEndpoint)
/// .WithTags(EndpointConstants.FileClassificationsTag)
/// .WithName(EndpointConstants.GetFileClassificationsName);
/// </code>
///     Example for creating a request:
///     <code>
/// var request = new GetFileClassificationRequest
/// {
/// Name = EndpointConstants.GetFileClassificationsGroupName
/// };
/// </code>
/// </example>
public static class EndpointConstants
{
    /// <summary>
    ///     Gets the name identifier for the "Get File Classifications" operation,
    ///     formatted as a single string with spaces removed to ensure uniqueness.
    ///     This is primarily used to reference the operation in code, routing, or documentation.
    /// </summary>
    /// <value>
    ///     A string value "GetFileClassifications", derived from the <c>GetFileClassificationsGroupName</c>
    ///     by removing all spaces.
    /// </value>
    /// <example>
    ///     Below is an example of how to access and use <c>GetFileClassificationsName</c>:
    ///     <code>
    /// string endpointName = EndpointConstants.GetFileClassificationsName;
    /// Console.WriteLine(endpointName); // Output: "GetFileClassifications"
    /// </code>
    ///     This can also be used as part of an endpoint definition:
    ///     <code>
    /// endPointBuilder
    /// .MapGet("/", handler)
    /// .WithName(EndpointConstants.GetFileClassificationsName);
    /// </code>
    /// </example>
    public static string GetFileClassificationsName => GetFileClassificationsGroupName.Replace(" ", string.Empty);

    /// <summary>
    ///     Gets the group name identifier for the "Get File Classifications" operation.
    ///     This name is primarily used to define endpoint groups, versioning, and documentation for the API.
    /// </summary>
    /// <value>
    ///     A string value "Get File Classifications", representing a human-readable description of the operation group.
    /// </value>
    /// <example>
    ///     Below is an example of how to access and use <c>GetFileClassificationsGroupName</c>:
    ///     <code>
    /// string groupName = EndpointConstants.GetFileClassificationsGroupName;
    /// Console.WriteLine(groupName); // Output: "Get File Classifications"
    /// </code>
    ///     This can also be used as part of the API version group definition:
    ///     <code>
    /// IVersionedEndpointRouteBuilder versionedApi = endpointRouteBuilder.NewVersionedApi(EndpointConstants.GetFileClassificationsGroupName);
    /// </code>
    /// </example>
    public static string GetFileClassificationsGroupName => "Get File Classifications";

    /// <summary>
    ///     Represents a tag identifier for the "File Classifications" API endpoints.
    ///     This tag is used to categorize and group all related endpoints under the
    ///     "File Classifications" category in documentation or middleware handling.
    /// </summary>
    /// <value>
    ///     A string value "File Classifications" that serves as the descriptive tag
    ///     for endpoints associated with handling file classifications.
    /// </value>
    /// <example>
    ///     Below is an example of how to use <c>FileClassificationsTag</c> when setting up endpoint routing:
    ///     <code>
    /// _ = apiGroup.MapGet("/", async ([FromServices] FilesContext filesContext,
    /// [FromServices] GetFileClassificationsHandler handler,
    /// CancellationToken cancellationToken)
    /// => await handler.HandleAsync(filesContext, cancellationToken))
    /// .Produces&lt;IReadOnlyCollection&lt;GetFileClassificationsResponse&gt;&gt;()
    /// .Produces(401)
    /// .Produces(403)
    /// .WithName(EndpointConstants.GetFileClassificationsName)
    /// .WithTags(EndpointConstants.FileClassificationsTag);
    /// </code>
    ///     This tag will categorize the endpoint for API documentation or Swagger UI, ensuring clear
    ///     grouping under the "File Classifications" category.
    /// </example>
    public static string FileClassificationsTag => "File Classifications";

    /// <summary>
    ///     Represents the endpoint path for handling file classifications API requests.
    ///     This value is used to define the base route when mapping the endpoint within the application.
    /// </summary>
    /// <value>
    ///     A string value representing the base path of the file classifications endpoint,
    ///     currently set to "/api/file-classifications".
    /// </value>
    /// <example>
    ///     Example usage of the <c>FileClassificationsEndpoint</c>:
    ///     <code>
    /// var endpointPath = EndpointConstants.FileClassificationsEndpoint;
    /// Console.WriteLine(endpointPath); // Output: "/api/file-classifications"
    /// </code>
    ///     This value is typically used as part of an endpoint route definition:
    ///     <code>
    /// endpointRouteBuilder
    /// .MapGroup(EndpointConstants.FileClassificationsEndpoint)
    /// .MapGet("/", handler)
    /// .WithTags(EndpointConstants.FileClassificationsTag);
    /// </code>
    ///     You can also access it dynamically to construct full endpoint routes:
    ///     <code>
    /// string fullUrl = $"{baseApiUrl}{EndpointConstants.FileClassificationsEndpoint}";
    /// Console.WriteLine(fullUrl); // Output: "https://example.com/api/file-classifications"
    /// </code>
    /// </example>
    public static string FileClassificationsEndpoint => "/api/file-classifications";
}
