# Phase 4 Report — Training & Examination System

**Status:** completed 2025-11-16

## Scope delivered

### Domain (`backend/src/Domain`)
- `TrainingSession` (mode, scope, status, deadline, counters, score, profile link)
- `SessionQuestion` — generation metadata + server-side answer key only; the
  rendered question is regenerated on demand from (generator, version, seed,
  difficulty). No rendered payload is ever persisted.
- `ExamProfile`, enums `TrainingMode`, `SessionStatus`, `QuestionCategoryScope`.

### Application (`backend/src/Application`)
- `StartSessionCommand` — creates session, generates N questions server-side,
  stores metadata + answer key, sets deadline from mode or exam profile.
- `GetSessionQuestionQuery` — regenerates the question from stored metadata via
  `GeneratorRegistry`; never returns the correct answer.
- `SubmitAnswerCommand` — server-side validation, concise feedback
  (correct/incorrect + correct answer), single-submit enforcement, deadline
  enforcement.
- `FinishSessionCommand` — auto-status (Completed/Expired), counter roll-up,
  scoring via `IScoringService`.
- `GetSessionResultQuery`, `ListExamProfilesQuery`.
- `ScoringService` — practice/weakness = weighted, timed/exam/daily = percentage,
  survival = 10 pts per correct; pass/fail against profile threshold.
- `QuestionGenerationService` — routes category+seed to registered generators.
- `ICurrentUserService` abstraction; FluentValidation validators for all inputs.

### Infrastructure / Web
- EF Core mapping + migration `AddTrainingSessions` (PostgreSQL-ready).
- `ExamProfileSeeder` — 6 seeded profiles (psikotes-umum, cpns, bumn, tni-polri,
  swasta, psikotes-umum-lengkap).
- `SessionsController` (start / get question / answer / finish / result) and
  `ExamProfilesController`; all session endpoints `[Authorize]`.
- `CurrentUserService` from JWT `sub` claim.

### Frontend (`frontend/src/features/training`)
- `ModeSelectPage` — mode (6), category (7), difficulty (5), question count.
- `SessionRunnerPage` — countdown with auto-submit, question navigation dialog,
  review marking, immediate concise feedback, distraction-free layout.
- `SessionResultPage` — score, pass/fail, per-question breakdown.
- Routes: `/latihan`, `/sesi/:id`, `/sesi/:id/hasil` (React Router, no switch).

## Acceptance criteria
- [x] Answers validated & scored server-side only; correct answer never sent
      before submission (verified by integration test)
- [x] Questions reproducible from stored metadata (no payload storage)
- [x] All six training modes startable; timed modes enforce deadline
- [x] Six exam profiles seeded and selectable (profile overrides count+duration)
- [x] Integration tests: start → answer → double-submit rejected → finish → result
- [x] Frontend build + tests green

## Verification
- `dotnet test` — 96 tests green (35 Application, 49 Engine, 12 Integration)
- `npm run build` + `npm test` — green (6 tests)
