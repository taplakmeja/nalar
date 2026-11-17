# Production Checklist

## Secrets
- [ ] `deploy/.env` created from `.env.example`; never committed (verify with
      `git status` / `.gitignore`).
- [ ] `POSTGRES_PASSWORD`: strong, generated (`openssl rand -base64 24`).
- [ ] `JWT_KEY`: 32+ characters, generated (`openssl rand -base64 48`), unique
      per environment; rotating it invalidates all sessions.
- [ ] `SEED_ADMIN_EMAIL` / `SEED_ADMIN_PASSWORD`: set for first boot, then the
      password changed on first login. Do not leave the seed password in `.env`
      longer than necessary.
- [ ] No secrets in images, logs, or the repository (grep before release).

## Network & TLS
- [ ] TLS-terminating reverse proxy (Caddy / Nginx / Traefik) in front of the
      `frontend` container; redirect to TLS enforced.
- [ ] `FRONTEND_URL` set to the public TLS origin (API CORS + PWA scope).
- [ ] Postgres port **not** published; only `frontend` exposed, on 127.0.0.1 or
      the proxy network if the proxy runs on the host.
- [ ] Optional: fail2ban / ufw on the host; auto-renewing certificates.

## Database
- [ ] `Database:MigrateOnStartup=true` (compose default) so migrations apply on
      deploy; check `docker compose logs api` after first boot.
- [ ] Daily backup scheduled (cron example in `README.md`) or
      `scripts/backup.sh` wired into the host scheduler.
- [ ] Restore rehearsed at least once with `scripts/restore.sh`.
- [ ] Volume `db-data` included in host-level backup policy.

## Containers
- [ ] `docker compose up -d --build` completes; `docker compose ps` shows all
      three services healthy.
- [ ] API runs as non-root (`USER app`); frontend uses
      `nginx-unprivileged`.
- [ ] Log rotation active (`json-file`, max-size/max-file per service).
- [ ] Restart policies `unless-stopped` verified: `docker compose kill api`
      recovers automatically.

## Application
- [ ] Health endpoints respond: `/health/live` (process), `/health/ready`
      (database check) through the proxy.
- [ ] Rate limiter reviewed (`RateLimit:PermitLimit`, default 20 auth
      requests/min/IP) — raise only with a proxy-level limiter in place.
- [ ] Ads disabled by default; if enabled, verify consent banner and exam-route
      suppression.
- [ ] PWA verified over TLS: manifest loads, service worker registers,
      install prompt available.

## Release
- [ ] CI green on the release commit (backend, frontend, compose jobs).
- [ ] Tag the release; record the deployed commit in the release report.
- [ ] Smoke test after deploy: register, login, start session, answer, result.
