import { NavLink, Outlet, useNavigate } from "react-router-dom"
import { BarChart3, Home, LogOut, Settings } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/features/auth/auth-context"
import { AdSlot } from "@/features/ads/AdSlot"
import { ConsentBanner } from "@/features/ads/ConsentBanner"

const navItems = [
  { to: "/", label: "Beranda", icon: Home, end: true },
  { to: "/latihan", label: "Latihan", icon: BarChart3, end: false },
  { to: "/admin", label: "Admin", icon: Settings, end: false },
]

export function AppLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    navigate("/login", { replace: true })
  }

  return (
    <div className="flex min-h-svh flex-col bg-background">
      <header className="sticky top-0 z-10 border-b bg-background/95 backdrop-blur">
        <div className="mx-auto flex h-14 w-full max-w-3xl items-center justify-between px-4">
          <span className="text-sm font-semibold">Nalar</span>
          <div className="flex items-center gap-2">
            <span className="hidden text-xs text-muted-foreground sm:inline">{user?.email}</span>
            <Button variant="ghost" size="icon" aria-label="Keluar" onClick={handleLogout}>
              <LogOut className="size-4" />
            </Button>
          </div>
        </div>
      </header>

      <main className="mx-auto w-full max-w-3xl flex-1 px-4 pb-24 pt-4">
        <AdSlot placement="below-header" />
        <Outlet />
        <AdSlot placement="above-nav" />
      </main>

      <ConsentBanner />

      <nav className="fixed inset-x-0 bottom-0 z-10 border-t bg-background/95 backdrop-blur">
        <div className="mx-auto flex w-full max-w-3xl items-stretch justify-around">
          {navItems.map(({ to, label, icon: Icon, end }) => (
            <NavLink
              key={to}
              to={to}
              end={end}
              className={({ isActive }) =>
                `flex flex-1 flex-col items-center gap-1 py-2 text-xs ${
                  isActive ? "text-primary" : "text-muted-foreground"
                }`
              }
            >
              <Icon className="size-5" />
              {label}
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  )
}
