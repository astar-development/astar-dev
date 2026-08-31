using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AStarDev.OneDriveSyncClient.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BaseUri = table.Column<string>(type: "TEXT", nullable: false),
                    SearchStringPrefix = table.Column<string>(type: "TEXT", nullable: false),
                    SearchStringSuffix = table.Column<string>(type: "TEXT", nullable: false),
                    TopWallpapers = table.Column<string>(type: "TEXT", nullable: false),
                    Subscriptions = table.Column<string>(type: "TEXT", nullable: false),
                    MaxImagePauseInMilliseconds = table.Column<int>(type: "INTEGER", nullable: false),
                    StartingPageNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPages = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionsStartingPageNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionsTotalPages = table.Column<int>(type: "INTEGER", nullable: false),
                    TopWallpapersStartingPageNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TopWallpapersTotalPages = table.Column<int>(type: "INTEGER", nullable: false),
                    UseHeadless = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SearchConfigurationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    LastKnownImageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPageVisited = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPages = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchCategories_SearchConfigurations_SearchConfigurationId",
                        column: x => x.SearchConfigurationId,
                        principalTable: "SearchConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchCategories_SearchConfigurationId",
                table: "SearchCategories",
                column: "SearchConfigurationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchCategories");

            migrationBuilder.DropTable(
                name: "SearchConfigurations");
        }
    }
}
