using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class AddVwDuplicateCounts : Migration
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
                                       [ImageHeight]
                                       ,[ImageWidth]
                                       ,[FileSize]
                                 FROM
                                     FilesDb.files.FileDetail AS FD
                                 WHERE HardDeletePending is null
                                 GROUP BY
                                     FileSize,
                                     [ImageHeight],
                                     [ImageWidth]
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
                                    EXEC sys.sp_executesql N'DROP VIEW [files].[vw_DuplicatesDetails];'
                                 END
                                 """);
        }
    }
}
