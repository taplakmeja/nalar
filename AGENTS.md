# AGENTS.md — Indonesian Psikotes Training Platform

Guidance for AI agents and human contributors working in this repository.

## Project Identity

A production-ready, mobile-first Indonesian psikotes (psychotest) training platform.
All examination questions are generated programmatically by a modular **Question
Generation Engine**. No external AI services are used or required. The app is free;
monetization is via a provider-independent advertising module (Google AdSense first).

**Disclaimer that must appear in the product:** generated practice tests are *not*
official examination questions. Readiness scores are indicative only.

## Technology Stack (fixed — do not substitute)

| Layer | Technology |
|---|---|
| Frontend | React + TypeScript (Vite) |
| UI | Tailwind CSS + shadcn/ui (all components properly installed) |
| Routing | React Router (no giant switch-case routers) |
| Backend | ASP.NET Core Web API (C#) |
| Database | PostgreSQL |
| ORM | Entity Framework Core (migrations maintained) |
| Auth | ASP.NET Core Identity (roles: User, Administrator, ContentManager) |
| Visual rendering | SVG + HTML Canvas |
| Deployment | Docker Compose |
| App type | Progressive Web App (PWA) |

## Repository Layout (monorepo)

```
/
├── AGENTS.md
├── PROGRESS.md
├── README.md
├── docs/                    # architecture, ADRs, phase plans & reports
├── backend/
│   ├── src/
│   │   ├── Api/             # ASP.NET Core host (controllers/endpoints, auth)
│   │   ├── Application/     # use cases, DTOs, scoring, adaptive logic
│   │   ├── Domain/          # entities, enums, value objects
│   │   ├── Infrastructure/  # EF Core, Identity, repositories, datasets
│   │   └── QuestionEngine/  # generation engine — NO dependency on Api/UI
│   └── tests/
├── frontend/
│   ├── src/                 # feature-first structure, React Router
│   └── tests/
├── deploy/                  # docker-compose files, env templates, backup scripts
└── .github/                 # CI workflows
```

## Hard Rules

1. **Engine independence.** `QuestionEngine` must never reference `Api`,
   `Infrastructure`, or any frontend code. It is pure C#: in → (seed, generator id,
   version, difficulty, parameters) → out → question + answer + distractors + metadata.
2. **Determinism.** Same seed + same generator version ⇒ byte-identical question and
   answer key. Never use wall-clock time, GUIDs, or unseeded RNG inside generators.
3. **Never trust the client.** All scoring and answer validation happen server-side.
   Correct answers are never sent to the client before an answer is submitted.
4. **Fail closed on custom scripts.** C# script generators execute out-of-process in an
   isolated sandbox with CPU/memory/time limits and no filesystem or network access.
   Scripts are validated before publication.
5. **Minimal persistence.** Do not store fully rendered question payloads. Store
   generation metadata (generator id, version, seed, parameters) and regenerate on
   demand; store only what analytics and audit require.
6. **Migrations only.** Schema changes go through EF Core migrations — never manual SQL
   against production databases.
7. **Mobile-first.** Every UI change is designed for a small viewport first.
8. **No ads during active examinations.** The ad module must be suppressible per route/mode.
9. **Indonesian language.** User-facing content and question wording are in Bahasa
   Indonesia. Verbal datasets are versioned and attributed.

## Question Generation Contract

Every generated question carries:

- `category` (Numerical | Verbal | Logical | Abstract | Spatial | Kraepelin)
- `generatorId`, `generatorVersion`
- `difficulty` (Beginner | Easy | Intermediate | Advanced | Expert)
- `seed`
- `parameters` (JSON, sufficient to reproduce)
- `correctAnswer`
- `options` (unique distractors + correct answer)
- `validation` (technical validation result)

Kraepelin/Pauli is a special shape: a timed continuous-arithmetic grid session, not a
single multiple-choice question. Model it as a separate exercise/session type.

## Quality Assurance

Three layers, in order: technical validation (automatic, per question), automated
mass testing (per generator version: duplicates, ambiguity heuristics, difficulty
consistency, edge cases, performance), human review (Content Manager approves/rejects
generator **versions** — never every individual question).

## Workflow for Agents

- Work in phases (see `docs/PHASES.md` and `PROGRESS.md`). Do not implement future
  phases prematurely.
- Each phase ends with: automated tests green, acceptance criteria checked off in
  `PROGRESS.md`, and a completion report in `docs/reports/phase-N.md`.
- Backend tests: xUnit. Frontend tests: Vitest + React Testing Library.
- Commits: conventional, small, focused (`feat(engine): number sequence generator`).
- Keep PRs scoped to a single phase or task.

## Security Notes

- Secrets live in environment variables / user-secrets; never in the repo.
- Identity: strong password policy, rate-limited login, JWT with short lifetime.
- Custom scripts: never compile/run inside the API process.
- Validate all inputs server-side; use parameterized queries (EF Core default).
