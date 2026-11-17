# Mobile Experience — Specification, Audit & Gap Analysis

Status: **Approved specification** · Owner: frontend · Last updated: 2026-11-17

This document is the single source of truth for the mobile experience of Nalar.
It defines the target platforms, audits what the codebase actually supports today
(with file references), lists every gap with severity, and states the requirements
that the implementation plan ([MOBILE_ROADMAP.md](MOBILE_ROADMAP.md)) must satisfy.
Key decisions are recorded as ADRs in [docs/adr/](adr/README.md).

---

## 1. Goal and Non-Goals

**Goal.** A *fully supported* mobile experience: an app-grade PWA that Indonesian
users can install on an Android or iOS phone and use to train and take exam
simulations on a 360–430 px wide screen, on a throttled 3G-class connection, with
one thumb, without losing exam progress.

**Non-Goals.**

- No native app wrapper (Capacitor/React Native) — see [ADR-0001](adr/ADR-0001-mobile-first-single-codebase.md).
- No separate mobile website (`m.nalar.…`) — one responsive codebase.
- No offline *exam taking* — scoring is server-authoritative by design; offline work
  is limited to shell caching and recovery, see [ADR-0004](adr/ADR-0004-exam-resilience.md).
- No tablet-specific 2-pane layouts in v1 (single column scales acceptably to 768 px).

## 2. Target Devices & Environments

| Dimension | Target |
|---|---|
| Viewport width | 320 px (iPhone SE) – 430 px (Pro Max); usable up to 768 px |
| Screen | 360×800 dp class Android; 375–430 pt iPhone incl. notch/Dynamic Island/home-indicator devices |
| Input | Touch primary (44 px targets), soft keyboard (numeric for Kraepelin), reduced-motion honored |
| Network | 3G slow (400 kbps, 400 ms RTT, occasional loss); intermittent offline must never lose a submitted answer |
| Browsers | Chrome Android (last 2 majors), Safari iOS ≥ 16 (incl. standalone PWA), Samsung Internet |
| Install | Android: installable PWA prompt; iOS: manual Add-to-Home-Screen guidance (no `beforeinstallprompt` on iOS) |
| Orientation | Portrait primary; landscape must not break layout (exam runner usable) |
| Accessibility | WCAG 2.2 AA (incl. 2.5.8 target size), platform comfort target 44 pt/48 dp — see [ADR-0002](adr/ADR-0002-touch-targets-44px.md) |

## 3. Current-State Audit (verified against code, 2026-11-17)

### 3.1 What already works

| Area | Evidence |
|---|---|
| Mobile-first single-column layout, sticky header, fixed bottom tab bar | `frontend/src/components/layouts/AppLayout.tsx` (`max-w-3xl`, bottom `nav` fixed) |
| Dynamic viewport height (no iOS URL-bar jump) | `AppLayout.tsx` uses `min-h-svh` |
| Fullscreen-friendly viewport | `frontend/index.html`: `width=device-width, initial-scale=1.0, viewport-fit=cover` |
| Installable PWA (Android criteria met) | `frontend/public/manifest.webmanifest`: standalone, portrait, `lang: id`, maskable 512 icon |
| Offline app shell + API network-first with 503 fallback | `frontend/public/sw.js` (app-shell precache, SWR static, network-first `/api/`) |
| Production-only SW registration | `frontend/src/main.tsx` |
| Route-level code splitting; main chunk 319 kB (102 kB gzip) | `frontend/src/app/router.tsx` (React.lazy), verified by `npm run build` |
| Exam ad suppression + consent gate | `frontend/src/features/ads/useIsExamActive.ts`, `AdSlot.tsx` |
| Correct soft-keyboard autocomplete on auth forms | `LoginPage.tsx`, `RegisterPage.tsx` (`autoComplete="email"`, `"current-password"`, `"new-password"`) |
| Accessible component primitives | shadcn/ui (Radix) in `src/components/ui/` |
| Exam runner essentials | `SessionRunnerPage.tsx`: countdown, question navigation, review marking, auto-submit on deadline, review dialog |

### 3.2 Gaps

