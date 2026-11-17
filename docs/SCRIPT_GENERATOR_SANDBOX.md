# C# Script Generator — Isolated Execution Design (Phase 6)

Status: **designed, deliberately deferred**. The rule-based Visual Generator
Builder ships in Phase 6; C# script generators are planned for a later phase.

## Hard rule

**Custom C# scripts must never execute inside the API process.** In-process
compilation (Roslyn `CSharpScript`, `Assembly.Load`, dynamic emit) would give a
script author full access to the API's memory, database credentials, and
process privileges. No configuration can make that safe for multi-tenant
content authoring.

## Prepared interface

`Psikotes.Application.Common.Interfaces.IScriptGeneratorRunner` is the seam the
rest of the system will program against. The Application layer depends only on
this abstraction; the actual runner will be an Infrastructure adapter.

## Approved design: out-of-process worker

1. **Submit** — Content Manager uploads a `.csx` script. Static analysis
   (Roslyn) rejects disallowed APIs: `System.IO`, `System.Net`, reflection,
   `Process`, P/Invoke, `Environment`. The script must define exactly one
   class implementing the engine contract (`IQuestionGenerator` shape).
2. **Bundle** — the validated script is packaged into a minimal worker image
   with only the QuestionEngine contracts assembly.
3. **Execute** — the API calls the worker over a local Unix domain socket /
   localhost-only HTTP channel:
   `POST /generate { generatorId, version, difficulty, seed }` → one question.
   The worker runs as an unprivileged user, with:
   - CPU limit (cgroups), e.g. 0.5 core
   - Memory cap, e.g. 128 MB
   - Wall-clock timeout per generation, e.g. 500 ms → kill + fail
   - Read-only root filesystem, no network namespace, no writable paths
     outside a small tmpfs
   - Output size cap per response
4. **Validate** — the same three QA layers apply (TechnicalValidator,
   MassTester, human review) before a script generator version can be
   published. Publication stores the script plus its validation bundle hash;
   the worker refuses to load bundles whose hash is not registered.
5. **Rollback** — identical to rule generators: versions flip between
   Draft/Published/RolledBack in `GeneratorConfig`.

## Why deferred

The sandbox requires container-level isolation (cgroups/namespaces) that cannot
be reproduced or verified in the current test environment (no Docker daemon in
CI). Shipping a weaker "sandbox" would violate the security requirement, so
Phase 6 delivers the interface seam and this design; the worker itself lands
with the Phase 8 hardening work where containerized CI is available.
