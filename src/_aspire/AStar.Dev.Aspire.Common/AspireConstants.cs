namespace AStar.Dev.Aspire.Common;

public static class AspireConstants
{
    public const string Ui        = "ui";

    public static class Apis
    {
        public const string AdminApi  = "admin-api";
        public const string FilesApi  = "files-api";
        public const string ImagesApi = "images-api";
        public const string UsageApi  = "usage-api";
    }

    public static class Sql
    {
        public const string SqlServer = "sql1";
        public const string FilesDb   = "filesDb";
        public const string AdminDb   = "adminDb";
    }

    public static class Services
    {
        public const string AstarMessaging  = "astar-dev-messaging";
        public const string FileMigrations  = "file-migrations";
        public const string DatabaseUpdater = "database-updater";
    }
}
