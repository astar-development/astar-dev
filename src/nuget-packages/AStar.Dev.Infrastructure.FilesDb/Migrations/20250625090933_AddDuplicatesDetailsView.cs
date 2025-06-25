using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicatesDetailsView : Migration
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
                                       ,[FileAccessDetailId]
                                       ,[FileName]
                                       ,[DirectoryName]
                                       ,FD.[Height]
                                       ,FD.[Width]
                                       ,FD.[FileSize]
                                       ,[FileHandle]
                                       ,[IsImage]
                                       , Dups.Instances
                                       , FAD.DetailsLastUpdated
                                       , LastViewed
                                       , SoftDeleted
                                       , SoftDeletePending
                                       , MoveRequired
                                       , HardDeletePending
                                     FROM files.FileDetail as FD
                                     INNER JOIN [files].[vw_DuplicateCounts] AS Dups ON FD.FileSize = Dups.FileSize AND FD.Height = Dups.Height AND FD.Width = Dups.Width
                                     INNER JOIN [files].[FileAccessDetail] as FAD ON FAD.Id = FD.FileAccessDetailId
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
