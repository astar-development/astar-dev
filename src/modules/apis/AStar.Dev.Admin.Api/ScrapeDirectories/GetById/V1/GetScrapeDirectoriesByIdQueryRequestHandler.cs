using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.GetById.V1;

/// <summary>
///     The GetScrapeDirectoriesByIdQueryRequestHandler as the name suggests, handles the GetById Site Configuration query.
/// </summary>
/// <param name="dbContext">The instance of <see cref="AdminContext" /> used to retrieve the site configurations from</param>
public sealed class GetScrapeDirectoriesByIdQueryRequestHandler(AdminContext dbContext)
{
    public async Task<IResult> Handle(GetScrapeDirectoriesByIdQuery request, CancellationToken cancellationToken)
    {
        ScrapeDirectory? scrapeDirectory = await dbContext
                                                .ScrapeDirectories
                                                .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                                .Where(site => site.ScrapeDirectoryId == request.ScrapeDirectoryId)
                                                .FirstOrDefaultAsync(cancellationToken);

        return scrapeDirectory is null
                   ? Results.NotFound()
                   : Results.Ok(new GetScrapeDirectoriesByIdResponse(scrapeDirectory));
    }
}
