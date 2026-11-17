# PROGRESS.md

Single source of truth for project progress. Update after every completed task.
Phases are defined in `docs/PHASES.md`; plans in `docs/PHASE1_PLAN.md` etc.

Status legend: ⬜ not started · 🟨 in progress · ✅ done · ⛔ blocked

## Phase Overview

| # | Phase | Status | Report |
|---|---|---|---|
| 0 | Assignment: inspection, evaluation, Phase 1 plan | ✅ | `docs/reports/phase-0.md` |
| 1 | Foundation, architecture, authentication, database | ✅ | `docs/reports/phase-1.md` |
| 2 | Question Generation Engine | ✅ | `docs/reports/phase-2.md` |
| 3 | Six question categories & visual rendering | ✅ | `docs/reports/phase-3.md` |
| 4 | Training & examination system | ✅ | `docs/reports/phase-4.md` |
| 5 | Analytics & adaptive difficulty | ✅ | `docs/reports/phase-5.md` |
| 6 | Administration & Generator Builder | ⬜ | — |
| 7 | PWA & advertising | ⬜ | — |
| 8 | Production deployment & optimization | ⬜ | — |

## Phase 0 — Assignment (completed)

- [x] Inspect workspace/repository → **empty; greenfield build** (no existing code to migrate)
- [x] Create `AGENTS.md` (project rules for agents/contributors)
- [x] Create documentation set and this progress file
- [x] Identify technology & architecture baseline → see `docs/ARCHITECTURE_EVALUATION.md`
- [x] Evaluate whether existing code supports the proposed architecture → N/A (greenfield);
      proposed architecture validated against requirements instead
- [x] Identify architectural risks & missing requirements → `docs/ARCHITECTURE_EVALUATION.md`
- [x] Produce Phase 1 implementation plan → `docs/PHASE1_PLAN.md`
- [x] Define Phase 1 acceptance criteria → `docs/PHASE1_PLAN.md`

## Phase 1 — Foundation (completed 2025-11-16)

Scope, tasks and acceptance criteria: see `docs/PHASE1_PLAN.md`.

- [x] Monorepo skeleton (`backend/`, `frontend/`, `deploy/`, `docs/`)
- [x] Backend solution: Api / Application / Domain / Infrastructure / QuestionEngine projects
- [x] EF Core + Npgsql wired, initial migration, PostgreSQL in Docker
- [x] ASP.NET Core Identity: register/login/refresh, 3 roles seeded
- [x] JWT authentication + role-based authorization
- [x] Health checks, Serilog logging, global error handling
- [x] Frontend: Vite + React + TS, Tailwind, shadcn/ui (53 components) installed, React Router shell
- [x] Auth pages (login/register), protected routes, role guards
- [x] Docker Compose: api + db + frontend, health checks, persistent volume
- [x] Automated tests (xUnit 17/17 + Vitest 6/6) wired and green
- [x] Phase 1 completion report
- [x] Clean Architecture refactor (Jason Taylor style): `Api` → `Web`, `AuthService`
      decomposed into Mediator commands/queries (`UseCases/Auth/...`) with
      FluentValidation validators, Ardalis `Result<T>` outcomes, GuardClauses,
      and Logging/Performance/Validation pipeline behaviors
- [x] Agent skills installed at `.claude/skills/` (superpowers, frontend-design,
      .NET standards)
- [ ] ⚠️ `docker compose up` end-to-end run — blocked in sandbox (no Docker daemon); validate on first deploy

## Phase 2 — Question Generation Engine (completed 2025-11-16)

Scope, tasks and acceptance criteria: see `docs/PHASE2_PLAN.md`; report:
`docs/reports/phase-2.md`.

- [x] Engine core abstractions (`GeneratorIdentity`, `IQuestionGenerator`,
      `GeneratorRegistry`, `GeneratedQuestion`)
- [x] Deterministic `SeededRandom`; same seed + version ⇒ same question
- [x] Distractor generation with uniqueness guarantees
- [x] QA layer 1 `TechnicalValidator` + QA layer 2 `MassTester`
      (mass generation, duplicate detection)
