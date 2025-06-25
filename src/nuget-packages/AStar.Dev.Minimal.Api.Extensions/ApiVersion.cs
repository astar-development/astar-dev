namespace AStar.Dev.Minimal.Api.Extensions;

/// <summary>
///     The <see cref="ApiVersion" /> class contains the parameters used during 'CreatedAtRoute' responses in the APIs
/// </summary>
public sealed class ApiVersion
{
    /// <summary>
    ///     Gets or sets the version of the API - used during 'CreatedAtRoute' responses in the APIs
    /// </summary>
    public string Version { get; set; } = "1.0";
}
