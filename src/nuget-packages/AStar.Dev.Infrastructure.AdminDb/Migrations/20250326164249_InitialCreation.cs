#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace AStar.Dev.Infrastructure.AdminDb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.EnsureSchema(
                name: "admin");

            _ = migrationBuilder.CreateTable(
                name: "ScrapeDirectory",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScrapeDirectoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RootDirectory = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BaseSaveDirectory = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BaseDirectory = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BaseDirectoryFamous = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SubDirectoryName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_ScrapeDirectory", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "SearchCategory",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SearchCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LastKnownImageCount = table.Column<int>(type: "int", nullable: false),
                    LastPageVisited = table.Column<int>(type: "int", nullable: false),
                    TotalPages = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_SearchCategory", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "SearchConfiguration",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SearchString = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TopWallpapers = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SearchStringPrefix = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SearchStringSuffix = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Subscriptions = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ImagePauseInSeconds = table.Column<int>(type: "int", nullable: false),
                    StartingPageNumber = table.Column<int>(type: "int", nullable: false),
                    TotalPages = table.Column<int>(type: "int", nullable: false),
                    UseHeadless = table.Column<bool>(type: "bit", nullable: false),
                    SubscriptionsStartingPageNumber = table.Column<int>(type: "int", nullable: false),
                    SubscriptionsTotalPages = table.Column<int>(type: "int", nullable: false),
                    TopWallpapersTotalPages = table.Column<int>(type: "int", nullable: false),
                    TopWallpapersStartingPageNumber = table.Column<int>(type: "int", nullable: false),
                    SlowMotionDelayInMilliseconds = table.Column<int>(type: "int", nullable: false),
                    SiteConfigurationSlug = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_SearchConfiguration", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "SiteConfiguration",
                schema: "admin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteConfigurationSlug = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LoginUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    LoginEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    _ = table.PrimaryKey("PK_SiteConfiguration", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropTable(
                name: "ScrapeDirectory",
                schema: "admin");

            _ = migrationBuilder.DropTable(
                name: "SearchCategory",
                schema: "admin");

            _ = migrationBuilder.DropTable(
                name: "SearchConfiguration",
                schema: "admin");

            _ = migrationBuilder.DropTable(
                name: "SiteConfiguration",
                schema: "admin");
        }
    }
}
