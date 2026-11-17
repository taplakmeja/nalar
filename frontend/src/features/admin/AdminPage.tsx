import { useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { useAuth } from "@/features/auth/auth-context"
import { Roles } from "@/app/roles"

export function AdminPage() {
  const navigate = useNavigate()
  const { hasRole } = useAuth()

  const canManageGenerators = hasRole(Roles.Administrator) || hasRole(Roles.ContentManager)

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-semibold">Administrasi</h1>
      <p className="text-sm text-muted-foreground">
        Kelola pengguna, peran, dan generator soal.
      </p>

      <div className="grid gap-3 sm:grid-cols-2">
        {hasRole(Roles.Administrator) && (
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Pengguna &amp; Peran</CardTitle>
              <CardDescription>Kelola akun, peran, dan blokir pengguna.</CardDescription>
            </CardHeader>
            <CardContent>
              <Button className="w-full" onClick={() => navigate("/admin/pengguna")}>
                Buka
              </Button>
            </CardContent>
          </Card>
        )}

        {canManageGenerators && (
          <Card>
            <CardHeader>
              <CardTitle className="text-base">Generator Soal</CardTitle>
              <CardDescription>
                Konfigurasi, pratinjau, mass test, publikasi, dan rollback.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Button className="w-full" onClick={() => navigate("/admin/generator")}>
                Buka
              </Button>
            </CardContent>
          </Card>
        )}
      </div>

      <Button variant="ghost" onClick={() => navigate("/")}>
        Kembali ke beranda
      </Button>
    </div>
  )
}
