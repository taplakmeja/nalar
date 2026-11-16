# Architecture Evaluation — Phase 0 Report

Date: 2025-11-16 · Status: Final

## 1. Existing Repository Inspection

The workspace was inspected and found **completely empty** — no repository, no source
code, no documentation, no CI configuration.

**Conclusion:** this is a **greenfield build**. There is no existing technology or
architecture to identify, and no migration risk. The evaluation below therefore
validates the *proposed* architecture against the requirements rather than against
legacy code, and focuses on risks and gaps that must be resolved before/during Phase 1.

## 2. Proposed Architecture vs. Requirements

| Requirement | Verdict | Notes |
|---|---|---|
| React + TS + Tailwind + shadcn/ui | ✅ Supported | Vite scaffold; shadcn/ui CLI installs all needed components; mobile-first via Tailwind breakpoints |
| Router, no big switch cases | ✅ Supported | React Router with feature-first route modules and layout routes |
| ASP.NET Core Web API (C#) | ✅ Supported | .NET 8 LTS, minimal APIs or controllers |
| PostgreSQL + EF Core | ✅ Supported | Npgsql provider, code-first migrations |
| ASP.NET Core Identity | ✅ Supported | Add JWT bearer on top for the SPA; seed 3 roles |
| SVG + Canvas rendering | ✅ Supported | Abstract/spatial questions rendered from *parameters*, not stored images |
| Docker Compose | ✅ Supported | api + db + frontend (nginx) + script-sandbox worker |
| PWA | ✅ Supported | Vite PWA plugin; careful caching policy (never cache active exam state) |
| No external AI | ✅ Supported | Engine is deterministic C# + versioned datasets |
| C# script generators (isolated) | ⚠️ Highest-risk item | Roslyn scripting in-process is **not** acceptable. Requires out-of-process sandbox — see Risk R1 |
| Kraepelin grid | ⚠️ Model mismatch | Not a question/options shape; needs its own session model — see Risk R3 |

## 3. Architectural Risks

- **R1 — Custom C# script execution.** Compiling/running user scripts inside the API
  process is a code-execution vulnerability. Mitigation: separate worker (own container)
  that compiles with Roslyn and executes with CPU/memory/wall-clock limits, no network,
  read-only filesystem; scripts validated & versioned before publication; kill-switch
  per script. Decided in Phase 1 at the architecture level, implemented in Phase 6.
- **R2 — Determinism & reproducibility.** Generators must be pure functions of
  (seed, version, parameters, difficulty). Mitigation: enforced by project boundary
  (`QuestionEngine` references nothing), a determinism unit-test harness that runs every
  generator twice per seed, and never pinning to wall-clock/GUID randomness.
- **R3 — Kraepelin/Pauli shape.** A timed continuous-arithmetic grid does not fit the
  "question + 4 options" model. Mitigation: model as a distinct `KraepelinSession`
  aggregate with its own scoring (throughput, accuracy, fatigue index).
- **R4 — Verbal dataset quality/licensing.** Synonyms/antonyms/analogies in Bahasa
  Indonesia need a versioned, legally usable dataset with enough entries per difficulty.
  Mitigation: dataset format + versioning designed in Phase 1 (`datasets/` + loader in
  Infrastructure); seed dataset authored in Phase 3; provenance recorded.
- **R5 — Distractor quality.** Numeric distractors are easy (plausible perturbations);
  verbal/visual distractors need dataset- and parameter-based strategies. Poor
  distractors make questions guessable. Mitigation: per-generator distractor strategy +
  automated ambiguity/duplicate detection in QA layer.
- **R6 — Answer leakage.** Sending correct answers to the client enables cheating.
  Mitigation: server-side session state; answers validated on submit; client receives
  only question payloads + option list, never the key.
- **R7 — Visual question complexity (Abstract/Spatial).** Programmatic SVG/Canvas
  generation is a large surface. Mitigation: questions store *render parameters*;
  frontend ships typed renderers per generator family; engine emits a `visual` spec
  (deterministic), never bitmaps.
- **R8 — Scope creep.** 8 phases, 6 categories, 3 authoring methods, ad module, PWA.
  Mitigation: strict phase gates in `PROGRESS.md`; Phase 1 deliberately excludes
  engine internals, analytics, ads, and Generator Builder.

## 4. Missing / Ambiguous Requirements (to confirm during Phase 1)

1. **Verbal dataset source & license** — who authors/acquires it, and attribution format.
2. **Adaptive difficulty algorithm** — exact thresholds (accuracy %, streaks, time
   windows). Proposed default documented in Phase 5 plan; tunable via config.
3. **Exam profile catalog** — CPNS/BUMN/TNI-Polri distributions need per-profile
   defaults; only structure + one seeded profile (General Psychotest) in Phase 4.
4. **Offline behavior** — PWA offline shell yes; question generation stays server-side
   (no offline exams) to preserve scoring integrity.
5. **Localization** — Indonesian-only UI for v1; keep strings centralized anyway.
6. **Consent for ads** — EU-style consent banner requirement; AdSense only loads after
   consent where applicable; exam routes hard-block ad slots.
7. **Scoring rules per profile** — define penalty-for-wrong-answer config (some
   psychotests penalize); store as profile configuration, default 0 penalty.

## 5. Phase 1 Decision

Proceed with the proposed stack and monorepo layout in `AGENTS.md`. Phase 1 plan and
acceptance criteria: `docs/PHASE1_PLAN.md`.
