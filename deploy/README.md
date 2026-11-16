# Deployment Guide

## Local Development

### Backend (without Docker)

Requires .NET 10 SDK and a reachable PostgreSQL instance.

```bash
cd backend
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api
# API on http://localhost:5000 (see launchSettings), Swagger at /swagger
```

### Frontend (dev server)

```bash
cd frontend
npm install
npm run dev
# http://localhost:5173 — /api is proxied to http://localhost:5000
```

## Docker Compose (local, VPS, or cloud VM)

```bash
cd deploy
cp .env.example .env
# edit .env: set POSTGRES_PASSWORD, JWT_KEY (openssl rand -base64 48), SEED_ADMIN_*
docker compose up -d --build
docker compose ps          # all services healthy
docker compose logs -f api # watch migrations + seeding
```

The frontend is served at `http://localhost:8080` (change `FRONTEND_PORT` in `.env`).
On first API startup, migrations run automatically and the seed admin account is
created when `SEED_ADMIN_EMAIL`/`SEED_ADMIN_PASSWORD` are set.

### VPS notes

- Put a TLS-terminating reverse proxy (Caddy/Nginx/Traefik) in front of the
  `frontend` container; keep all containers on an internal network.
- Update `FRONTEND_URL` to the public origin so API CORS matches.
- Restrict access to the Postgres port — it is not published by default.

## Database Backup & Recovery

### Backup

```bash
cd deploy
docker compose exec -T db pg_dump -U psikotes psikotes > backup-$(date +%F).sql
```

Recommended: schedule a daily cron job on the host, e.g.

```cron
0 3 * * * cd /opt/nalar/deploy && docker compose exec -T db pg_dump -U psikotes psikotes | gzip > /var/backups/nalar/nalar-$(date +\%F).sql.gz
```

### Restore

```bash
cd deploy
docker compose exec -T db psql -U psikotes -d psikotes < backup-2025-01-01.sql
```

### Data persistence

PostgreSQL data lives in the named volume `db-data`. It survives container
rebuilds; it does **not** survive `docker compose down -v` (which deletes
volumes — use with care).
