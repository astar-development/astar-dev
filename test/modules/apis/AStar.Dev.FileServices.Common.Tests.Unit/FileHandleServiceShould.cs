using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.FileServices.Common.Tests.Unit;

public class FileHandleServiceShould
{
    private readonly FileHandleService _sut = new();

    [Theory]
    [InlineData("document.pdf", "document-pdf")]
    [InlineData("my file.txt", "my-file-txt")]
    [InlineData("source_code.cs", "source-code-cs")]
    [InlineData("report(2023).xlsx", "report-2023-xlsx")]
    [InlineData("data+analysis.json", "data-analysis-json")]
    [InlineData("notes,draft.md", "notes-draft-md")]
    [InlineData("-readme.txt", "readme-txt")]
    [InlineData("config--file.xml", "config-file-xml")]
    public void GenerateValidFileHandleFromFileName(string fileName, string expectedHandle)
    {
        var fileDetail = new FileDetail { FileName = new FileName(fileName), DirectoryName = new DirectoryName("/test/path") };
        var existingHandles = new List<FileHandle>();

        var result = _sut.GenerateFileHandle(fileDetail, existingHandles);

        result.Value.ShouldBe(expectedHandle);
        existingHandles.Count.ShouldBe(1);
        existingHandles[0].ShouldBe(result);
    }

    [Fact]
    public void AppendGuidWhenHandleAlreadyExists()
    {
        var fileName = "document.pdf";
        var fileDetail = new FileDetail { FileName = new FileName(fileName), DirectoryName = new DirectoryName("/test/path") };
        var existingHandles = new List<FileHandle> { FileHandle.Create("document-pdf") };

        var result = _sut.GenerateFileHandle(fileDetail, existingHandles);

        result.Value.ShouldStartWith(result.Value);
        result.Value.ShouldNotBe("document-pdf");
        result.Value.Length.ShouldBeGreaterThan("document-pdf".Length);
        existingHandles.Count.ShouldBe(2);
    }

    [Fact]
    public void TruncateLongFileNamesTo350Characters()
    {
        var longFileName = new string('a', 500) + ".jpg";
        var fileDetail = new FileDetail { FileName = new FileName(longFileName), DirectoryName = new DirectoryName("/test/path") };
        var existingHandles = new List<FileHandle>();

        var result = _sut.GenerateFileHandle(fileDetail, existingHandles);

        result.Value.Length.ShouldBeLessThanOrEqualTo(350);
        existingHandles.Count.ShouldBe(1);
    }
}
