# Phase 8 Report — Production Deployment & Optimization

Plan: `docs/PHASE8_PLAN.md`.

## Delivered

### Deployment hardening
- `backend/Dockerfile`: API container now runs as non-root (`USER app`);
  a minimal download client is installed in the final image so the container
  healthcheck can probe `/health/ready`.
- `frontend/Dockerfile`: switched to `nginxinc/nginx-unprivileged:1.27-alpine`
  (no root master process), listening on 8080.
- `frontend/nginx.conf`: gzip for text assets; immutable long-cache for
  hashed `/assets/`; `no-cache` for `sw.js`; short cache for the manifest.
- `deploy/docker-compose.yml`: json-file log rotation on all services
  (max-size 10m, max-file 3–5); frontend healthcheck added; `frontend` now
  waits for the API to be *healthy* (was: started); API healthcheck hits
  `/health/ready` (database check); restart policies unchanged
  (`unless-stopped`).

### Operations
- `deploy/scripts/backup.sh` — gzipped `pg_dump` with timestamp.
- `deploy/scripts/restore.sh` — restore with confirmation prompt.
- `deploy/PRODUCTION_CHECKLIST.md` — secrets, TLS/reverse proxy, database,
  containers, application, release sections.
- `deploy/README.md` — operations scripts section added.

### Performance
- Route-level code splitting: all pages lazy-loaded via `React.lazy` +
  `Suspense` with a shared `PageLoader`; main JS chunk reduced from >500 kB
  (build warning) to 319 kB (102 kB gzip) with per-page chunks.

### Release documentation
- This report; `PROGRESS.md` updated with the final phase and the
  known-limitations register below.

## Known limitations
- C# script generator worker (out-of-process sandbox) remains designed but
  unimplemented — requires containerized CI to verify isolation
  (`docs/SCRIPT_GENERATOR_SANDBOX.md`; `IScriptGeneratorRunner` seam ready).
- Integration tests run on SQLite in-memory; production parity (PostgreSQL)
  is exercised only by the compose smoke path, not automated tests.
- Kraepelin/Pauli continuous-grid mode is modeled as a session type; dedicated
  grid UI polish is future work.
- No automated E2E browser tests (Playwright) yet; coverage is unit +
  integration.
- Lighthouse installability verified by manifest structure + CI build, not by
  an in-CI Lighthouse run.

## Verification
- All suites green: 139 backend tests (70 Engine, 44 Application,
  25 Integration), 15 frontend tests.
- `tsc -b && vite build` clean, no chunk-size warning; oxlint 0 errors.
- Compose YAML validated structurally (CI runs `docker compose config -q`).
- Secret scan: no credentials/keys in the repository.
- Shell scripts pass `sh -n` syntax check.

## Acceptance criteria
1. ✅ Compose config valid (CI compose job) — structurally validated locally;
   CI green confirms.
2. ✅ All test suites green; secret scan clean.
3. ✅ Backup/restore scripted (`deploy/scripts/`) and documented
   (`deploy/README.md`).
4. ✅ Production checklist in `deploy/PRODUCTION_CHECKLIST.md`; this release
   report written.
