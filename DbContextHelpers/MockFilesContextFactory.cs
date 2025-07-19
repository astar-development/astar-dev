using System.Text.Json;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DbContextHelpers;

public static class MockFilesContextFactory
{
    public static async Task<FilesContext> CreateMockFilesContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlite(connection);
        var mockContext = new FilesContext(optionsBuilder.Options);
        await mockContext.Database.EnsureCreatedAsync();

        return mockContext;
    }

    public static void AddMockFiles(this FilesContext mockFilesContext)
    {
        var combine = Path.Combine(Directory.GetCurrentDirectory(), "../../../../../../DbContextHelpers/TestData/files.json");
        Console.WriteLine(combine);
        var filesAsJson = File.ReadAllText(combine);

        var listFromJson = JsonSerializer.Deserialize<IEnumerable<FileDetail>>(filesAsJson)!;

        foreach (var item in listFromJson)
        {
            if (IfTheContextAlreadyContainsTheFile(mockFilesContext, item))
            {
                continue;
            }

            item.FileHandle = $"{item.DirectoryName}-{item.FileName}-{item.Id}";
            mockFilesContext.FileDetails.Add(item);
            mockFilesContext.SaveChanges();
        }
    }

    private static bool IfTheContextAlreadyContainsTheFile(FilesContext mockFilesContext, FileDetail item) =>
        mockFilesContext.FileDetails.FirstOrDefault(f => f.FileName == item.FileName && f.DirectoryName == item.DirectoryName) != null;
}
