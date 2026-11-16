# Phase 2 Plan — Question Generation Engine

## Scope
Core deterministic generation engine: full contracts, technical validation,
distractor generation, registry, determinism harness, and engine-side QA hooks.
No UI, no persistence (metadata-only contract for later phases).

## Tasks
- Engine contracts: `IQuestionGenerator`, `GeneratedQuestion` (with optional SVG
  payload for visual categories), `GeneratorIdentity`, enums.
- `SeededRandom` deterministic utilities (pick, shuffle, numeric ranges).
- `NumericDistractors`: automatic plausible distractor generation with uniqueness.
- `TechnicalValidator`: layer-1 QA — prompt/answer/option structure, unique
  options, correct answer present, validation status.
- `GeneratorRegistry` (id@version lookup, latest) + `DeterminismHarness`
  (per-difficulty reproducibility sweeps).
- Sample generator retained as contract reference.

## Acceptance Criteria
1. Same seed + generator version ⇒ byte-identical question (harness-verified).
2. All generated questions pass `TechnicalValidator`.
3. Distractors are unique, never equal to the correct answer.
4. Engine has zero references to Web/Infrastructure (CI check).
5. xUnit tests green for the above.
