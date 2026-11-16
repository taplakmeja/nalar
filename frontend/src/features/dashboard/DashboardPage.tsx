import { BookOpen, Brain, ChartLine, Timer } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { useAuth } from "@/features/auth/auth-context"

const categories = [
  "Numerikal",
  "Verbal",
  "Logika",
  "Abstrak",
  "Spasial",
  "Kraepelin",
]

export function DashboardPage() {
  const { user } = useAuth()

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-xl font-semibold">
          Selamat datang, {user?.displayName || "Pengguna"}
        </h1>
        <p className="text-sm text-muted-foreground">
          Ringkasan latihan Anda akan tampil di sini.
        </p>
      </div>

      <div className="grid grid-cols-2 gap-3">
        <Card>
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-1">
              <Brain className="size-3" /> Latihan selesai
            </CardDescription>
            <CardTitle className="text-2xl">0</CardTitle>
          </CardHeader>
          <CardContent />
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardDescription className="flex items-center gap-1">
              <ChartLine className="size-3" /> Akurasi
            </CardDescription>
            <CardTitle className="text-2xl">–</CardTitle>
          </CardHeader>
          <CardContent />
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <Timer className="size-4" /> Tantangan Harian
          </CardTitle>
          <CardDescription>
            Tersedia setelah sistem latihan dirilis (Fase 4).
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Button disabled className="w-full">
            Segera hadir
          </Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <BookOpen className="size-4" /> Kategori Latihan
          </CardTitle>
          <CardDescription>Enam kategori akan tersedia.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-wrap gap-2">
          {categories.map((category) => (
            <Badge key={category} variant="secondary">
              {category}
            </Badge>
          ))}
        </CardContent>
      </Card>
    </div>
  )
}
