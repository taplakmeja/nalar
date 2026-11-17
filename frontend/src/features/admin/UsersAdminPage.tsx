import { useCallback, useEffect, useState } from "react"
import { Badge } from "@/components/ui/badge"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Switch } from "@/components/ui/switch"
import { listUsers, setUserLockout, setUserRoles } from "./api"
import type { AdminUser } from "./types"

const ALL_ROLES = ["User", "ContentManager", "Administrator"] as const

export function UsersAdminPage() {
  const [users, setUsers] = useState<AdminUser[]>([])
  const [error, setError] = useState<string | null>(null)
  const [busyId, setBusyId] = useState<string | null>(null)

  const load = useCallback(async () => {
    try {
      setUsers(await listUsers())
    } catch {
      setError("Gagal memuat daftar pengguna.")
    }
  }, [])

  useEffect(() => {
    void load()
  }, [load])

  const toggleRole = async (user: AdminUser, role: string, checked: boolean) => {
    setBusyId(user.id)
    setError(null)
    try {
      const roles = checked
        ? [...user.roles, role]
        : user.roles.filter((r) => r !== role)
      await setUserRoles(user.id, roles)
      await load()
    } catch {
      setError("Gagal mengubah peran pengguna.")
    } finally {
      setBusyId(null)
    }
  }

  const toggleLock = async (user: AdminUser, locked: boolean) => {
    setBusyId(user.id)
    setError(null)
    try {
      await setUserLockout(user.id, locked)
      await load()
    } catch {
      setError("Gagal mengubah status blokir.")
    } finally {
      setBusyId(null)
    }
  }

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-semibold">Pengguna &amp; Peran</h1>
      {error && <p className="text-sm text-destructive">{error}</p>}

      {users.map((user) => (
        <Card key={user.id}>
          <CardHeader>
            <CardTitle className="text-base break-all">{user.email}</CardTitle>
            <CardDescription>{user.displayName}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="flex flex-wrap gap-2">
              {ALL_ROLES.map((role) => (
                <label
                  key={role}
                  className="flex items-center gap-2 rounded-md border px-3 py-1.5 text-sm"
                >
                  <Switch
                    checked={user.roles.includes(role)}
                    disabled={busyId === user.id}
                    onCheckedChange={(checked) => void toggleRole(user, role, checked)}
                  />
                  {role}
                </label>
              ))}
            </div>
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <span className="text-sm text-muted-foreground">Blokir akun</span>
                {user.locked && <Badge variant="destructive">Terblokir</Badge>}
              </div>
              <Switch
                checked={user.locked}
                disabled={busyId === user.id}
                onCheckedChange={(checked) => void toggleLock(user, checked)}
              />
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  )
}
