using System.Collections.ObjectModel;

namespace AStar.Dev.Files.Api.Files.Add.V1;

public class AddFilesEndpoint
{
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IResult> AddFiles(IReadOnlyCollection<FileDetailToAdd> files, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1_000, cancellationToken);

        return Results.Ok(new ReadOnlyCollection<AddFilesResponse>(new List<AddFilesResponse>()));
    }
}
