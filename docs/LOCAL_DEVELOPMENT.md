# Local Development (without Docker)

Run the full stack natively: PostgreSQL installed on your machine, the ASP.NET Core
API with `dotnet run`, and the React frontend with the Vite dev server. Docker is only
needed for the production-style deployment in `deploy/`.

## Prerequisites

| Tool | Version | Check |
|---|---|---|
| .NET SDK | 10.0 | `dotnet --version` |
| Node.js + npm | 22.x | `node --version` |
| PostgreSQL | 16 (15+ works) | `psql --version` |

## 1. PostgreSQL

Install and start a local server:

```bash
# macOS (Homebrew)
brew install postgresql@16
brew services start postgresql@16

# Ubuntu / Debian
sudo apt install postgresql-16

# Windows: use the official installer from postgresql.org
```

Create the database role and database (matches the default connection string):

```bash
psql -U postgres -c "CREATE USER psikotes WITH PASSWORD 'psikotes';"
psql -U postgres -c "CREATE DATABASE psikotes OWNER psikotes;"
```

Already have a local PostgreSQL with different credentials? Point the API at it by
overriding the connection string when starting the backend (step 2) — no code change
needed:

```bash
ConnectionStrings__Database="Host=localhost;Port=5432;Database=psikotes;Username=myuser;Password=mypassword" \
  dotnet run --project src/Web
```

## 2. Backend

```bash
cd backend
dotnet restore
dotnet run --project src/Web
```

In the `Development` environment the API automatically:

- applies all EF Core migrations on startup (`Database:MigrateOnStartup: true` in
  `appsettings.Development.json` — no `dotnet-ef` tool install required),
- seeds the three roles and a development administrator
  (`admin@psikotes.local` / `Admin#12345` — dev-only credentials, never use them
  outside local development),
- serves Swagger UI at `http://localhost:5000/swagger`.

The API listens on `http://localhost:5000` (see
`src/Web/Properties/launchSettings.json`).

Smoke check: `GET http://localhost:5000/health/live` returns 200.

## 3. Frontend

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173`. The Vite dev server proxies `/api/*` to the backend on
port 5000 (configured in `vite.config.ts`), so no CORS setup is needed. CORS for
`http://localhost:5173` is also pre-allowed in the API config as a fallback.

Log in as the seeded administrator, or register a regular user account from the UI.

## 4. Tests

Both test suites run without Docker — integration tests use SQLite in-memory:

```bash
cd backend && dotnet test
cd frontend && npm test && npm run lint && npm run build
```

## Troubleshooting

| Symptom | Cause / fix |
|---|---|
| Backend fails on startup with a connection error | PostgreSQL not running, wrong port, or role/database missing — recheck step 1 |
| Frontend shows network errors on API calls | Backend not running on port 5000, or you changed `applicationUrl` without updating the proxy target in `vite.config.ts` |
| Port 5000 or 5173 already in use | Change `applicationUrl` in `launchSettings.json` and the proxy target in `vite.config.ts` (backend), or run `npm run dev -- --port 5174` (frontend) |
| Migrations in a bad state after schema experiments | Drop the database (`DROP DATABASE psikotes;`) and restart the API — migrations and seed data are re-applied |
| Login as admin fails | The seed admin is only created when `SeedAdmin:Email`/`SeedAdmin:Password` are set (they are, in `appsettings.Development.json`) and when no user with that email exists yet |

## Notes

- The JWT signing key in `appsettings.Development.json` is a placeholder for local
  development only. Production deployments must supply a real key via the `JWT_KEY`
  environment variable (see `deploy/README.md`).
- The Docker Compose setup in `deploy/` runs the same applications — use it when you
  want a production-like environment instead of a dev loop.
