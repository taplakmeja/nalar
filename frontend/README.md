# Frontend — Nalar Web

React + TypeScript SPA for the Nalar psikotes training platform. Mobile-first.

## Stack

- Vite + React 19 + TypeScript
- Tailwind CSS 3.4 with shadcn/ui (new-york style, full component set in `src/components/ui`)
- React Router (`createBrowserRouter`, nested layout routes, no switch-case routing)
- Vitest + React Testing Library + jsdom

## Structure

```
src/
  app/            application wiring: router, route guards, role constants
  components/
    layouts/      page shells (public, authenticated app shell)
    ui/           shadcn/ui components (generated, do not hand-edit)
  features/       feature-first modules
    auth/         auth context (JWT + refresh), login, register
    dashboard/    learning dashboard
    admin/        administration (Phase 6)
  lib/            api client (token storage, refresh-on-401), cn() helper
  test/           test setup
```

## Commands

```bash
npm install        # install dependencies
npm run dev        # dev server on :5173, /api proxied to :5000
npm test           # vitest run
npm run lint       # oxlint
npm run build      # type-check + production build to dist/
```

## shadcn/ui

Configured via `components.json` with the `@/*` path alias (`vite.config.ts`,
`tsconfig.json`). Add components with:

```bash
npx shadcn@latest add <component>
```

Components in `src/components/ui` are generated code — customize by wrapping,
not by editing the generated files.

## Conventions

- UI text is Bahasa Indonesia.
- Routes are guarded by `RequireAuth` / `RequireRole` (`src/app/guards.tsx`).
- API access goes through `apiFetch` (`src/lib/api.ts`) — never call `fetch`
  directly from components; scoring is validated server-side only.
