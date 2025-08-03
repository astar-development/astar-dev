using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace AStar.Dev.Test.Helpers.Unit;

/// <summary>
/// </summary>
public class ExampleTests
{
    /// <summary>
    /// </summary>
    [Fact]
    public void SaveFile_AddsEntityAndCallsSaveChanges()
    {
        // Arrange
        var store       = new List<FileDetail>();
        var mockSet     = DbContextMockFactory.CreateMockDbSet(store);
        var mockContext = Substitute.For<FilesContext>(new DbContextOptions<FilesContext>());
        mockContext.Files.Returns(mockSet);
        var newEntity = new FileDetail { Id = 1, FileName = "Test File" };
        var service   = new FileService(mockContext);

        // Act
        service.SaveFile(newEntity);

        // Assert
        Assert.Contains(store, e => e.FileName == "Test File");
        mockContext.Received(1).SaveChanges();
    }

    /// <summary>
    /// </summary>
    [Fact]
    public async Task SaveFileAsync_AddsAndSaves()
    {
        // Arrange
        var store = new List<FileDetail>();

        var mockContext = DbContextMockFactory.CreateMockDbContext<FilesContext, FileDetail>(
                                                                                             store,
                                                                                             ctx => ctx.Files
                                                                                            );

        var service = new FileService(mockContext);
        var file    = new FileDetail { Id = 42, FileName = "Async File" };

        // Act
        await service.SaveFileAsync(file);

        // Assert
        Assert.Single(store);
        Assert.Equal("Async File", store[0].FileName);
        await mockContext.Received(1).SaveChangesAsync();
    }
}
