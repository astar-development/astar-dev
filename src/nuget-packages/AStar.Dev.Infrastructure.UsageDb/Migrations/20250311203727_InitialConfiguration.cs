#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AStar.Dev.Api.Usage.Logger.Migrations
{
    /// <inheritdoc />
    public partial class InitialConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "usage");

            migrationBuilder.CreateTable(
                name: "ApiUsageEvent",
                schema: "usage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ApiEndpoint = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ElapsedMilliseconds = table.Column<long>(type: "bigint", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "DateTime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiUsageEvent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageEvent_ApiName",
                schema: "usage",
                table: "ApiUsageEvent",
                column: "ApiName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiUsageEvent",
                schema: "usage");
        }
    }
}
