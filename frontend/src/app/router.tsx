import { createBrowserRouter } from "react-router-dom"
import { AppLayout } from "@/components/layouts/AppLayout"
import { PublicLayout } from "@/components/layouts/PublicLayout"
import { RequireAuth, RequireRole } from "@/app/guards"
import { LoginPage } from "@/features/auth/LoginPage"
import { RegisterPage } from "@/features/auth/RegisterPage"
import { DashboardPage } from "@/features/dashboard/DashboardPage"
import { ModeSelectPage } from "@/features/training/ModeSelectPage"
import { SessionResultPage } from "@/features/training/SessionResultPage"
import { SessionRunnerPage } from "@/features/training/SessionRunnerPage"
import { AdminPage } from "@/features/admin/AdminPage"
import { UsersAdminPage } from "@/features/admin/UsersAdminPage"
import { GeneratorConfigsPage } from "@/features/admin/GeneratorConfigsPage"
import { Roles } from "@/app/roles"

export const router = createBrowserRouter([
  {
    element: <PublicLayout />,
    children: [
      { path: "/login", element: <LoginPage /> },
      { path: "/register", element: <RegisterPage /> },
    ],
  },
  {
    element: <RequireAuth />,
    children: [
      {
        element: <AppLayout />,
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
