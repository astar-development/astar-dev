CREATE
LOGIN AdminDbUser WITH PASSWORD = '<enterStrongPasswordHere>';
GO
-- Create the user
CREATE
USER AdminDbUser FOR LOGIN AdminDbUser;

-- Grant read (SELECT) and update (UPDATE) permissions on the specific database
USE
AdminDb;
GRANT SELECT, INSERT TO AdminDbUser;
