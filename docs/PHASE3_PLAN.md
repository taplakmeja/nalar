# Phase 3 Plan — Six Question Categories & Visual Rendering

## Scope
Concrete generators for all six categories, including versioned Indonesian
verbal datasets and programmatic SVG rendering for Abstract/Spatial.

## Tasks
- Numerical: number sequences, arithmetic (existing), fractions, percentages,
  ratios/proportions, word problems (templates).
- Verbal: versioned embedded dataset (`verbal-id-v1.json`); synonyms, antonyms,
  analogies, vocabulary classification.
- Logical: syllogisms, logical sequences, statements & conclusions.
- Abstract: SVG shape sequences, shape rotation patterns.
- Spatial: cube counting (isometric SVG), rotation matching, paper folding.
- Kraepelin/Pauli: continuous arithmetic grid session (separate session shape,
  not MCQ).

## Acceptance Criteria
1. Every category has at least one published generator per subtopic listed.
2. Determinism harness passes for every generator (all difficulties, bulk seeds).
3. Technical validation passes for every generator.
4. Verbal datasets carry a version and are embedded resources.
5. SVG payloads are deterministic strings for the same seed.
6. xUnit tests green (per-generator determinism + validity sweep).
