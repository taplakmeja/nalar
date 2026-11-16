# Phase 8 Plan — Production Deployment & Optimization

## Scope
Deployment hardening, operational documentation, and release readiness.

## Tasks
- Compose: restart policies, healthchecks verified, logging limits, env template
  completeness.
- Production checklist (secrets, HTTPS/reverse proxy, migrations, backups).
- CI: full test matrix, compose validation, engine boundary check (existing).
- Release report and known-limitations register.

## Acceptance Criteria
1. `docker compose config` valid; compose job green in CI.
2. All test suites green; no secrets in repo.
3. Backup/restore documented and scripted.
4. Production checklist in deploy docs; release report written.
