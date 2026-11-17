# Phase 6 Report — Administration & Generator Builder

Plan: `docs/PHASE6_PLAN.md`.

## Delivered

### Rule-based generation (Visual Generator Builder backend)
- `QuestionEngine/Rules/SafeExpressionEvaluator` — recursive-descent arithmetic
  parser (`+ - * / %`, unary minus, parentheses, variables). Rejects function
  calls, unknown identifiers, division by zero, non-finite results.
- `RuleParameterDefinition` / `RuleOperationDefinition` /
  `GeneratorConfigDefinition` — declarative builder config (JSON, parsed
  case-insensitively) with per-difficulty parameter scaling
  (`MaxByDifficulty`, 5 entries).
- `RuleConfigValidator` — structure, identifier syntax, duplicate names,
  Min≤Max, difficulty array length, expression parseability, prompt
  placeholders.
- `RuleBasedGenerator : IQuestionGenerator` — samples parameters via
  `SeededRandom`, evaluates operations in order, derives answer + distractors
  (dedup + numeric fallback), renders prompt template, shuffles options
  deterministically.
- `GeneratorRegistry` rewritten: all versions (`Get`/`GetLatest`) + active set
  (`All`/`Activate`/`Deactivate`); constructor now activates supplied
  generators (bug fix — previously the argument was ignored).

### Administration
- Domain: `GeneratorConfig` (versioned, Draft/Published/RolledBack/Rejected),
  `AppSetting` (key-value); migrations `AddGeneratorConfigs`, `AddAppSettings`.
- Users: ListUsers, SetUserRoles, SetUserLockout (Administrator only).
- Settings: GetSettings, UpdateSetting (Administrator only).
- Exam profiles: admin list (incl. non-public), Create, Update, Delete with
  slug uniqueness (Administrator + ContentManager).

### Generator management (ContentManager + Administrator)
- CRUD + `CreateVersion` (new draft version, blocks when an open draft exists).
- Preview (2 samples × 5 difficulties, seeds from 1000).
- Mass test via engine `MassTester` (50 samples/difficulty); pass rule:
  deterministic ∧ 0 invalid ∧ duplicate rate ≤ 20%. Report persisted to
  `MassTestJson`.
- Publish: draft-only, requires passed mass test, demotes previous Published
  version to RolledBack, activates in registry.
- Rollback: swaps current Published with highest RolledBack version below it.
- `GeneratorConfigLoader` activates Published rule generators at startup.

### Web API
- `api/admin/users` (list, roles, lockout) — Administrator.
- `api/admin/settings` (list, update) — Administrator.
- `api/generator-configs` (CRUD, versions, preview, mass-test, publish,
  rollback) — Administrator + ContentManager.
- `api/exam-profiles` extended with admin CRUD — Administrator + ContentManager.
- Rate limiter limit now configurable (`RateLimit:PermitLimit`, default 20/min).

### Frontend
- Admin hub with role-aware tiles; `/admin/pengguna` (roles + lockout switches);
  `/admin/generator` (create, JSON editor, preview, mass test, publish,
  rollback, new version). `RequireRole` now accepts multiple roles.

### C# script generators
- Out-of-process sandbox design documented (`docs/SCRIPT_GENERATOR_SANDBOX.md`);
  in-process execution explicitly forbidden; `IScriptGeneratorRunner` seam
  prepared. Worker implementation deferred to Phase 8 (needs containerized CI).

## Verification
- 137 backend tests green: 70 Engine (incl. 21 new: evaluator, generator,
  config validator), 44 Application, 23 Integration (incl. role guards 403/200
  and full create → mass-test → publish → v2 → rollback lifecycle).
- Frontend: `tsc -b && vite build` clean, oxlint clean, 6 vitest tests green.
- Bugs found & fixed during verification: DI assembly scan crashed on
  `RuleBasedGenerator` (no parameterless ctor) — scan now filters to
  parameterless-constructor types; `GeneratorRegistry` constructor ignored its
  argument; integration JWTs minted before role promotion lacked the new role
  (tests now re-login after promotion).

## Acceptance criteria
1. ✅ Only ContentManager (+Administrator) manages generators; only
   Administrator manages users — enforced by role attributes, covered by
   integration tests.
2. ✅ Publishing requires passing mass test — enforced in Publish handler,
   covered by integration test (publish-before-test → 400).
3. ✅ Rollback restores previous published version — covered by lifecycle test.
4. ✅ Rule-based generator produces valid, deterministic questions — engine
   unit tests (determinism, difficulty scaling, option uniqueness).
5. ✅ Integration tests cover role guards + publish/rollback lifecycle.
