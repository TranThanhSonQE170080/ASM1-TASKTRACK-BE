# TaskTrack API

ASP.NET Core 8 Web API with PostgreSQL and EF Core. This repository is the backend repository for TaskTrack.

## Local development

1. Create `TaskManagementDB` in PostgreSQL and run `TaskManagementDB_Postgres.sql`.
2. Set the local connection string in the ignored `TaskTrack.API/appsettings.Development.json` or set `ConnectionStrings__DefaultConnection` in your environment.
3. Run `dotnet run --project TaskTrack.API --urls http://localhost:5000`.
4. Open Swagger at `http://localhost:5000/swagger`.

Do not commit database credentials. Production uses the `DATABASE_URL` environment variable.

## Render deployment

Deploy this repository using the included `render.yaml` Blueprint. It creates a free web service and a free PostgreSQL database. Configure this web service environment variable in Render:

- `FrontendUrl`: the deployed Vercel origin (the Blueprint sets `https://asm-1-tasktrack-fe.vercel.app`).
- `ASPNETCORE_ENVIRONMENT`: set to `Production` (the Blueprint sets this by default).

Run `TaskManagementDB_Postgres.sql` against the Render database before using the API. Render's free PostgreSQL database expires after 30 days; upgrade it or choose another hosted PostgreSQL provider for persistent data. The health check is `/api/summary`; Swagger is enabled only in Development.
