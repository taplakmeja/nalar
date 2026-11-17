# Backend — Nalar API

ASP.NET Core Web API for the Nalar psikotes training platform, built with Clean Architecture.

## Stack

- .NET 10 (`net10.0`), C# with nullable reference types
- EF Core 10 + Npgsql (PostgreSQL), code-first migrations
- ASP.NET Core Identity + JWT bearer auth (access + refresh tokens)
- [Mediator](https://github.com/martinothamar/Mediator) source-generator (not MediatR) with validation/performance/logging pipeline behaviors
- Ardalis.Result + Ardalis.GuardClauses; FluentValidation 12
- Serilog structured logging

## Structure

```
src/
  Domain/           entities, enums, no dependencies
  Application/      use cases (Mediator commands/queries), abstractions, services
  Infrastructure/   EF Core DbContext, migrations, Identity, external services
  Web/              controllers, Program.cs, DI composition root
  QuestionEngine/   self-contained question generation engine (no API/Infrastructure
                    references — enforced in CI); 18 generators across 6 categories,
                    SeededRandom determinism, TechnicalValidator + MassTester QA
tests/
  Psikotes.QuestionEngine.Tests      generator determinism, validators, rules
  Psikotes.Application.UnitTests     use case + scoring logic
  Psikotes.Web.IntegrationTests      WebApplicationFactory + SQLite in-memory
```

Dependency flow: `Web → Application → Domain`, `Infrastructure → Application → Domain`;
`QuestionEngine` is independent and referenced by Web only.

## Conventions

- One public class per file (small DTO/record co-location allowed where cohesion demands).
- Handlers return `Result<T>` / `Result`; controllers map via `ToActionResult()`.
- All scoring and answer validation happens server-side — never trust the client.
- Question generation is deterministic for a given generator id + version + seed.
- Guard clauses on constructor/method inputs; user-facing messages in Bahasa Indonesia.
- Every schema change ships as an EF migration.

## Commands

```bash
dotnet restore Psikotes.sln
dotnet build Psikotes.sln
dotnet test Psikotes.sln                        # all three test projects

dotnet ef migrations add <Name> \
  -p src/Infrastructure -s src/Web              # add a migration
dotnet ef database update \
  -p src/Infrastructure -s src/Web              # apply migrations
```

The API listens on :8080 in containers (`backend/Dockerfile`); readiness probe at
`/health/ready`. Deployment lives in `deploy/` (Docker Compose, backup/restore
scripts, production checklist).
