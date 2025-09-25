using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations;

/// <inheritdoc />
[SuppressMessage("Style", "IDE0058:Expression value is never used")]
[SuppressMessage("Style", "IDE0053:Use expression body for lambda expression")]
public partial class AddAuditableEntityWhereRequired : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
                                      name: "EventOccurredAt",
                                      schema: "files",
                                      table: "Event",
                                      newName: "UpdatedOn");

        migrationBuilder.AddColumn<string>(
                                           name: "UpdatedBy",
                                           schema: "files",
                                           table: "TagToIgnore",
                                           type: "nvarchar(max)",
                                           nullable: false,
                                           defaultValue: "");

        migrationBuilder.AddColumn<DateTimeOffset>(
                                                   name: "UpdatedOn",
                                                   schema: "files",
                                                   table: "TagToIgnore",
                                                   type: "datetimeoffset",
                                                   nullable: false,
                                                   defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<string>(
                                           name: "UpdatedBy",
                                           schema: "files",
                                           table: "ModelToIgnore",
                                           type: "nvarchar(max)",
                                           nullable: false,
                                           defaultValue: "");

        migrationBuilder.AddColumn<DateTimeOffset>(
                                                   name: "UpdatedOn",
                                                   schema: "files",
                                                   table: "ModelToIgnore",
                                                   type: "datetimeoffset",
                                                   nullable: false,
                                                   defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<string>(
                                           name: "UpdatedBy",
                                           schema: "files",
                                           table: "FileDetail",
                                           type: "nvarchar(max)",
                                           nullable: false,
                                           defaultValue: "");

        migrationBuilder.AddColumn<DateTimeOffset>(
                                                   name: "UpdatedDate",
                                                   schema: "files",
                                                   table: "FileDetail",
                                                   type: "datetimeoffset",
                                                   nullable: false,
                                                   defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<DateTimeOffset>(
                                                   name: "UpdatedOn",
                                                   schema: "files",
                                                   table: "FileDetail",
                                                   type: "datetimeoffset",
                                                   nullable: false,
                                                   defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<string>(
                                           name: "UpdatedBy",
                                           schema: "files",
                                           table: "FileAccessDetail",
                                           type: "nvarchar(max)",
                                           nullable: false,
                                           defaultValue: "");

        migrationBuilder.AddColumn<DateTimeOffset>(
                                                   name: "UpdatedOn",
                                                   schema: "files",
                                                   table: "FileAccessDetail",
                                                   type: "datetimeoffset",
                                                   nullable: false,
                                                   defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.AddColumn<string>(
                                           name: "UpdatedBy",
                                           schema: "files",
                                           table: "DeletionStatus",
                                           type: "nvarchar(max)",
                                           nullable: false,
                                           defaultValue: "");

        migrationBuilder.AddColumn<DateTimeOffset>(
                                                   name: "UpdatedOn",
                                                   schema: "files",
                                                   table: "DeletionStatus",
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
                                    table: "TagToIgnore");

        migrationBuilder.DropColumn(
                                    name: "UpdatedOn",
                                    schema: "files",
                                    table: "TagToIgnore");

        migrationBuilder.DropColumn(
                                    name: "UpdatedBy",
                                    schema: "files",
                                    table: "ModelToIgnore");

        migrationBuilder.DropColumn(
                                    name: "UpdatedOn",
                                    schema: "files",
                                    table: "ModelToIgnore");

        migrationBuilder.DropColumn(
                                    name: "UpdatedBy",
                                    schema: "files",
                                    table: "FileDetail");

        migrationBuilder.DropColumn(
                                    name: "UpdatedDate",
                                    schema: "files",
                                    table: "FileDetail");

        migrationBuilder.DropColumn(
                                    name: "UpdatedOn",
                                    schema: "files",
                                    table: "FileDetail");

        migrationBuilder.DropColumn(
                                    name: "UpdatedBy",
                                    schema: "files",
                                    table: "FileAccessDetail");

        migrationBuilder.DropColumn(
                                    name: "UpdatedOn",
                                    schema: "files",
                                    table: "FileAccessDetail");

        migrationBuilder.DropColumn(
                                    name: "UpdatedBy",
                                    schema: "files",
                                    table: "DeletionStatus");

        migrationBuilder.DropColumn(
                                    name: "UpdatedOn",
                                    schema: "files",
                                    table: "DeletionStatus");

        migrationBuilder.RenameColumn(
                                      name: "UpdatedOn",
                                      schema: "files",
                                      table: "Event",
                                      newName: "EventOccurredAt");
    }
}