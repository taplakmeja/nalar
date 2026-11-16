import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { Loader2 } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { startSession } from "./api"
import type { CategoryScope, TrainingMode } from "./types"

const modes: { value: TrainingMode; label: string; description: string }[] = [
  { value: "Practice", label: "Latihan Bebas", description: "Tanpa batas waktu, langsung lihat pembahasan singkat." },
  { value: "TimedTest", label: "Latihan Terbatas", description: "Dengan hitungan waktu menekan." },
  { value: "ExamSimulation", label: "Simulasi Ujian", description: "Format ujian penuh dengan navigasi soal." },
  { value: "DailyChallenge", label: "Tantangan Harian", description: "Set soal singkat setiap hari." },
  { value: "WeaknessTraining", label: "Latihan Kelemahan", description: "Fokus pada kategori terlemah Anda." },
  { value: "Survival", label: "Survival", description: "Jawab terus sampai waktu habis." },
]

const categories: { value: CategoryScope; label: string }[] = [
  { value: "Mixed", label: "Campuran" },
  { value: "Numerical", label: "Numerikal" },
  { value: "Verbal", label: "Verbal" },
  { value: "Logical", label: "Logika" },
  { value: "Abstract", label: "Abstrak" },
  { value: "Spatial", label: "Spasial" },
  { value: "Kraepelin", label: "Kraepelin" },
]

const difficulties = ["Pemula", "Mudah", "Sedang", "Sulit", "Ahli"]

export function ModeSelectPage() {
  const navigate = useNavigate()
  const [mode, setMode] = useState<TrainingMode>("Practice")
  const [categoryScope, setCategoryScope] = useState<CategoryScope>("Mixed")
  const [difficulty, setDifficulty] = useState(1)
  const [questionCount, setQuestionCount] = useState(10)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleStart() {
    setLoading(true)
    setError(null)
    try {
      const session = await startSession({
        mode,
        categoryScope,
        difficulty,
        questionCount,
        examProfileSlug: null,
      })
      navigate(`/sesi/${session.sessionId}`, { state: session })
    } catch (err) {
      setError(err instanceof Error ? err.message : "Gagal memulai sesi.")
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-xl font-semibold">Mulai Latihan</h1>
        <p className="text-sm text-muted-foreground">
          Pilih mode, kategori, dan tingkat kesulitan.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Mode Latihan</CardTitle>
        </CardHeader>
        <CardContent>
          <RadioGroup value={mode} onValueChange={(v) => setMode(v as TrainingMode)} className="gap-2">
            {modes.map((m) => (
              <Label
                key={m.value}
                htmlFor={m.value}
                className="flex cursor-pointer items-start gap-3 rounded-lg border p-3 has-[[data-state=checked]]:border-primary"
              >
                <RadioGroupItem value={m.value} id={m.value} className="mt-0.5" />
                <span className="space-y-0.5">
                  <span className="block text-sm font-medium">{m.label}</span>
                  <span className="block text-xs text-muted-foreground">{m.description}</span>
                </span>
              </Label>
            ))}
          </RadioGroup>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Kategori</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-2 gap-2">
          {categories.map((c) => (
            <Button
              key={c.value}
              type="button"
              variant={categoryScope === c.value ? "default" : "outline"}
              size="sm"
              onClick={() => setCategoryScope(c.value)}
            >
              {c.label}
            </Button>
          ))}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Tingkat Kesulitan</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-5 gap-2">
          {difficulties.map((d, i) => (
            <Button
              key={d}
              type="button"
              variant={difficulty === i ? "default" : "outline"}
              size="sm"
              onClick={() => setDifficulty(i)}
            >
              {d}
            </Button>
          ))}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Jumlah Soal</CardTitle>
        </CardHeader>
        <CardContent>
          <Input
            type="number"
            min={1}
            max={100}
            value={questionCount}
            onChange={(e) => setQuestionCount(Number(e.target.value))}
          />
        </CardContent>
      </Card>

      {error && <p className="text-sm text-destructive">{error}</p>}

      <Button className="w-full" size="lg" onClick={handleStart} disabled={loading}>
        {loading && <Loader2 className="animate-spin" />}
        Mulai
      </Button>
    </div>
  )
}
