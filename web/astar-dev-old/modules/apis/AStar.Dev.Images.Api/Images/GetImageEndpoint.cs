using System.IO.Abstractions;
using Asp.Versioning.Builder;
using AStar.Dev.Images.Api.Extensions;
using AStar.Dev.Images.Api.Models;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace AStar.Dev.Images.Api.Images;

public sealed class GetImageEndpoint(WebApplication app) : IEndpoint
{
    private const int MaximumHeightAndWidthForThumbnail = 850;

    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.ImageGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.ImageEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapGet("/", async (
                            [FromServices] ILogger<GetImageEndpoint> logger,
                            [FromServices] IFileSystem fileSystem,
                            [FromServices] FilesContext context,
                            [FromQuery] string imagePath,
                            [FromQuery] int maximumSizeInPixels = MaximumHeightAndWidthForThumbnail,
                            CancellationToken cancellationToken = default) => await Handle(imagePath, maximumSizeInPixels, fileSystem, context, logger, cancellationToken))
           .AddBasicProduces<FileStream>()
           .WithDescription("Get the file Image matching the specified criteria.")
           .WithSummary("Get the file Image")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.ImageTag);
    }

    [Refactor(5, 2, "Should be far more functional")]
    private static async Task<IResult> Handle(string imagePath, int maxPixelSize, IFileSystem fileSystem, FilesContext context, ILogger<GetImageEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting retrieval for: {ImagePath} with a maximum size of: {MaximumSizeInPixels}px", imagePath, maxPixelSize);

        var directory = imagePath.DirectoryName();
        logger.LogDebug("Starting retrieval for directory: {ImagePath} ", directory);
        var filename = imagePath.FileName();
        logger.LogDebug("Starting retrieval for filename: {ImagePath} ", filename);

        FileDetail? fileInfoJb = ReadDb(context, directory, filename);

        await SaveLastViewedDate(context, logger, fileInfoJb, cancellationToken);

        if(!imagePath.IsImage()) return LoggedBadRequest(filename, logger);

        if(!fileSystem.File.Exists(imagePath)) return LoggedNotFound(imagePath, filename, logger);

        MemoryStream outputStream = CreateImageStream(imagePath, maxPixelSize, logger);

        return Results.File(outputStream, "image/jpeg");
    }

    private static async Task SaveLastViewedDate(FilesContext context, ILogger<GetImageEndpoint> logger, FileDetail? fileDetail, CancellationToken cancellationToken)
    {
        if(fileDetail is null) return;

        try
        {
            fileDetail.FileAccessDetail.LastViewed = DateTime.UtcNow;
            logger.LogDebug("Updating Last Viewed for: {FileName}", fileDetail.FullNameWithPath);
            _ = await context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Any error here is not important.
        }
    }

    private static IResult LoggedBadRequest(string filename, ILogger<GetImageEndpoint> logger)
    {
        logger.LogDebug("{FileName} is not an image, exiting...", filename);

        return Results.BadRequest("Unsupported file type.");
    }

    private static IResult LoggedNotFound(string imagePath, string filename, ILogger<GetImageEndpoint> logger)
    {
        logger.LogDebug("{FileName} was not found ({ImagePath}), exiting...", filename, imagePath);

        return Results.NotFound();
    }

    private static MemoryStream CreateImageStream(string imagePath, int maximumSizeInPixels, ILogger<GetImageEndpoint> logger)
    {
        logger.LogDebug("Resized image is being created for: {FileName}", imagePath.FileName());
        var image = SKImage.FromEncodedData(imagePath);

        var imageBitMap = SKBitmap.FromImage(image);

        Dimensions dimensions = CalcNewSize(new Dimensions { Width = image.Width, Height = image.Height, }, maximumSizeInPixels, true);

        using SKBitmap? resizedBitmap = imageBitMap.Resize(new SKImageInfo(dimensions.Width, dimensions.Height), SKSamplingOptions.Default);
        using var       resizedImage  = SKImage.FromBitmap(resizedBitmap);
        var             outputStream  = new MemoryStream();

        resizedImage.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(outputStream);
        _ = outputStream.Seek(0, SeekOrigin.Begin);

        return outputStream;
    }

    private static FileDetail? ReadDb(FilesContext context, string directory, string filename)
    {
        try
        {
            return context.Files.Include(file => file.FileAccessDetail)
                          .FirstOrDefault(f => f.FileName.Value == filename && f.DirectoryName.Value == directory);
        }
        catch
        {
            _ = Task.Delay(TimeSpan.FromSeconds(2));

            return context.Files.Include(file => file.FileAccessDetail)
                          .FirstOrDefault(f => f.FileName.Value == filename && f.DirectoryName.Value == directory);
        }
    }

    private static Dimensions CalcNewSize(Dimensions currentSize, int newWidth, bool resizeIfSmaller)
    {
        var width = newWidth;

        if(!resizeIfSmaller && width > currentSize.Width) width = currentSize.Width;

        var newHeight = CalcNewHeight(currentSize.Width, currentSize.Height, width);

        return new Dimensions { Width = width, Height = newHeight, };
    }

    private static int CalcNewHeight(int currentWidth, int currentHeight, int newWidth)
    {
        var ratio     = (float)newWidth / currentWidth;
        var newHeight = currentHeight   * ratio;

        return (int)newHeight;
    }
}
