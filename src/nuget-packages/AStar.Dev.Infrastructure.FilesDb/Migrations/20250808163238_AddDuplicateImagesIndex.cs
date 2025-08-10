using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicateImagesIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileDetail_FileHandle",
                schema: "files",
                table: "FileDetail");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_DuplicateImages",
                schema: "files",
                table: "FileDetail",
                columns: new[] { "IsImage", "FileSize" });

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_FileHandle",
                schema: "files",
                table: "FileDetail",
                column: "FileHandle",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileDetail_DuplicateImages",
                schema: "files",
                table: "FileDetail");

            migrationBuilder.DropIndex(
                name: "IX_FileDetail_FileHandle",
                schema: "files",
                table: "FileDetail");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_FileHandle",
                schema: "files",
                table: "FileDetail",
                column: "FileHandle");
        }
    }
}
