using System.Text.Json;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Test.DbContext.Helpers.Fixtures;

/// <summary>
/// </summary>
public static class MockFilesContextFactory
{
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static async Task<FilesContext> CreateMockFilesContextAsync()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        IConfiguration config = TestSetup.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = config.GetConnectionString("SqlServer");
        _ = optionsBuilder.UseSqlServer(connectionString, contextOptionsBuilder => contextOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null));
        var testFilesContext = new FilesContext(optionsBuilder.Options);

        try
        {
            _ = await testFilesContext.Database.EnsureCreatedAsync();
        }
        catch
        {
            // NAR
        }

        return testFilesContext;
    }

    /// <summary>
    /// </summary>
    /// <param name="mockFilesContext"></param>
    public static void AddMockFiles(this FilesContext mockFilesContext)
    {
        var combine = Path.Combine(Directory.GetCurrentDirectory(), "../../../../../../src/nuget-packages/AStar.Dev.Test.DbContext.Helpers/TestData/files.json");
        var filesAsJson = File.ReadAllText(combine);

        IEnumerable<FileDetail> listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson, JsonSerializerOptions.Web)!;

        foreach(FileDetail item in listFromJson)
        {
            item.FileHandle = FileHandle.Create($"{item.DirectoryName}-{item.FileName}-{item.Id}");
            _ = mockFilesContext.Files.Add(item);
        }

        try
        {
            _ = mockFilesContext.SaveChanges();
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}
