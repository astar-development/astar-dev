using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class RestructureDeletionAndImageDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "HardDeletePending",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropColumn(
                name: "ImageHeight",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropColumn(
                name: "ImageWidth",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropColumn(
                name: "SoftDeletePending",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropColumn(
                name: "SoftDeleted",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropColumn(
                name: "HardDeletePending",
                schema: "files",
                table: "FileAccessDetail");

            _ = migrationBuilder.DropColumn(
                name: "HardDeleted",
                schema: "files",
                table: "FileAccessDetail");

            _ = migrationBuilder.DropColumn(
                name: "SoftDeletePending",
                schema: "files",
                table: "FileAccessDetail");

            _ = migrationBuilder.DropColumn(
                name: "SoftDeleted",
                schema: "files",
                table: "FileAccessDetail");

            _ = migrationBuilder.AddColumn<int>(
                name: "DeletionStatusId",
                schema: "files",
                table: "FileDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.AddColumn<int>(
                name: "ImageDetailId",
                schema: "files",
                table: "FileDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            _ = migrationBuilder.CreateTable(
                name: "DeletionStatus",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoftDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SoftDeletePending = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    HardDeletePending = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_DeletionStatus", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "ImageDetail",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageWidth = table.Column<int>(type: "int", nullable: true),
                    ImageHeight = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ImageDetail", x => x.Id);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_FileDetail_DeletionStatusId",
                schema: "files",
                table: "FileDetail",
                column: "DeletionStatusId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_FileDetail_ImageDetailId",
                schema: "files",
                table: "FileDetail",
                column: "ImageDetailId");

            _ = migrationBuilder.AddForeignKey(
                name: "FK_FileDetail_DeletionStatus_DeletionStatusId",
                schema: "files",
                table: "FileDetail",
                column: "DeletionStatusId",
                principalSchema: "files",
                principalTable: "DeletionStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            _ = migrationBuilder.AddForeignKey(
                name: "FK_FileDetail_ImageDetail_ImageDetailId",
                schema: "files",
                table: "FileDetail",
                column: "ImageDetailId",
                principalSchema: "files",
                principalTable: "ImageDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropForeignKey(
                name: "FK_FileDetail_DeletionStatus_DeletionStatusId",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropForeignKey(
                name: "FK_FileDetail_ImageDetail_ImageDetailId",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropTable(
                name: "DeletionStatus",
                schema: "files");

            _ = migrationBuilder.DropTable(
                name: "ImageDetail",
                schema: "files");

            _ = migrationBuilder.DropIndex(
                name: "IX_FileDetail_DeletionStatusId",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropIndex(
                name: "IX_FileDetail_ImageDetailId",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropColumn(
                name: "DeletionStatusId",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.DropColumn(
                name: "ImageDetailId",
                schema: "files",
                table: "FileDetail");

            _ = migrationBuilder.AddColumn<DateTimeOffset>(
                name: "HardDeletePending",
                schema: "files",
                table: "FileDetail",
                type: "datetimeoffset",
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "ImageHeight",
                schema: "files",
                table: "FileDetail",
                type: "int",
                nullable: true);

            _ = migrationBuilder.AddColumn<int>(
                name: "ImageWidth",
                schema: "files",
                table: "FileDetail",
                type: "int",
                nullable: true);

            _ = migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SoftDeletePending",
                schema: "files",
                table: "FileDetail",
                type: "datetimeoffset",
                nullable: true);

            _ = migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SoftDeleted",
                schema: "files",
                table: "FileDetail",
                type: "datetimeoffset",
                nullable: true);

            _ = migrationBuilder.AddColumn<bool>(
                name: "HardDeletePending",
                schema: "files",
                table: "FileAccessDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<bool>(
                name: "HardDeleted",
                schema: "files",
                table: "FileAccessDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<bool>(
                name: "SoftDeletePending",
                schema: "files",
                table: "FileAccessDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            _ = migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                schema: "files",
                table: "FileAccessDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
