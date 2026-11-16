import { useNavigate } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

export function AdminPage() {
  const navigate = useNavigate()

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-semibold">Administrasi</h1>
      <p className="text-sm text-muted-foreground">
        Panel administrasi lengkap tersedia pada Fase 6.
      </p>

      <div className="grid gap-3 sm:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Pengguna & Peran</CardTitle>
            <CardDescription>Kelola akun dan peran (Fase 6).</CardDescription>
          </CardHeader>
          <CardContent>
            <Button variant="outline" disabled className="w-full">
              Segera hadir
            </Button>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Generator Soal</CardTitle>
            <CardDescription>
              Konfigurasi, pratinjau, dan publikasi generator (Fase 6).
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button variant="outline" disabled className="w-full">
              Segera hadir
            </Button>
          </CardContent>
        </Card>
      </div>

      <Button variant="ghost" onClick={() => navigate("/")}>
        Kembali ke beranda
      </Button>
    </div>
  )
}
