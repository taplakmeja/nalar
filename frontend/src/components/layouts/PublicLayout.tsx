import { Outlet } from "react-router-dom"

export function PublicLayout() {
  return (
    <div className="flex min-h-svh flex-col items-center justify-center bg-muted/40 p-4">
      <div className="w-full max-w-sm">
        <Outlet />
      </div>
    </div>
  )
}
