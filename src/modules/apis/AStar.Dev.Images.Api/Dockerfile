FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/apis/AStar.Dev.Images.Api/AStar.Dev.Images.Api.csproj", "src/apis/AStar.Dev.Images.Api/"]
COPY ["src/nuget-packages/AStar.Dev.AspNet.Extensions/AStar.Dev.AspNet.Extensions.csproj", "src/nuget-packages/AStar.Dev.AspNet.Extensions/"]
COPY ["src/nuget-packages/AStar.Dev.Api.HealthChecks/AStar.Dev.Api.HealthChecks.csproj", "src/nuget-packages/AStar.Dev.Api.HealthChecks/"]
COPY ["src/nuget-packages/AStar.Dev.Api.Usage.Sdk/AStar.Dev.Api.Usage.Sdk.csproj", "src/nuget-packages/AStar.Dev.Api.Usage.Sdk/"]
COPY ["src/nuget-packages/AStar.Dev.Utilities/AStar.Dev.Utilities.csproj", "src/nuget-packages/AStar.Dev.Utilities/"]
COPY ["src/nuget-packages/AStar.Dev.Technical.Debt.Reporting/AStar.Dev.Technical.Debt.Reporting.csproj", "src/nuget-packages/AStar.Dev.Technical.Debt.Reporting/"]
COPY ["src/nuget-packages/AStar.Dev.Auth.Extensions/AStar.Dev.Auth.Extensions.csproj", "src/nuget-packages/AStar.Dev.Auth.Extensions/"]
COPY ["src/nuget-packages/AStar.Dev.Infrastructure.FilesDb/AStar.Dev.Infrastructure.FilesDb.csproj", "src/nuget-packages/AStar.Dev.Infrastructure.FilesDb/"]
COPY ["src/nuget-packages/AStar.Dev.Infrastructure/AStar.Dev.Infrastructure.csproj", "src/nuget-packages/AStar.Dev.Infrastructure/"]
COPY ["src/nuget-packages/AStar.Dev.Logging.Extensions/AStar.Dev.Logging.Extensions.csproj", "src/nuget-packages/AStar.Dev.Logging.Extensions/"]
COPY ["src/nuget-packages/AStar.Dev.Minimal.Api.Extensions/AStar.Dev.Minimal.Api.Extensions.csproj", "src/nuget-packages/AStar.Dev.Minimal.Api.Extensions/"]
COPY ["src/nuget-packages/AStar.Dev.Restful.Root.Document/AStar.Dev.Restful.Root.Document.csproj", "src/nuget-packages/AStar.Dev.Restful.Root.Document/"]
RUN dotnet restore "src/apis/AStar.Dev.Images.Api/AStar.Dev.Images.Api.csproj"
COPY . .
WORKDIR "/src/src/apis/AStar.Dev.Images.Api"
RUN dotnet build "./AStar.Dev.Images.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AStar.Dev.Images.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AStar.Dev.Images.Api.dll"]
