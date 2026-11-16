# PROGRESS.md

Single source of truth for project progress. Update after every completed task.
Phases are defined in `docs/PHASES.md`; plans in `docs/PHASE1_PLAN.md` etc.

Status legend: ⬜ not started · 🟨 in progress · ✅ done · ⛔ blocked

## Phase Overview

| # | Phase | Status | Report |
|---|---|---|---|
| 0 | Assignment: inspection, evaluation, Phase 1 plan | ✅ | `docs/reports/phase-0.md` |
| 1 | Foundation, architecture, authentication, database | ⬜ | — |
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

## Phase 1 — Foundation (not started)

Scope, tasks and acceptance criteria: see `docs/PHASE1_PLAN.md`.

- [ ] Monorepo skeleton (`backend/`, `frontend/`, `deploy/`, `docs/`)
- [ ] Backend solution: Api / Application / Domain / Infrastructure / QuestionEngine projects
- [ ] EF Core + Npgsql wired, initial migration, PostgreSQL in Docker
- [ ] ASP.NET Core Identity: register/login/refresh, 3 roles seeded
- [ ] JWT authentication + role-based authorization
- [ ] Health checks, Serilog logging, global error handling
- [ ] Frontend: Vite + React + TS, Tailwind, shadcn/ui installed, React Router shell
- [ ] Auth pages (login/register), protected routes, role guards
- [ ] Docker Compose: api + db + frontend, health checks, persistent volume
- [ ] Automated tests (xUnit + Vitest) wired and green
- [ ] Phase 1 completion report

## Decisions Log

| Date | Decision | Rationale |
|---|---|---|
| 2025-11-16 | Greenfield build; no migration from legacy code | Workspace contained no repository |
| 2025-11-16 | Monorepo with strict `QuestionEngine` project boundary | Requirement: engine independent of presentation |
| 2025-11-16 | Kraepelin modeled as session type, not MCQ | Continuous timed grid does not fit question/option shape |
