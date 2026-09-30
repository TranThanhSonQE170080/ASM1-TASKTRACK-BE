# Render.com deploys this Dockerfile (or use the .NET runtime automatically)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore TaskTrack.API/TaskTrack.API.csproj && \
    dotnet publish TaskTrack.API/TaskTrack.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends postgresql-client && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
COPY TaskManagementDB_Postgres.sql .
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-10000} dotnet TaskTrack.API.dll"]