- [x] 49 engine tests green

## Phase 3 — Six Categories & Visual Rendering (completed 2025-11-16)

Scope, tasks and acceptance criteria: see `docs/PHASE3_PLAN.md`; report:
`docs/reports/phase-3.md`.

- [x] 18 generators across Numerical, Verbal, Logical, Abstract, Spatial,
      Kraepelin, each with 5 difficulty levels and auto answers/distractors
- [x] Versioned Indonesian verbal dataset (`verbal-id-v1.json`)
- [x] Programmatic SVG rendering for abstract & spatial questions
- [x] Per-generator determinism + validity sweep tests green

## Phase 4 — Training & Examination System (completed 2025-11-16)

Scope, tasks and acceptance criteria: see `docs/PHASE4_PLAN.md`; report:
`docs/reports/phase-4.md`.

- [x] Domain: `TrainingSession`, `SessionQuestion` (metadata + answer key only),
      `ExamProfile`, mode/status/scope enums
- [x] Application: StartSession, GetSessionQuestion (regenerate from metadata),
      SubmitAnswer (server-side validation, concise feedback), FinishSession,
      GetSessionResult, ListExamProfiles + validators
- [x] `ScoringService`: weighted / percentage / survival scoring, pass thresholds
- [x] Migration `AddTrainingSessions`; 6 seeded exam profiles
- [x] Web: `SessionsController`, `ExamProfilesController`, `CurrentUserService`
- [x] Frontend: ModeSelectPage, SessionRunnerPage (countdown, navigation,
      review marking, auto-submit), SessionResultPage; routes wired
- [x] Tests: 96 backend (35 Application, 49 Engine, 12 Integration) + 6 frontend

## Phase 5 — Analytics & Adaptive Difficulty (completed 2025-11-16)

Scope, tasks and acceptance criteria: see `docs/PHASE5_PLAN.md`; report:
`docs/reports/phase-5.md`.

- [x] `GetDashboardQuery`: accuracy, avg time, per-category / per-difficulty
      stats, recent history, recommendations — all server-side
- [x] Readiness estimate (Siap / Hampir Siap / Belum Siap) always labelled
      indicative with disclaimer, never a pass guarantee
- [x] `AdaptiveDifficultyService`: consecutive correct/incorrect + accuracy
      rules, clamped 0–4; `UseAdaptiveDifficulty` flag + UI toggle
- [x] Weakness Training auto-selects weakest category (adaptive difficulty)
- [x] `AnalyticsController` → `GET /api/analytics/dashboard`
- [x] Dashboard UI: stat cards, readiness, category bars, difficulty table,
      recommendations, history; adaptive switch on mode select
- [x] Tests: 109 backend (44 Application incl. 8 adaptation-rule tests,
      49 Engine, 16 Integration incl. dashboard + weakness selection) + 6 frontend

## Phase 6 — Administration & Generator Builder (completed 2025-11-17)

Scope, tasks and acceptance criteria: see `docs/PHASE6_PLAN.md`; report:
`docs/reports/phase-6.md`.

- [x] Rule-based engine: `SafeExpressionEvaluator` (sandboxed arithmetic
      parser), `GeneratorConfigDefinition`, `RuleConfigValidator`,
      `RuleBasedGenerator` with per-difficulty parameter scaling
- [x] `GeneratorRegistry` versioned (all versions vs active set) + constructor
      fix; `GeneratorConfigLoader` activates Published configs at startup
- [x] Domain: `GeneratorConfig` (Draft/Published/RolledBack/Rejected),
      `AppSetting`; migrations `AddGeneratorConfigs`, `AddAppSettings`
- [x] Admin use cases: ListUsers, SetUserRoles, SetUserLockout, GetSettings,
      UpdateSetting (Administrator only)
- [x] Generator use cases: CRUD, CreateVersion, Preview, MassTest, Publish
      (requires passed mass test), Rollback (ContentManager + Administrator)
