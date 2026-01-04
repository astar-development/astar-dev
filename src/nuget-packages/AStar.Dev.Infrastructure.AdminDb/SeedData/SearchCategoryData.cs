using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.AdminDb.SeedData;

/// <summary>
///     The <see cref="SearchCategoryData" /> adds the default Search Category configuration when no entry exists.
/// </summary>
public static class SearchCategoryData
{
    /// <summary>
    ///     The seed method to populate the database with
    /// </summary>
    /// <param name="context">The <see cref="DbContext" /> to populate</param>
    public static void Seed(DbContext context)
    {
        DbSet<SearchCategory> searchCategories = context.Set<SearchCategory>();

        if(searchCategories.Any()) return;

        _ = context.Set<SearchCategory>()
               .Add(new SearchCategory { Id = 2401, Name = "Initial Category", });

        _ = context.SaveChanges();
    }
}
