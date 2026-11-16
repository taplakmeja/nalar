# Phase 7 Plan — PWA & Advertising

## Scope
Progressive Web App shell (installable, offline-capable) and a
provider-independent advertising module with consent management.

## Tasks
- PWA: web manifest, service worker (app-shell cache, network-first API),
  install icons, registration.
- Ads: `AdSlot` component, provider abstraction (AdSense first), enable/disable
  controls, consent gate, configurable placements.
- Hard rule: ads never render during active examinations (mode-aware hook).

## Acceptance Criteria
1. Lighthouse-installable manifest + service worker registered in production.
2. Ad provider is swappable behind an interface; AdSense default.
3. No ad renders on exam routes / active sessions.
4. Consent stored and respected before loading any provider.
5. Frontend tests cover ad suppression during exams.
