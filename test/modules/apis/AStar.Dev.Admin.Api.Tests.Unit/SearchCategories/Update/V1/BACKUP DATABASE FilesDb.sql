BACKUP
DATABASE FilesDb
    TO DISK = '/home/jason-barden/SQL-Backups/FilesDb.bak'
    WITH FORMAT,
    MEDIANAME = 'SQLServerBackups',
    NAME = 'Full Backup of Filesdb';