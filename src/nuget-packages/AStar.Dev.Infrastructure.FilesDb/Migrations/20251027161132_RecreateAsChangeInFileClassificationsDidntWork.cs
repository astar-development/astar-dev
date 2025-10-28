using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStar.Dev.Infrastructure.FilesDb.Migrations
{
    /// <inheritdoc />
    public partial class RecreateAsChangeInFileClassificationsDidntWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "files");

            migrationBuilder.CreateTable(
                name: "DeletionStatus",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoftDeleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SoftDeletePending = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    HardDeletePending = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletionScope = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletionStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DirectoryName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Handle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FileLastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAccessDetail",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetailsLastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastViewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoveRequired = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAccessDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileClassification",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SearchLevel = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Celebrity = table.Column<bool>(type: "bit", nullable: false),
                    IncludeInSearch = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileClassification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelToIgnore",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelToIgnore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagToIgnore",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IgnoreImage = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagToIgnore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileDetail",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileAccessDetailId = table.Column<int>(type: "int", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsImage = table.Column<bool>(type: "bit", nullable: false),
                    FileHandle = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    DeletionStatusId = table.Column<int>(type: "int", nullable: true),
                    DirectoryName = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    ImageHeight = table.Column<int>(type: "int", nullable: true),
                    ImageWidth = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileDetail_DeletionStatus_DeletionStatusId",
                        column: x => x.DeletionStatusId,
                        principalSchema: "files",
                        principalTable: "DeletionStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileDetail_FileAccessDetail_FileAccessDetailId",
                        column: x => x.FileAccessDetailId,
                        principalSchema: "files",
                        principalTable: "FileAccessDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileNamePart",
                schema: "files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IncludeInSearch = table.Column<bool>(type: "bit", nullable: false),
                    FileClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileNamePart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileNamePart_FileClassification_FileClassificationId",
                        column: x => x.FileClassificationId,
                        principalSchema: "files",
                        principalTable: "FileClassification",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileDetailClassifications",
                schema: "files",
                columns: table => new
                {
                    FileDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDetailClassifications", x => new { x.FileDetailId, x.FileClassificationId });
                    table.ForeignKey(
                        name: "FK_FileDetailClassifications_FileClassification_FileClassificationId",
                        column: x => x.FileClassificationId,
                        principalSchema: "files",
                        principalTable: "FileClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileDetailClassifications_FileDetail_FileDetailId",
                        column: x => x.FileDetailId,
                        principalSchema: "files",
                        principalTable: "FileDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileClassification_Name_SearchLevel",
                schema: "files",
                table: "FileClassification",
                columns: new[] { "Name", "SearchLevel" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_DeletionStatusId",
                schema: "files",
                table: "FileDetail",
                column: "DeletionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_DuplicateImages",
                schema: "files",
                table: "FileDetail",
                columns: new[] { "IsImage", "FileSize" });

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_FileAccessDetailId",
                schema: "files",
                table: "FileDetail",
                column: "FileAccessDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_FileHandle",
                schema: "files",
                table: "FileDetail",
                column: "FileHandle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileDetail_FileSize",
                schema: "files",
                table: "FileDetail",
                column: "FileSize");

            migrationBuilder.CreateIndex(
                name: "IX_FileDetailClassifications_FileClassificationId",
                schema: "files",
                table: "FileDetailClassifications",
                column: "FileClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_FileNamePart_FileClassificationId",
                schema: "files",
                table: "FileNamePart",
                column: "FileClassificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileDetailClassifications",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileNamePart",
                schema: "files");

            migrationBuilder.DropTable(
                name: "ModelToIgnore",
                schema: "files");

            migrationBuilder.DropTable(
                name: "TagToIgnore",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileDetail",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileClassification",
                schema: "files");

            migrationBuilder.DropTable(
                name: "DeletionStatus",
                schema: "files");

            migrationBuilder.DropTable(
                name: "FileAccessDetail",
                schema: "files");
        }
    }
}