- [x] Exam profile admin CRUD with slug uniqueness
- [x] Web: UsersController, SettingsController, GeneratorConfigsController,
      extended ExamProfilesController; configurable rate limit
- [x] Frontend: admin hub, `/admin/pengguna`, `/admin/generator` (preview,
      mass test, publish, rollback, new version); multi-role `RequireRole`
- [x] C# script generators: out-of-process sandbox design documented
      (`docs/SCRIPT_GENERATOR_SANDBOX.md`), in-process execution forbidden,
      `IScriptGeneratorRunner` seam prepared
- [x] Tests: 137 backend (70 Engine incl. 21 new, 44 Application,
      23 Integration incl. role guards + publish/rollback lifecycle) + 6 frontend

## Phase 7 — PWA & Advertising (completed 2025-11-17)

Scope, tasks and acceptance criteria: see `docs/PHASE7_PLAN.md`; report:
`docs/reports/phase-7.md`.

- [x] PWA: manifest (installable icons incl. maskable), hand-rolled service
      worker (app-shell precache, SWR static, network-first API + offline
      fallback), theme-color/apple-touch-icon, production-only registration
- [x] Ads backend: `GET /api/ads/config` from `AppSetting` keys
      (`Ads.Enabled/Provider/Client/PlacementsJson`), disabled by default
- [x] Ads frontend: `AdProvider` abstraction + `AdSenseProvider`,
      `createAdProvider` factory, placement-aware `AdSlot`, consent banner
      (stored + respected), placements below-header / above-nav
- [x] Hard rule enforced: no ad renders on active session routes
      (`useIsExamActive`), covered by frontend tests
- [x] Tests: 139 backend (25 Integration incl. ads config) + 15 frontend
      (9 new ad/consent tests)

## Phase 8 — Production Deployment & Optimization (completed 2025-11-17)

Scope, tasks and acceptance criteria: see `docs/PHASE8_PLAN.md`; report:
`docs/reports/phase-8.md`.

- [x] Containers hardened: API non-root (`USER app`), frontend on
      `nginx-unprivileged`; gzip + cache headers (immutable assets, no-cache
      service worker)
- [x] Compose: log rotation (json-file, max-size/max-file) on all services,
      frontend healthcheck, frontend waits for healthy API, API readiness
      check against database
- [x] Operations: `deploy/scripts/backup.sh` + `restore.sh`;
      `deploy/PRODUCTION_CHECKLIST.md`; README operations section
- [x] Performance: route-level code splitting (React.lazy + Suspense), main
      chunk 500+ kB → 319 kB (102 kB gzip), per-page chunks
- [x] Release: `docs/reports/phase-8.md` incl. known-limitations register;
      secret scan clean
- [x] Tests: 139 backend + 15 frontend green; compose validated in CI

All eight phases complete.

## Decisions Log

| Date | Decision | Rationale |
|---|---|---|
| 2025-11-16 | Greenfield build; no migration from legacy code | Workspace contained no repository |
| 2025-11-16 | Monorepo with strict `QuestionEngine` project boundary | Requirement: engine independent of presentation |
| 2025-11-16 | Kraepelin modeled as session type, not MCQ | Continuous timed grid does not fit question/option shape |
| 2025-11-16 | .NET 10 instead of .NET 8 | User decision ("use dotnet 10") |
| 2025-11-16 | Integration tests run on SQLite in-memory | No Docker daemon in sandbox for PostgreSQL/Testcontainers |
| 2025-11-16 | Mediator (`martinothamar/Mediator`) instead of MediatR | User decision; source-generated, AOT-friendly, better perf |
| 2025-11-16 | Ardalis.Result + GuardClauses + FluentValidation pipeline | User decision; expected outcomes as values, exceptions only for programmer errors |
| 2025-11-16 | `Mediator.SourceGenerator` referenced only in `Web` | Library requirement: generator belongs in the edge project; `AddMediator` is generated there |
