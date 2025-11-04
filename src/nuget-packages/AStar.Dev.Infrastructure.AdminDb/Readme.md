# AStar.Dev.Infrastructure.AdminDb

The AStar.Dev.Infrastructure.AdminDb solution. Please update / add detail.

## GitHub build

[![Build and test solution](https://github.com/astar-development/astar-dev-infrastructure-admindb/actions/workflows/dotnet.yml/badge.svg)](https://github.com/astar-development/astar-dev-infrastructure-admindb/actions/workflows/dotnet.yml)

## SonarCloud Analysis Results

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=jbarden_astar-dev-infrastructure-admindb&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=jbarden_astar-dev-infrastructure-admindb)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=jbarden_astar-dev-infrastructure-admindb&metric=bugs)](https://sonarcloud.io/summary/new_code?id=jbarden_astar-dev-infrastructure-admindb)

[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=jbarden_astar-dev-infrastructure-admindb&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=jbarden_astar-dev-infrastructure-admindb)

[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=jbarden_astar-dev-infrastructure-admindb&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=jbarden_astar-dev-infrastructure-admindb)

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=jbarden_astar-dev-infrastructure-admindb&metric=coverage)](https://sonarcloud.io/summary/new_code?id=jbarden_astar-dev-infrastructure-admindb)

[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=jbarden_astar-dev-infrastructure-admindb&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=jbarden_astar-dev-infrastructure-admindb)

## Additional Information

The DbContext is stored in the main project (along with the various models). To add EF Migrations, the easy option is:

```powershell
dotnet ef migrations add "Description / Migration name"

dotnet ef database update --connection 'Data Source=localhost;Initial Catalog=AdminDb;User ID=sa;Password=<SecurePasswordHere1!>;TrustServerCertificate=True' 

```

OK, the above is pretty obvious if you know EF Core - the reminder that really matters is, for simplicity, the above
needs to be run from within the main project's (AStar.Dev.Infrastructure.AdminDb) directory.

If required, the dotnet ef tools can be added using: ```dotnet tool install --global dotnet-ef```