| # | Gap | Severity | Evidence |
|---|---|---|---|
| G1 | **No safe-area handling.** `viewport-fit=cover` is set but no `env(safe-area-inset-*)` padding exists; in iOS standalone mode the bottom nav sits under the home indicator and content can hide behind the notch. | High | `AppLayout.tsx` — no `env()` anywhere in `src/` |
| G2 | **Touch targets below standard.** Default button is `h-9` (36 px); bottom-nav items ≈ 40 px tall; review-dialog grid buttons are `size="sm"`. Below Apple HIG 44 pt / Material 48 dp (WCAG 2.5.8 AA passes only marginally). | High | `src/components/ui/button.tsx` (`h-9`), `AppLayout.tsx` (`py-2`, `text-xs`) |
| G3 | **Exam state lost on refresh.** `totalQuestions` and `deadlineUtc` come from `location.state`; `answeredOrders`/`marked` are in-memory. A refresh (easy to trigger in standalone PWA) shows "Sesi tidak ditemukan" and loses marks. | High | `SessionRunnerPage.tsx` (`LocationState`, `useState(new Set())`) |
| G4 | **Failed answer submission is silent.** `handleSubmit` has `try/finally` without `catch`; a network blip swallows the error — the user gets no feedback and may believe the answer was recorded. | High | `SessionRunnerPage.tsx` `handleSubmit` |
| G5 | **No Kraepelin grid UI.** "Kraepelin" is selectable in `ModeSelectPage.tsx`, but the runner renders it as ordinary MCQ. Kraepelin requires a continuous arithmetic grid with numeric keypad, auto-advance, and per-column timing. | High | grep `Kraepelin` in `src/` — no runner support |
| G6 | **No SW update UX.** `sw.js` uses `skipWaiting` + `clients.claim()`; a stale tab can keep old chunks while `index.html`/lazy chunks update → runtime chunk-load errors after deploy. No "new version" prompt. | Medium | `public/sw.js`, `main.tsx` |
| G7 | **No install guidance.** Android gets the default mini-infobar only; iOS has no programmatic install prompt at all — Indonesian users (majority Android, significant iOS) get no invitation to install. | Medium | no install UI in `src/` |
| G8 | **Number inputs not mobile-tuned.** Question count uses `type="number"` (iOS shows spinner keys, no deterministic keypad). No `inputMode`/`enterKeyHint` anywhere. | Medium | `ModeSelectPage.tsx:159` |
| G9 | **No skeleton loading states.** Spinners everywhere; CLS-safe skeletons absent on dashboard/runner. | Medium | `DashboardPage.tsx`, `SessionRunnerPage.tsx` |
| G10 | **Admin pages not responsive-verified.** `GeneratorConfigsPage.tsx` (350 lines) renders tables/config forms designed on desktop; horizontal overflow likely at 360 px. | Medium | `GeneratorConfigsPage.tsx`, `UsersAdminPage.tsx` |
| G11 | **No back-button/exit guard during exams.** Browser/standalone back navigation mid-exam is unguarded; no `beforeunload` protection against accidental close. | Medium | `SessionRunnerPage.tsx` |
| G12 | **No performance/mobile CI gate.** No Lighthouse (mobile profile) run, no budgets enforced; bundle regression risk. | Medium | `.github/workflows/ci.yml` |
| G13 | **Micro-touch polish missing.** No `touch-action: manipulation` (double-tap zoom delay on rapid taps — fatal for Kraepelin), no `-webkit-tap-highlight-color` policy, `text-xs` (12 px) labels below comfortable readability. | Low | global CSS, `AppLayout.tsx` |
| G14 | **iOS-specific PWA metadata absent.** No `apple-mobile-web-app-status-bar-style`, no `display_override`; status-bar theming left to default. | Low | `index.html`, `manifest.webmanifest` |

## 4. Requirements

### 4.1 Functional requirements (mobile)

| ID | Requirement | ADR |
|---|---|---|
| FR-M1 | All interactive targets ≥ 44×44 CSS px (icon buttons, nav items, option rows, Kraepelin cells); spacing ≥ 8 px between adjacent targets. | 0002 |
| FR-M2 | Safe-area insets respected on all four edges; bottom nav extends its background through the home-indicator zone; content clears nav height. | 0003 |
| FR-M3 | Exam runner survives a full page refresh: session metadata + progress restored from `sessionStorage`; user resumes at the same question. | 0004 |
| FR-M4 | Failed answer submission is visible (inline error + retry); submission retried automatically on transient failure; answer never silently dropped. | 0004 |
| FR-M5 | Back navigation and tab close during a timed session trigger a confirmation; auto-submit on deadline unchanged. | 0004 |
| FR-M6 | Kraepelin sessions render a dedicated grid UI: numeric soft keypad (`inputMode="numeric"`), auto-advance to next cell, column timing, ≥ 44 px cells, `touch-action: manipulation`. | 0006 |
| FR-M7 | Service-worker updates surface a "Versi baru tersedia" toast; activation deferred until user confirms (no stale-chunk mismatch). | 0005 |
| FR-M8 | Install prompt on Android via `beforeinstallprompt`; iOS users see Add-to-Home-Screen instructions when eligible (Safari, not installed). | — |
| FR-M9 | All numeric inputs use `inputMode` + `enterKeyHint`; number spinners never shown on iOS. | — |
| FR-M10 | Loading states use CLS-safe skeletons on dashboard, runner, and result pages. | — |

### 4.2 Non-functional requirements

| ID | Requirement | ADR |
|---|---|---|
| NFR-M1 | Performance budgets on a Moto G-class device, throttled 4G: LCP ≤ 2.5 s, INP ≤ 200 ms, CLS ≤ 0.1, TBT ≤ 200 ms; main JS chunk ≤ 350 kB raw / 120 kB gzip. Enforced in CI. | 0007 |
| NFR-M2 | One responsive codebase; no user-agent sniffing for layout (feature detection only). | 0001 |
| NFR-M3 | `prefers-reduced-motion` honored for all non-essential animation. | — |
| NFR-M4 | Offline: app shell loads and shows an Indonesian offline notice; API 503 fallback preserved; no exam data fabricated client-side. | 0004 |
| NFR-M5 | Landscape must not clip content or trap scroll; exam runner usable in landscape. | — |
| NFR-M6 | Mobile Lighthouse (performance + a11y) score ≥ 90 on dashboard and exam runner, enforced in CI. | 0007 |

## 5. Standards & References

- WCAG 2.2 SC 2.5.8 Target Size (Minimum) — 24×24 CSS px AA; SC 2.5.5 — 44×44 AAA.
- Apple HIG: 44×44 pt minimum hit target; Material Design: 48×48 dp, 8 dp spacing.
- CSS `env(safe-area-inset-*)` requires `viewport-fit=cover` (already set); on iOS
  standalone, `dvh` resolves inside the safe area — compensate at the fixed-element
  level, not on `body`.
- iOS PWA: push notifications require iOS 16.4+ *and* standalone install; no
  `beforeinstallprompt`; ITP may evict storage after 7 days of non-use (do not treat
  client storage as durable).
- Core Web Vitals thresholds: LCP ≤ 2.5 s, INP ≤ 200 ms, CLS ≤ 0.1.

## 6. Traceability

Gaps → requirements → plan phases are cross-referenced in
[MOBILE_ROADMAP.md](MOBILE_ROADMAP.md) §"Coverage matrix".
