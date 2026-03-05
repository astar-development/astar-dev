-- Example script for creating a new Login and dB User with only SELECT and INSERT permissions on the admin schema
IF
NOT EXISTS (SELECT 1
               FROM sys.server_principals
               WHERE name = 'Admin')
BEGIN
        CREATE
LOGIN Admin WITH PASSWORD = '<enterStrongPasswordHere>';
        CREATE
USER Admin FOR LOGIN AdminDbUser WITH DEFAULT_SCHEMA = admin;
        USE
AdminDb;
GRANT SELECT, INSERT TO Admin;
END