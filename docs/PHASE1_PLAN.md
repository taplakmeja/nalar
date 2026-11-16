# Phase 1 Implementation Plan — Foundation, Architecture, Authentication, Database

Status: completed 2025-11-16

## Scope

Deliver the runnable skeleton of the whole system: monorepo, backend solution with clean
layer boundaries, database with migrations, authentication with three roles, frontend
shell with routing and UI kit, Docker Compose with health checks, automated tests, and
documentation. **Excluded:** generation engine internals, question categories, training
modes, analytics, Generator Builder, PWA specifics, ads.

## Implementation Tasks

### Backend

1. Solution `backend/Psikotes.sln` with projects:
   - `Api` — ASP.NET Core Web API host; Serilog; global exception handler; health
     endpoints (`/health/live`, `/health/ready`); Swagger in dev only.
   - `Domain` — entities (`AppUser` profile extensions, `RefreshToken`), enums
     (`UserRole` mapping to Identity roles).
   - `Application` — use cases: auth (register/login/refresh/logout), current-user query.
   - `Infrastructure` — EF Core `AppDbContext` (Npgsql), Identity stores, JWT issuer,
     migrations, role seeding.
   - `QuestionEngine` — **empty project with folder structure + one trivial sample
     generator + determinism test harness**, to lock the boundary and prove the test
     pattern early (no real generators).
2. ASP.NET Core Identity: `ApplicationUser`, roles `User`, `Administrator`,
   `ContentManager`; seeded admin from environment variables on startup.
3. Auth endpoints: register, login (JWT access token + refresh token), refresh, logout,
   `GET /api/me`. Rate limiting on login/register.
4. EF Core migration `InitialCreate` (users, roles, refresh tokens).
5. xUnit tests: auth integration tests (register/login/refresh/role guard), JWT
   validation, QuestionEngine determinism harness demo test.

### Frontend

6. Vite + React + TypeScript scaffold; feature-first folders (`features/auth`,
   `features/dashboard`, `app/`, `lib/`).
7. Tailwind CSS + shadcn/ui fully installed (button, card, input, label, form, dialog,
   dropdown-menu, tabs, toast/sonner, avatar, badge, progress, skeleton, separator,
   sheet, table, alert).
8. React Router: `PublicLayout` (login/register), `AppLayout` (dashboard shell,
   mobile-first bottom nav), `AdminLayout`; route guards by auth state and role.
   No switch-case routing.
9. Auth feature: login, register, auth context (token storage, refresh on 401),
   `GET /api/me` integration.
10. Dashboard placeholder page reading real user data; role-aware admin menu stub.
11. Vitest + React Testing Library: route guard tests, auth form tests, one layout
    smoke test.

### Deployment & Ops

12. `deploy/docker-compose.yml`: `db` (postgres:16, volume, healthcheck), `api`
    (build, depends_on healthy, healthcheck), `frontend` (nginx serving build,
    proxies `/api`); `deploy/.env.example`; CORS pinned to frontend origin.
13. `deploy/README.md`: local dev, VPS deploy, backup (`pg_dump` cron example) and
    restore instructions.
14. Logging: Serilog console + rolling file inside container; request logging.

### Documentation

15. `README.md` (run instructions), `docs/PHASES.md` (phase definitions), ADRs:
    ADR-001 monorepo layout, ADR-002 JWT + Identity, ADR-003 engine boundary,
    ADR-004 script sandbox approach (decision deferred to Phase 6, boundary set now).

## Acceptance Criteria

All must be verifiable by command or test:

1. `docker compose -f deploy/docker-compose.yml up --build` starts db, api, frontend;
   all three health checks pass; frontend loads at `http://localhost:8080`.
2. `dotnet test backend` green, including: registration + login returns valid JWT;
   refresh token rotation works; endpoint with `[Authorize(Roles = "Administrator")]`
   rejects non-admin with 403; same seed + generator version produces identical
   question twice (determinism harness).
3. `npm test` (frontend) green, including: unauthenticated user is redirected from
   `/dashboard` to `/login`; `ContentManager` cannot open `/admin/users`.
4. `dotnet ef migrations list` in `Infrastructure` shows `InitialCreate`; fresh
   database is created and roles + admin user are seeded on API startup.
5. `QuestionEngine` project has **zero references** to `Api`/`Infrastructure`
   (enforced by project reference check in CI or a test).
6. Serilog logs appear in container stdout; unhandled exceptions return a safe 500
   envelope, never stack traces.
7. `PROGRESS.md` Phase 1 checklist fully checked; `docs/reports/phase-1.md` written.
8. No secrets committed; `.env.example` contains placeholders only.

## Definition of Done

- All acceptance criteria pass.
- Automated tests run in CI (GitHub Actions: backend + frontend + compose build).
- Phase 1 completion report in `docs/reports/phase-1.md` with evidence (command output).
