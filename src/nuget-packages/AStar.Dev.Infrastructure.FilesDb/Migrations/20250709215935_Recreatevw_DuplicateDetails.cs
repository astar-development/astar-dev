using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class Recreatevw_DuplicateDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 IF OBJECT_ID(N'[files].[vw_DuplicatesDetails]', N'V') IS NULL
                                 BEGIN
                                    EXEC sys.sp_executesql N'CREATE VIEW [files].[vw_DuplicatesDetails]
                                    AS
                                    SELECT FD.[Id]
                                       ,[FileName]
                                       ,[DirectoryName]
                                       ,[Dups].[Height]
                                       ,[Dups].[Width]
                                       ,FD.[FileSize]
                                       ,[FileHandle]
                                       ,[IsImage]
                                       , Dups.Instances
                                       , DetailsModified
                                       , FileLastViewed
                                       , SoftDeleted
                                       , SoftDeletePending
                                       , MoveRequired
                                       , HardDeletePending
                                 FROM files.FileDetail as FD
                                 LEFT JOIN [files].[ImageDetails] as ImageD ON ImageD.Id = FD.ImageDetailsId
                                 INNER JOIN [files].[vw_DuplicateCounts] AS Dups ON FD.FileSize = Dups.FileSize AND ImageD.Height = Dups.Height AND ImageD.Width = Dups.Width
                                 WHERE HardDeleted = 0;'
                                 END
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 IF OBJECT_ID(N'[files].[vw_DuplicatesDetails]', N'V') IS NOT NULL
                                 BEGIN
                                    EXEC sys.sp_executesql N'DROP VIEW [files].[vw_DuplicatesDetails];'
                                 END
                                 """);
        }
    }
}
