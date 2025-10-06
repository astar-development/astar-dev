using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations;

/// <inheritdoc />
[SuppressMessage("Style", "IDE0058:Expression value is never used")]
[SuppressMessage("Style", "IDE0053:Use expression body for lambda expression")]
[SuppressMessage("Style", "IDE0022:Use expression body for method")]
[SuppressMessage("Style", "CA1861:Use expression body for method")]
public partial class UpdateDatabaseStructorForClassifications : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FileKeywordMatches",
            schema: "files");

        migrationBuilder.AlterColumn<DateTime>(
            name: "DetailsLastUpdated",
            schema: "files",
            table: "FileAccessDetail",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
            oldClrType: typeof(DateTime),
            oldType: "datetime2",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "DetailsLastUpdated",
            schema: "files",
            table: "FileAccessDetail",
            type: "datetime2",
            nullable: true,
            oldClrType: typeof(DateTime),
            oldType: "datetime2");

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
}
