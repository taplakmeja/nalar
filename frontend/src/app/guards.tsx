import { Navigate, Outlet, useLocation } from "react-router-dom"
import { useAuth } from "@/features/auth/auth-context"

export function RequireAuth() {
  const { user, isLoading } = useAuth()
  const location = useLocation()

  if (isLoading) {
    return (
      <div className="flex min-h-svh items-center justify-center">
        <p className="text-muted-foreground">Memuat…</p>
      </div>
    )
  }

  if (!user) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  return <Outlet />
}

export function RequireRole({ role }: { role: string | string[] }) {
  const { hasRole, isLoading } = useAuth()

  if (isLoading) {
    return (
      <div className="flex min-h-svh items-center justify-center">
        <p className="text-muted-foreground">Memuat…</p>
      </div>
    )
  }

  const roles = Array.isArray(role) ? role : [role]
  const allowed = roles.some((item) => hasRole(item))

  if (!allowed) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}
