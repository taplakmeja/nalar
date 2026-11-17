import { Suspense, lazy } from "react"
import { createBrowserRouter } from "react-router-dom"
import { AppLayout } from "@/components/layouts/AppLayout"
import { PublicLayout } from "@/components/layouts/PublicLayout"
import { RequireAuth, RequireRole } from "@/app/guards"
import { PageLoader } from "@/components/layouts/PageLoader"
import { Roles } from "@/app/roles"

const LoginPage = lazy(() =>
  import("@/features/auth/LoginPage").then((m) => ({ default: m.LoginPage })),
)
const RegisterPage = lazy(() =>
  import("@/features/auth/RegisterPage").then((m) => ({ default: m.RegisterPage })),
)
const DashboardPage = lazy(() =>
  import("@/features/dashboard/DashboardPage").then((m) => ({ default: m.DashboardPage })),
)
const ModeSelectPage = lazy(() =>
  import("@/features/training/ModeSelectPage").then((m) => ({ default: m.ModeSelectPage })),
)
const SessionResultPage = lazy(() =>
  import("@/features/training/SessionResultPage").then((m) => ({ default: m.SessionResultPage })),
)
const SessionRunnerPage = lazy(() =>
  import("@/features/training/SessionRunnerPage").then((m) => ({ default: m.SessionRunnerPage })),
)
const AdminPage = lazy(() =>
  import("@/features/admin/AdminPage").then((m) => ({ default: m.AdminPage })),
)
const UsersAdminPage = lazy(() =>
  import("@/features/admin/UsersAdminPage").then((m) => ({ default: m.UsersAdminPage })),
)
const GeneratorConfigsPage = lazy(() =>
  import("@/features/admin/GeneratorConfigsPage").then((m) => ({
    default: m.GeneratorConfigsPage,
  })),
)

export const router = createBrowserRouter([
  {
    element: (
      <Suspense fallback={<PageLoader />}>
        <PublicLayout />
      </Suspense>
    ),
    children: [
      { path: "/login", element: <LoginPage /> },
      { path: "/register", element: <RegisterPage /> },
    ],
  },
  {
    element: <RequireAuth />,
    children: [
      {
        element: (
          <Suspense fallback={<PageLoader />}>
            <AppLayout />
          </Suspense>
        ),
        children: [
          { path: "/", element: <DashboardPage /> },
          { path: "/latihan", element: <ModeSelectPage /> },
          { path: "/sesi/:sessionId", element: <SessionRunnerPage /> },
          { path: "/sesi/:sessionId/hasil", element: <SessionResultPage /> },
          {
            element: <RequireRole role={Roles.Administrator} />,
            children: [
              { path: "/admin", element: <AdminPage /> },
              { path: "/admin/pengguna", element: <UsersAdminPage /> },
            ],
          },
          {
            element: (
              <RequireRole role={[Roles.Administrator, Roles.ContentManager]} />
            ),
            children: [{ path: "/admin/generator", element: <GeneratorConfigsPage /> }],
          },
        ],
      },
    ],
  },
])
