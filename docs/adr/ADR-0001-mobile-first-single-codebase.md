# ADR-0001: Mobile-first single responsive codebase

Status: Accepted · Date: 2026-11-17

## Context

Nalar must feel native on phones for the majority of Indonesian users, who reach
the product primarily via mobile browsers and home-screen installs. Options:
(a) one responsive codebase, (b) a separate `m.` mobile site, (c) a native wrapper
(Capacitor/React Native) around the SPA.

## Decision

One mobile-first responsive codebase (option a). Layout, typography, and touch
sizing are designed at 360–430 px first and enhanced upward. No user-agent
sniffing for layout; feature detection only. No native wrapper and no separate
mobile site.

## Consequences

- One deployment, one test matrix, no feature drift between "mobile" and
  "desktop" versions.
- The PWA manifest + service worker provide install and shell behavior on both
  platforms; iOS limitations (no install prompt API, safe-area quirks, 16.4+
  push) are handled in-app ([ADR-0003](ADR-0003-safe-area-strategy.md),
  [ADR-0005](ADR-0005-service-worker-update-flow.md)).
- Native-only capabilities (widgets, shortcuts, true fullscreen) are out of
  scope; if they become business-critical, a wrapper ADR supersedes this one.
