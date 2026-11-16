# Phase Definitions

Each phase has defined scope, tasks, acceptance criteria, automated tests, and a
completion report in `docs/reports/phase-N.md`. Do not start a phase before the
previous one meets its Definition of Done.

| Phase | Scope | Key deliverables |
|---|---|---|
| 1 | Foundation | Monorepo, clean architecture boundaries, EF Core + PostgreSQL, Identity + JWT + 3 roles, frontend shell (Router + shadcn/ui), Docker Compose, CI, tests |
| 2 | Question Generation Engine | Generator contracts, seeded RNG, template + algorithmic generation, parameters, distractor strategies, validation layer 1, determinism harness, generator registry & versioning |
| 3 | Categories & visual rendering | Generators for Numerical, Verbal (versioned datasets), Logical, Abstract (SVG), Spatial (SVG/Canvas), Kraepelin grid; QA layers 2–3 (mass testing, human review workflow) |
| 4 | Training & examination | Practice, Timed Test, Exam Simulation, Daily Challenge, Weakness Training, Survival; exam profiles; server-side scoring; session model; exam UI (countdown, navigation, review marking, auto-submit) |
| 5 | Analytics & adaptive difficulty | Accuracy/time stats, per-category & per-generator performance, history, weakness detection, recommendations, readiness (indicative), adaptive engine, training plans |
| 6 | Administration & Generator Builder | User/role admin, settings, dataset management, profile management, Visual Generator Builder, C# script generators in isolated sandbox worker, preview/test/publish/rollback |
| 7 | PWA & advertising | Service worker, installability, offline shell; ad module with provider abstraction (AdSense first), placements, consent management, exam-route suppression |
| 8 | Production deployment | Hardening, rate limits, backups/restore drills, monitoring, performance tuning, load testing, release docs |
