#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicatesCountView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 IF OBJECT_ID(N'[files].[vw_DuplicatesDetails]', N'V') IS NULL
                                 BEGIN
                                    EXEC sys.sp_executesql N'CREATE VIEW [files].[vw_DuplicateCounts]
                                 AS
                                  SELECT COUNT(*) AS Instances,
                                        [Height]
                                        ,[Width]
                                        ,[FileSize]
                                  FROM
                                      FilesDb.files.FileDetail AS FD
                                  INNER JOIN [files].[ImageDetails] as ImageD ON ImageD.Id = FD.ImageDetailsId
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
                                 IF OBJECT_ID(N'[files].[vw_DuplicatesDetails]', N'V') IS NOT NULL
                                 BEGIN
                                    EXEC sys.sp_executesql N'DROP VIEW [files].[vw_DuplicatesDetails];'
                                 END
                                 """);
        }
    }
}
