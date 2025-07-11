using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class AddVwDuplicateDetails : Migration
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
                                       ,[Dups].[ImageHeight]
                                       ,[Dups].[ImageWidth]
                                       ,FD.[FileSize]
                                       ,[FileHandle]
                                       ,[IsImage]
                                       , Dups.Instances
                                       , UpdatedOn
                                       , FileLastViewed
                                       , SoftDeleted
                                       , SoftDeletePending
                                       , MoveRequired
                                       , HardDeletePending
                                 FROM files.FileDetail as FD
                                 INNER JOIN [files].[vw_DuplicateCounts] AS Dups ON FD.FileSize = Dups.FileSize AND FD.ImageHeight = Dups.[ImageHeight] AND FD.[ImageWidth] = Dups.[ImageWidth]
                                 WHERE HardDeletePending is null;'
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
