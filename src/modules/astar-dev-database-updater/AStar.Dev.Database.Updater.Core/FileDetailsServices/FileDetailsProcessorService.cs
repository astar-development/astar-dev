using System.Text.RegularExpressions;
using AStar.Dev.FileServices.Common;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
/// </summary>
public class FileDetailsProcessorService(FileHandleService fileHandleService, ILogger<FileDetailsProcessorService> logger)
{
    /// <summary>
    /// </summary>
    /// <param name="fileDetail"></param>
    /// <param name="classifications"></param>
    /// <param name="fileHandlesAlreadyInTheContext"></param>
    /// <param name="counter"></param>
    /// <param name="writeCount"></param>
    /// <param name="keywords"></param>
    /// <returns></returns>
    public (int counter, int writeCount) ProcessFileDetailAsync(FileDetail fileDetail, List<FileClassification> classifications, List<FileHandle> fileHandlesAlreadyInTheContext,
        int counter, int writeCount, IReadOnlyList<FileNamePartsWithClassifications> keywords)
    {
        Regex regex = KeywordRegexBuilder.BuildKeywordPattern(keywords);

        var nameToCheck = fileDetail.FullNameWithPath.SanitizeFilePath();

        IEnumerable<string> matches = GetFilenameMatches(regex, nameToCheck).Where(m => m.Length > 0);
        FileClassification unmatchedClassification = classifications.First(xx => xx.Name == "None");
        counter = ProcessMatches(matches, counter, classifications, fileDetail, unmatchedClassification);

        fileDetail.FileHandle = fileHandleService.GenerateFileHandle(fileDetail, fileHandlesAlreadyInTheContext);

        if(fileDetail.FileName.Value.IsImage()) _ = UpdateFileDetailsForImage(fileDetail).TapError(exception => TemporaryLogFileScanningError(exception, fileDetail.FullNameWithPath));

        return (counter, writeCount);
    }

    private void TemporaryLogFileScanningError(Exception e, string path) => logger.LogError(e, "An error occurred while scanning file: {FileName}", path);

    private int ProcessMatches(IEnumerable<string> nonEmptyMatches, int counter, List<FileClassification> classifications, FileDetail fileWithClassifications,
        FileClassification unmatchedClassification)
    {
        foreach(var keyword in nonEmptyMatches)
        {
            counter++;

            IEnumerable<FileClassification> fileClassifications = GetFileClassifications(classifications, fileWithClassifications.FullNameWithPath);
            foreach(FileClassification fileClassification in fileClassifications) fileWithClassifications.FileClassifications.Add(fileClassification);

            if(counter % 100 == 0) logger.LogInformation("Found keyword: {Keyword} in file: {FileName}", keyword, fileWithClassifications.FullNameWithPath);
        }

        if(fileWithClassifications.FileClassifications.Count == 0) fileWithClassifications.FileClassifications.Add(unmatchedClassification);

        return counter;
    }

    private static string[] GetFilenameMatches(Regex regex, string nameToCheck)
        => regex.Matches(nameToCheck)
                .Select(m => m.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

    private static IEnumerable<FileClassification> GetFileClassifications(List<FileClassification> fileClassifications, string file)
        => from fileClassification in fileClassifications
           from fileNamePart in fileClassification.FileNameParts
           where file.Contains(fileNamePart.Text)
           select fileClassification;

    private static Result<FileDetail, Exception> UpdateFileDetailsForImage(FileDetail fileDetail)
        => Try.Run(() =>
                   {
                       fileDetail.IsImage = true;
                       var image = SKImage.FromEncodedData(fileDetail.FullNameWithPath);

                       if(image is null) return fileDetail;

                       fileDetail.ImageDetail = new ImageDetail(image.Width, image.Height);

                       return fileDetail;
                   });
}
