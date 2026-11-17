# Phase 7 Report — PWA & Advertising

Plan: `docs/PHASE7_PLAN.md`.

## Delivered

### PWA
- `public/manifest.webmanifest` — name/short_name (id), standalone display,
  portrait, theme #4f46e5, icons: 192/512 `any` + 512 `maskable` (generated
  PNGs), `start_url` `/`.
- `public/sw.js` — hand-rolled service worker (no new dependencies):
  app-shell precache (`/`, `index.html`, manifest, favicon), stale-while-revalidate
  for same-origin static assets, network-first with offline fallback (503 JSON)
  for `/api/*`.
- `index.html`: manifest link, `theme-color`, `apple-touch-icon`, description,
  `viewport-fit=cover`.
- `main.tsx`: SW registration in production builds only.

### Advertising module
- Backend: `GET /api/ads/config` (public) reads `Ads.Enabled`, `Ads.Provider`,
  `Ads.Client`, `Ads.PlacementsJson` from `AppSetting` (defaults: disabled,
  adsense, no client, no placements). Administrators toggle via the existing
  settings API.
- Frontend `features/ads/`:
  - `AdProvider` interface — provider abstraction; `AdSenseProvider` default;
    `createAdProvider` factory (unknown provider → null).
  - `AdSlot` — placement-aware component; renders nothing unless ads enabled +
    consent granted + not in an active exam.
  - `useIsExamActive` — route-aware hook: suppresses ads on active session
    routes (`/sesi/:id`, but not the result page).
  - `ConsentBanner` — bottom banner (Setuju/Tolak); consent persisted in
    localStorage and respected before any provider loads.
  - Placements: `below-header`, `above-nav` in `AppLayout`.

## Verification
- 139 backend tests green (70 Engine, 44 Application, 25 Integration incl.
  ads config defaults + enabled-from-settings).
- 15 frontend tests green, incl. 9 new ad tests: disabled → no render, no
  consent → no render, denied consent → no render, active exam → no render,
  consent + enabled outside exams → renders, consent banner stores choice,
  provider factory.
- `tsc -b && vite build` clean; oxlint 0 errors.

## Acceptance criteria
1. ✅ Installable manifest + service worker registered in production builds.
2. ✅ Provider swappable behind `AdProvider` interface; AdSense default.
3. ✅ No ad renders on active session routes — covered by test.
4. ✅ Consent stored and respected before loading any provider — covered.
5. ✅ Frontend tests cover ad suppression during exams.
