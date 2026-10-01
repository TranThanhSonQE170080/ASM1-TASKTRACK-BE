#!/bin/sh
set -eu

if [ -z "${DATABASE_URL:-}" ]; then
  echo "DATABASE_URL is required." >&2
  exit 1
fi

table_count=$(psql "$DATABASE_URL" --no-psqlrc --set=ON_ERROR_STOP=1 --tuples-only --no-align --command="SELECT COUNT(*) FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename IN ('Department', 'Project', 'Task', 'Tag', 'TaskTag');")

case "$table_count" in
  0)
    psql "$DATABASE_URL" --no-psqlrc --set=ON_ERROR_STOP=1 --file=/app/TaskManagementDB_Postgres.sql
    ;;
  5)
    echo "TaskTrack schema already exists; skipping initialization."
    ;;
  *)
    echo "Found $table_count of 5 TaskTrack tables; refusing to run the destructive seed script on a partial schema." >&2
    exit 1
    ;;
esac

export ASPNETCORE_URLS="http://0.0.0.0:${PORT:-10000}"
exec dotnet TaskTrack.API.dll