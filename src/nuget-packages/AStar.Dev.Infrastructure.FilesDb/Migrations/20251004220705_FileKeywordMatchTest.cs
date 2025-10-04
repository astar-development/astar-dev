using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations;

/// <inheritdoc />
[SuppressMessage("Style", "IDE0058:Expression value is never used")]
[SuppressMessage("Style", "IDE0053:Use expression body for lambda expression")]
[SuppressMessage("Style", "IDE0022:Use expression body for method")]
public partial class FileKeywordMatchTest : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FileKeywordMatches",
            schema: "files",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FileName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                Keyword = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FileKeywordMatches", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FileKeywordMatches",
            schema: "files");
    }
}
