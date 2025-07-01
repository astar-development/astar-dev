namespace AStar.Dev.Test.Helpers.Unit;

/// <summary>
/// </summary>
/// <param name="context"></param>
public class FileService(FilesContext context)
{
    /// <summary>
    /// </summary>
    /// <param name="file"></param>
    public void SaveFile(FileDetail file)
    {
        context.Files.Add(file);
        context.SaveChanges();
    }

    /// <summary>
    /// </summary>
    /// <param name="file"></param>
    public async Task SaveFileAsync(FileDetail file)
    {
        context.Files.Add(file);
        await context.SaveChangesAsync();
    }
}
