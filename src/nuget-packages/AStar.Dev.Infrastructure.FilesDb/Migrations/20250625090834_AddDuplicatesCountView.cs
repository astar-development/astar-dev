using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicatesCountView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 IF OBJECT_ID(N'[files].[vw_DuplicateCounts]', N'V') IS NULL
                                 BEGIN
                                    EXEC sys.sp_executesql N'CREATE VIEW [files].[vw_DuplicateCounts]
                                    AS
                                     SELECT COUNT(*) AS Instances,
                                           [Height]
                                           ,[Width]
                                           ,[FileSize]
                                     FROM
                                         FilesDb.files.FileDetail AS FD
                                     INNER JOIN [files].[FileAccessDetail] as FAD ON FAD.Id = FD.FileAccessDetailId
                                     WHERE HardDeleted = 0
                                     GROUP BY
                                         FileSize,
                                         Height,
                                         Width
                                     HAVING
                                       COUNT(*) > 1;'
                                 END
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 IF OBJECT_ID(N'[files].[vw_DuplicateCounts]', N'V') IS NOT NULL
                                 BEGIN
                                    EXEC sys.sp_executesql N'DROP VIEW [files].[vw_DuplicateCounts];'
                                 END
                                 """);
        }
    }
}
