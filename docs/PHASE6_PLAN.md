# Phase 6 Plan — Administration & Generator Builder

## Scope
Administration (users, roles, settings), generator management (config versions,
preview, test, publish, rollback), rule-based Visual Generator Builder backend.

## Tasks
- Domain: `GeneratorConfig` (versioned config JSON, Draft/Published/RolledBack/
  Rejected lifecycle).
- Engine: `RuleBasedGenerator` interpreting builder configs (parameters,
  operations, randomization, answer/difficulty rules).
- Use cases: user management (admin), generator config CRUD + preview + mass
  test + publish + rollback (ContentManager), exam profile CRUD.
- Web: admin/content endpoints with role authorization.
- Frontend: admin users page; generator config manager with preview/test.
- C# script generators: out-of-process sandbox **design** documented; in-process
  execution explicitly forbidden (security). Runtime sandbox deferred with
  rationale and interface prepared.

## Acceptance Criteria
1. Only ContentManager manages generators; only Administrator manages users.
2. Publishing requires passing mass-test QA (duplicates, uniqueness, determinism).
3. Rollback restores previous published version.
4. Rule-based generator produces valid, deterministic questions from configs.
5. Integration tests cover role guards + publish/rollback lifecycle.
