using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations;

/// <inheritdoc />
[SuppressMessage("Style", "IDE0058:Expression value is never used")]
[SuppressMessage("Style", "IDE0053:Use expression body for lambda expression")]
[SuppressMessage("Style", "IDE0022:Use expression body for method")]
public partial class MakeDeletionStatusOptional : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_FileDetail_DeletionStatus_DeletionStatusId",
            schema: "files",
            table: "FileDetail");

        migrationBuilder.AlterColumn<int>(
            name: "DeletionStatusId",
            schema: "files",
            table: "FileDetail",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddForeignKey(
            name: "FK_FileDetail_DeletionStatus_DeletionStatusId",
            schema: "files",
            table: "FileDetail",
            column: "DeletionStatusId",
            principalSchema: "files",
            principalTable: "DeletionStatus",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_FileDetail_DeletionStatus_DeletionStatusId",
            schema: "files",
            table: "FileDetail");

        migrationBuilder.AlterColumn<int>(
            name: "DeletionStatusId",
            schema: "files",
            table: "FileDetail",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_FileDetail_DeletionStatus_DeletionStatusId",
            schema: "files",
            table: "FileDetail",
            column: "DeletionStatusId",
            principalSchema: "files",
            principalTable: "DeletionStatus",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
