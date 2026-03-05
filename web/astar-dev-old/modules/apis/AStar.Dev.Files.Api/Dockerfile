FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/apis/AStar.Dev.Files.Api/AStar.Dev.Files.Api.csproj", "src/apis/AStar.Dev.Files.Api/"]
COPY ["src/libs/AStar.Dev.Api.Usage.Sdk/AStar.Dev.Api.Usage.Sdk.csproj", "src/libs/AStar.Dev.Api.Usage.Sdk/"]
COPY ["src/libs/AStar.Dev.Utilities/AStar.Dev.Utilities.csproj", "src/libs/AStar.Dev.Utilities/"]
COPY ["src/libs/AStar.Dev.AspNet.Extensions/AStar.Dev.AspNet.Extensions.csproj", "src/libs/AStar.Dev.AspNet.Extensions/"]
COPY ["src/libs/AStar.Dev.Api.HealthChecks/AStar.Dev.Api.HealthChecks.csproj", "src/libs/AStar.Dev.Api.HealthChecks/"]
COPY ["src/libs/AStar.Dev.Technical.Debt.Reporting/AStar.Dev.Technical.Debt.Reporting.csproj", "src/libs/AStar.Dev.Technical.Debt.Reporting/"]
COPY ["src/libs/AStar.Dev.Auth.Extensions/AStar.Dev.Auth.Extensions.csproj", "src/libs/AStar.Dev.Auth.Extensions/"]
COPY ["src/libs/AStar.Dev.Infrastructure.FilesDb/AStar.Dev.Infrastructure.FilesDb.csproj", "src/libs/AStar.Dev.Infrastructure.FilesDb/"]
COPY ["src/libs/AStar.Dev.Infrastructure/AStar.Dev.Infrastructure.csproj", "src/libs/AStar.Dev.Infrastructure/"]
COPY ["src/libs/AStar.Dev.Logging.Extensions/AStar.Dev.Logging.Extensions.csproj", "src/libs/AStar.Dev.Logging.Extensions/"]
COPY ["src/libs/AStar.Dev.Minimal.Api.Extensions/AStar.Dev.Minimal.Api.Extensions.csproj", "src/libs/AStar.Dev.Minimal.Api.Extensions/"]
RUN dotnet restore "src/apis/AStar.Dev.Files.Api/AStar.Dev.Files.Api.csproj"
COPY . .
WORKDIR "/src/src/apis/AStar.Dev.Files.Api"
RUN dotnet build "./AStar.Dev.Files.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AStar.Dev.Files.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AStar.Dev.Files.Api.dll"]
