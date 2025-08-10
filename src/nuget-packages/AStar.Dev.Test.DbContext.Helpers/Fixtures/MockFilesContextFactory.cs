using System.Text.Json;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Test.DbContext.Helpers.Fixtures;

public static class MockFilesContextFactory
{
    public static async Task<FilesContext> CreateMockFilesContextAsync()
    {
        var optionsBuilder   = new DbContextOptionsBuilder<FilesContext>();
        var config           = TestSetup.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = config.GetConnectionString("SqlServer");
        optionsBuilder.UseSqlServer(connectionString, contextOptionsBuilder => contextOptionsBuilder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null));
        var testFilesContext = new FilesContext(optionsBuilder.Options);

        try
        {
            await testFilesContext.Database.EnsureCreatedAsync();
        }
        catch
        {
            // NAR
        }

        return testFilesContext;
    }

    public static void AddMockFiles(this FilesContext mockFilesContext)
    {
        var combine     = Path.Combine(Directory.GetCurrentDirectory(), "../../../../../../src/nuget-packages/AStar.Dev.Test.DbContext.Helpers/TestData/files.json");
        var filesAsJson = File.ReadAllText(combine);

        var listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson, JsonSerializerOptions.Web)!;

        foreach(var item in listFromJson)
        {
            item.FileHandle = FileHandle.Create($"{item.DirectoryName}-{item.FileName}-{item.Id}");
            mockFilesContext.FileDetails.Add(item);
        }

        try
        {
            mockFilesContext.SaveChanges();
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}
