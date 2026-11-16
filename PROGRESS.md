# PROGRESS.md

Single source of truth for project progress. Update after every completed task.
Phases are defined in `docs/PHASES.md`; plans in `docs/PHASE1_PLAN.md` etc.

Status legend: ⬜ not started · 🟨 in progress · ✅ done · ⛔ blocked

## Phase Overview

| # | Phase | Status | Report |
|---|---|---|---|
| 0 | Assignment: inspection, evaluation, Phase 1 plan | ✅ | `docs/reports/phase-0.md` |
| 1 | Foundation, architecture, authentication, database | ✅ | `docs/reports/phase-1.md` |
| 2 | Question Generation Engine | ⬜ | — |
| 3 | Six question categories & visual rendering | ⬜ | — |
| 4 | Training & examination system | ⬜ | — |
| 5 | Analytics & adaptive difficulty | ⬜ | — |
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
