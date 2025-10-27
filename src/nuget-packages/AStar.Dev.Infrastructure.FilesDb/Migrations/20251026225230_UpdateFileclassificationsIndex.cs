using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFileclassificationsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileClassification_Name",
                schema: "files",
                table: "FileClassification");

            migrationBuilder.CreateIndex(
                name: "IX_FileClassification_Name_SearchLevel",
                schema: "files",
                table: "FileClassification",
                columns: new[] { "Name", "SearchLevel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileClassification_Name_SearchLevel",
                schema: "files",
                table: "FileClassification");

            migrationBuilder.CreateIndex(
                name: "IX_FileClassification_Name",
                schema: "files",
                table: "FileClassification",
                column: "Name",
                unique: true);
        }
    }
}
