using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations;

/// <inheritdoc />
[SuppressMessage("Style", "IDE0058:Expression value is never used")]
[SuppressMessage("Style", "IDE0053:Use expression body for lambda expression")]
[SuppressMessage("Style", "IDE0022:Use expression body for method")]
public partial class AddDeletionScope : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
                                        name: "DeletionScope",
                                        schema: "files",
                                        table: "DeletionStatus",
                                        type: "int",
                                        nullable: false,
                                        defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
                                    name: "DeletionScope",
                                    schema: "files",
                                    table: "DeletionStatus");
    }
}