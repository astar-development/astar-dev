using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicatesToContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "files",
                table: "Event");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "files",
                table: "Event",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedOn",
                schema: "files",
                table: "Event",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "files",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                schema: "files",
                table: "Event");

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "files",
                table: "Event",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
