# Render.com deploys this Dockerfile (or use the .NET runtime automatically)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore TaskTrack.API/TaskTrack.API.csproj && \
    dotnet publish TaskTrack.API/TaskTrack.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "TaskTrack.API.dll"]
