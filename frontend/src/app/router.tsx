import { createBrowserRouter } from "react-router-dom"
import { AppLayout } from "@/components/layouts/AppLayout"
import { PublicLayout } from "@/components/layouts/PublicLayout"
import { RequireAuth, RequireRole } from "@/app/guards"
import { LoginPage } from "@/features/auth/LoginPage"
import { RegisterPage } from "@/features/auth/RegisterPage"
import { DashboardPage } from "@/features/dashboard/DashboardPage"
import { AdminPage } from "@/features/admin/AdminPage"
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
          { path: "/latihan", element: <DashboardPage /> },
          {
            element: <RequireRole role={Roles.Administrator} />,
            children: [{ path: "/admin", element: <AdminPage /> }],
          },
        ],
      },
    ],
  },
])
