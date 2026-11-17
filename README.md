# Nalar — Latihan Psikotes Indonesia

A mobile-first Indonesian psychotest training platform. Every question is generated
programmatically by a deterministic question-generation engine — no question bank, no
external AI services. Free to use; monetized only through a provider-independent,
consent-gated advertising module (disabled by default).

> Generated practice tests are **not** official examination questions, and readiness
> estimates are indicative only.

## Stack

- **Frontend:** React + TypeScript (Vite), Tailwind CSS, shadcn/ui, React Router
- **Backend:** ASP.NET Core (.NET 10) Web API, C#
- **Database:** PostgreSQL + Entity Framework Core
- **Auth:** ASP.NET Core Identity + JWT (roles: User, Administrator, ContentManager)
- **Deployment:** Docker Compose
- **App type:** PWA (Phase 7)

## Quick Start (Docker Compose)

```bash
cd deploy
cp .env.example .env      # then edit: passwords, JWT key, seed admin
docker compose up -d --build
# App:  http://localhost:8080
# API:  http://localhost:8080/api (Swagger in dev)
```

See [deploy/README.md](deploy/README.md) for VPS/cloud notes and database
backup/restore instructions.

## Quick Start (Local, without Docker)

Requires .NET 10 SDK, Node 22, and a local PostgreSQL:

```bash
# 1. PostgreSQL: create the role and database
psql -U postgres -c "CREATE USER psikotes WITH PASSWORD 'psikotes';"
psql -U postgres -c "CREATE DATABASE psikotes OWNER psikotes;"

# 2. Backend on http://localhost:5000 (migrates + seeds on startup)
cd backend && dotnet run --project src/Web

# 3. Frontend on http://localhost:5173 (proxies /api to the backend)
cd frontend && npm install && npm run dev
```

Full instructions, credential overrides, and troubleshooting:
[docs/LOCAL_DEVELOPMENT.md](docs/LOCAL_DEVELOPMENT.md).

## Repository Layout

```
backend/
  src/Web/             ASP.NET Core host: controllers, middleware, Mediator wiring
  src/Application/     Use cases (Mediator commands/queries, validators, behaviors)
  src/Domain/          Entities and enums
  src/Infrastructure/  EF Core, Identity, JWT, migrations, seeding
  src/QuestionEngine/  Deterministic question generation (independent project)
  tests/               xUnit unit + integration + engine tests
frontend/              React SPA (features-first), shadcn/ui components
deploy/                docker-compose.yml, .env.example, deployment guide
.claude/               agent skills (superpowers, frontend-design, .NET standards)
docs/                  Architecture, phase plans, reports
```

## Development

```bash
# backend
cd backend && dotnet test

# frontend
cd frontend && npm install && npm test && npm run build
```

## Roadmap

Phases are tracked in [PROGRESS.md](PROGRESS.md) and defined in
[docs/PHASES.md](docs/PHASES.md). All eight phases are complete.
