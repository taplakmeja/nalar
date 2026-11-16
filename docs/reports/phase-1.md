# Phase 1 Completion Report — Foundation, Architecture, Authentication, Database

Date: 2025-11-16 · Stack: .NET 10 (per user decision), Vite/React/TS

## Scope Delivered

- **Monorepo**: `backend/` (5 projects), `frontend/`, `deploy/`, `docs/`, CI.
- **Backend clean architecture**: `Domain` (entities, roles) ← `Application`
  (auth use case, interfaces, DTOs) ← `Infrastructure` (EF Core Npgsql,
  Identity, JWT issuer, migrations, seeder) ← `Api` (controllers, middleware).
  `QuestionEngine` is fully independent (zero references to Api/Infrastructure;
  enforced in CI).
- **Auth**: register / login / refresh (rotating) / logout, `GET /api/me`,
  JWT bearer with raw claim mapping, 3 Identity roles, rate limiting on auth
  endpoints (20 req/min/IP), seed admin on startup.
- **Database**: PostgreSQL via Npgsql; `InitialCreate` migration checked in;
  optional `Database:MigrateOnStartup` (enabled in compose deployment).
- **QuestionEngine skeleton**: category/difficulty enums, `GeneratedQuestion`
  contract (generator id, version, seed, parameters, answer, options,
  validation), `SeededRandom`, sample `ArithmeticAdditionGenerator`,
  `GeneratorRegistry`, `DeterminismHarness`.
- **Frontend**: Vite + React + TS, Tailwind 3.4, shadcn/ui — all 53 components
  installed; React Router (`createBrowserRouter`, no switch-case routing);
  layouts (public + app shell with mobile bottom nav); login/register pages;
  `AuthProvider` with token storage + refresh-on-401; `RequireAuth`/`RequireRole`
  guards; dashboard + admin stubs (Indonesian UI).
- **Deployment**: multi-stage Dockerfiles (backend, frontend+nginx),
  `deploy/docker-compose.yml` (db with healthcheck + volume, api with healthcheck
  + auto-migrate + seeding, frontend), `.env.example`, deploy guide with
  backup/restore instructions.
- **CI**: GitHub Actions — backend build/test + engine boundary check, frontend
  test/build, compose config validation.

## Verification Evidence

- `dotnet build` → Build succeeded (0 errors).
- `dotnet test` → **17/17 passed**
  - `Psikotes.QuestionEngine.Tests`: 9 passed (determinism, unique options,
    difficulty scaling, bulk harness).
  - `Psikotes.Api.IntegrationTests`: 8 passed on SQLite in-memory via
    `WebApplicationFactory` (register/login, /api/me, 401 without token,
    403 non-admin on /api/admin/ping, refresh rotation + reuse rejection,
    duplicate email 400, health endpoints 200).
- `npm run build` → tsc + vite build succeeded.
- `npm test` → **6/6 passed** (auth guard redirects, role allow/block,
  login success + error paths).

## Issues Found & Fixed During Verification

1. `AddIdentity` set cookie as default authenticate scheme → `[Authorize]`
   redirected to `/Account/Login` instead of challenging JWT. Fixed by
   explicitly overriding default schemes with JwtBearer.
2. Inbound claim mapping renamed `sub`/`email` claims → `/api/me` returned
   empty values. Fixed with `MapInboundClaims = false`.
3. `RequireRole` redirected before the user profile finished loading (found by
   frontend tests). Fixed by awaiting `isLoading` before deciding.
4. `react-resizable-panels` v4 API broke shadcn's `resizable.tsx` → pinned 2.1.7.

## Acceptance Criteria Status

| # | Criterion | Status |
|---|---|---|
| 1 | Compose up with healthy services | ⚠️ Files complete; **not executed** — Docker daemon unavailable in sandbox. Validate on first deploy. |
| 2 | `dotnet test` green incl. JWT, refresh rotation, role guard 403, determinism | ✅ 17/17 |
| 3 | Frontend tests green incl. redirect + role-block | ✅ 6/6 |
| 4 | `InitialCreate` migration exists; fresh DB + roles + admin seeded | ✅ migration checked in; startup seeding wired (runs in compose deployment) |
| 5 | QuestionEngine has zero refs to Api/Infrastructure | ✅ verified in CI step |
| 6 | Serilog logs; safe 500 envelope | ✅ implemented |
| 7 | PROGRESS.md checked; this report | ✅ |
| 8 | No secrets committed | ✅ `.env.example` placeholders only; dev keys marked CHANGE_ME |

## Notes for Phase 2

- Engine contracts are stable enough to build real generators against.
- `GeneratedQuestion.Parameters` is string-valued; consider typed JSON in Phase 2
  if generators need structured parameters.
- Integration tests currently use SQLite; consider a Testcontainers PostgreSQL
  job in CI when compose verification becomes a gate.

## Addendum — Clean Architecture Refactor (2025-11-16)

Per user decision, the backend was refactored to Jason Taylor-style Clean
Architecture with the Mediator pattern (`martinothamar/Mediator`), Ardalis.Result,
Ardalis.GuardClauses, and FluentValidation. Agent skills (superpowers set,
frontend-design, .NET standards) were installed at `.claude/skills/`.

### Structural changes

- `src/Api` renamed to `src/Web` (assembly `Psikotes.Web`); Mediator source
  generator referenced only there (edge project rule), `Mediator.Abstractions`
  in Application.
- `AuthService` deleted and decomposed into use cases under
  `Application/UseCases/Auth/`: `RegisterCommand`, `LoginCommand`,
  `RefreshCommand`, `LogoutCommand` — each with handler + FluentValidation
  validator; shared token issuance extracted to `AuthResponseFactory`.
  `AuthException` removed; expected outcomes are now `Ardalis.Result` values
  (`Invalid` → 400, `Unauthorized` → 401, `NoContent` → 204) mapped via
  `ToActionResult(this)`.
- Pipeline behaviors added in `Application/Common/Behaviors/`:
  `LoggingBehavior`, `PerformanceBehavior`, `ValidationBehavior` (short-circuits
  Result-typed messages to `Result.Invalid`); registered via `AddMediator`
  options in `Web/Program.cs` with `ServiceLifetime.Scoped` (handlers use the
  scoped DbContext).
- `RefreshToken` entity gained guarded `Revoke`/`SetReplacedByToken` methods;
  handlers use `Guard.Against.*` preconditions.
- Test project renamed to `Psikotes.Web.IntegrationTests`; new
  `Psikotes.Application.UnitTests` covers all validators; placeholder
  `UnitTest1.cs` files removed. API surface unchanged.

### Verification

- `dotnet build` → 0 errors.
- `dotnet test` → **36/36 passed** (8 QuestionEngine, 21 Application validator
  units, 7 Web integration on SQLite in-memory).

### Mediator integration notes

- `AddMediator` (and `MediatorOptions`) are generated in the edge project, so
  the `AddMediator(options => ...)` call lives in `Web/Program.cs`; behaviors
  are registered via `options.PipelineBehaviors` (AOT-safe, per library docs).
