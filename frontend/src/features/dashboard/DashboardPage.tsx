import { useEffect, useState } from "react"
import { Link } from "react-router-dom"
import {
  Brain,
  ChartLine,
  Clock,
  Lightbulb,
  Loader2,
  Target,
  TrendingUp,
} from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Progress } from "@/components/ui/progress"
import { useAuth } from "@/features/auth/auth-context"
import { getDashboard } from "./api"
import type { DashboardDto } from "./types"

const difficultyLabels = ["Pemula", "Mudah", "Sedang", "Sulit", "Ahli"]

const modeLabels: Record<string, string> = {
  Practice: "Latihan Bebas",
  TimedTest: "Latihan Terbatas",
  ExamSimulation: "Simulasi Ujian",
  DailyChallenge: "Tantangan Harian",
  WeaknessTraining: "Latihan Kelemahan",
  Survival: "Survival",
}

function accuracyColor(accuracy: number): string {
  if (accuracy >= 80) return "bg-emerald-500"
  if (accuracy >= 60) return "bg-amber-500"
  return "bg-red-500"
}

export function DashboardPage() {
  const { user } = useAuth()
  const [dashboard, setDashboard] = useState<DashboardDto | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    getDashboard()
      .then((data) => {
        if (!cancelled) setDashboard(data)
      })
      .catch(() => {
        if (!cancelled) setDashboard(null)
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [])

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-xl font-semibold">
          Selamat datang, {user?.displayName || "Pengguna"}
        </h1>
        <p className="text-sm text-muted-foreground">
          Ringkasan performa latihan Anda.
        </p>
      </div>

      {loading ? (
        <div className="flex justify-center py-16">
          <Loader2 className="animate-spin text-muted-foreground" />
        </div>
      ) : dashboard ? (
        <>
          <div className="grid grid-cols-2 gap-3">
            <Card>
              <CardHeader className="pb-2">
                <CardDescription className="flex items-center gap-1">
                  <Brain className="size-3" /> Sesi selesai
                </CardDescription>
                <CardTitle className="text-2xl tabular-nums">
                  {dashboard.sessionsCompleted}
                </CardTitle>
              </CardHeader>
              <CardContent />
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription className="flex items-center gap-1">
                  <Target className="size-3" /> Akurasi
                </CardDescription>
                <CardTitle className="text-2xl tabular-nums">
                  {dashboard.accuracyPercent}%
                </CardTitle>
              </CardHeader>
              <CardContent />
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription className="flex items-center gap-1">
                  <ChartLine className="size-3" /> Soal dijawab
                </CardDescription>
                <CardTitle className="text-2xl tabular-nums">
                  {dashboard.totalAnswered}
                </CardTitle>
              </CardHeader>
              <CardContent />
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardDescription className="flex items-center gap-1">
                  <Clock className="size-3" /> Rata-rata / soal
                </CardDescription>
                <CardTitle className="text-2xl tabular-nums">
                  {dashboard.averageAnswerSeconds}s
                </CardTitle>
              </CardHeader>
              <CardContent />
            </Card>
          </div>

          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2 text-base">
                <TrendingUp className="size-4" /> Estimasi Kesiapan
              </CardTitle>
              <CardDescription>{dashboard.readiness.disclaimer}</CardDescription>
            </CardHeader>
            <CardContent className="space-y-2">
              <div className="flex items-center justify-between">
                <Badge variant={dashboard.readiness.percent >= 80 ? "default" : "secondary"}>
                  {dashboard.readiness.label}
                </Badge>
                <span className="text-2xl font-semibold tabular-nums">
                  {dashboard.readiness.percent}%
                </span>
              </div>
              <Progress value={dashboard.readiness.percent} />
            </CardContent>
          </Card>

          {dashboard.categories.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Performa per Kategori</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                {dashboard.categories.map((c) => (
                  <div key={c.category} className="space-y-1">
                    <div className="flex items-center justify-between text-sm">
                      <span>{c.category}</span>
                      <span className="text-xs text-muted-foreground">
                        {c.correct}/{c.answered} · {c.accuracyPercent}%
                      </span>
                    </div>
                    <div className="h-2 w-full overflow-hidden rounded-full bg-muted">
                      <div
                        className={`h-full rounded-full ${accuracyColor(c.accuracyPercent)}`}
                        style={{ width: `${Math.min(100, c.accuracyPercent)}%` }}
                      />
                    </div>
                  </div>
                ))}
              </CardContent>
            </Card>
          )}

          {dashboard.difficulties.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Performa per Kesulitan</CardTitle>
              </CardHeader>
              <CardContent className="grid grid-cols-5 gap-2 text-center text-xs">
                {dashboard.difficulties.map((d) => (
                  <div key={d.difficulty} className="space-y-1">
                    <div className="font-semibold tabular-nums">
                      {d.accuracyPercent}%
                    </div>
                    <div className="text-muted-foreground">
                      {difficultyLabels[d.difficulty] ?? d.difficulty}
                    </div>
                  </div>
                ))}
              </CardContent>
            </Card>
          )}

          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2 text-base">
                <Lightbulb className="size-4" /> Rekomendasi
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-2">
              {dashboard.recommendations.map((r, i) => (
                <div key={i} className="rounded-lg border p-3 text-sm">
                  <Badge variant="outline" className="mb-1">
                    {r.category}
                  </Badge>
                  <p>{r.message}</p>
                </div>
              ))}
            </CardContent>
          </Card>

          {dashboard.recentSessions.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="text-base">Riwayat Latihan</CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                {dashboard.recentSessions.map((s) => (
                  <Link
                    key={s.sessionId}
                    to={`/sesi/${s.sessionId}/hasil`}
                    className="flex items-center justify-between rounded-lg border p-3 text-sm hover:bg-muted/50"
                  >
                    <span>
                      <span className="block font-medium">
                        {modeLabels[s.mode] ?? s.mode}
                      </span>
                      <span className="text-xs text-muted-foreground">
                        {s.correctCount}/{s.totalQuestions} benar
                        {s.completedAtUtc
                          ? ` · ${new Date(s.completedAtUtc).toLocaleDateString("id-ID")}`
                          : ""}
                      </span>
                    </span>
                    <span className="font-semibold tabular-nums">{s.score}</span>
                  </Link>
                ))}
              </CardContent>
            </Card>
          )}
        </>
      ) : (
        <p className="text-sm text-muted-foreground">
          Gagal memuat ringkasan. Coba muat ulang halaman.
        </p>
      )}

      <Button asChild className="w-full" size="lg">
        <Link to="/latihan">Mulai Latihan</Link>
      </Button>
    </div>
  )
}
