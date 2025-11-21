namespace AStar.Dev.Web.Aspire.Common;

public static class AspireConstants
{
    public const string Ui = "ui";

    public static class Apis
    {
        public const string AdminApi  = "admin-api";
        public const string FilesApi  = "files-api";
        public const string FileClassificationsApi = "file-classifications-api";
        public const string ImagesApi = "images-api";
        public const string UsageApi  = "usage-api";
    }

    public static class Sql
    {
        public const string SqlServer                  = "sql1";
        public const string SqlSaUserPasswordParameter = "sa-user-password";
        public const string SqlFilesUserPasswordParameter = "files-user-password";
        public const string SqlAdminUserPasswordParameter = "admin-user-password";
        public const string SqlUsageUserPasswordParameter = "usage-user-password";
        public const string AStarDb                    = "astar-db";
    }

    public static class Services
    {
        public const string AstarMessaging  = "astar-dev-messaging";
        public const string FileMigrations  = "file-migrations";
        public const string DatabaseUpdater = "database-updater";
    }
}
