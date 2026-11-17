import { useCallback, useEffect, useState } from "react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Separator } from "@/components/ui/separator"
import { Textarea } from "@/components/ui/textarea"
import {
  createGeneratorConfig,
  createGeneratorConfigVersion,
  getGeneratorConfig,
  listGeneratorConfigs,
  massTestGeneratorConfig,
  previewGeneratorConfig,
  publishGeneratorConfig,
  rollbackGeneratorConfig,
} from "./api"
import type {
  GeneratorConfig,
  GeneratorConfigSummary,
  MassTestReport,
  PreviewQuestion,
} from "./types"

const CATEGORIES = [
  "Numerical",
  "Verbal",
  "Logical",
  "Abstract",
  "Spatial",
  "Kraepelin",
] as const

const DIFFICULTY_LABELS = ["Beginner", "Easy", "Intermediate", "Advanced", "Expert"]

const DEFAULT_CONFIG = `{
  "promptTemplate": "Berapakah hasil dari {a} + {b}?",
  "parameters": [
    { "name": "a", "min": 1, "max": 20 },
    { "name": "b", "min": 1, "max": 20 }
  ],
  "operations": [
    { "name": "total", "expression": "a + b" }
  ],
  "answerExpression": "total",
  "distractorExpressions": ["total + 1", "total - 1", "total + 2"],
  "optionsCount": 4
}`

export function GeneratorConfigsPage() {
  const [configs, setConfigs] = useState<GeneratorConfigSummary[]>([])
  const [selected, setSelected] = useState<GeneratorConfig | null>(null)
  const [name, setName] = useState("")
  const [category, setCategory] = useState<string>("Numerical")
  const [configJson, setConfigJson] = useState(DEFAULT_CONFIG)
  const [previews, setPreviews] = useState<PreviewQuestion[]>([])
  const [report, setReport] = useState<MassTestReport | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  const load = useCallback(async () => {
    try {
      setConfigs(await listGeneratorConfigs())
    } catch {
      setError("Gagal memuat daftar generator.")
    }
  }, [])

  useEffect(() => {
    void load()
  }, [load])

  const runPreview = async () => {
    setBusy(true)
    setError(null)
    try {
      setPreviews(await previewGeneratorConfig({ configJson, category, samples: 2 }))
    } catch {
      setError("Pratinjau gagal. Periksa kembali konfigurasi.")
    } finally {
      setBusy(false)
    }
  }

  const handleCreate = async () => {
    setBusy(true)
    setError(null)
    try {
      await createGeneratorConfig({ name, category, configJson })
      setName("")
      setConfigJson(DEFAULT_CONFIG)
      await load()
    } catch {
      setError("Gagal membuat konfigurasi generator.")
    } finally {
      setBusy(false)
    }
  }

  const openDetail = async (id: string) => {
    setBusy(true)
    setError(null)
    try {
      setSelected(await getGeneratorConfig(id))
      setReport(null)
      setPreviews([])
    } catch {
      setError("Gagal memuat detail generator.")
    } finally {
      setBusy(false)
    }
  }

  const runMassTest = async (id: string) => {
    setBusy(true)
    setError(null)
    try {
      setReport(await massTestGeneratorConfig(id))
    } catch {
      setError("Mass test gagal dijalankan.")
    } finally {
      setBusy(false)
    }
  }

  const publish = async (id: string) => {
    setBusy(true)
    setError(null)
    try {
      setSelected(await publishGeneratorConfig(id))
      await load()
    } catch {
      setError("Publikasi gagal. Pastikan mass test sudah lulus.")
    } finally {
      setBusy(false)
    }
  }

  const rollback = async (generatorId: string) => {
    setBusy(true)
    setError(null)
    try {
      setSelected(await rollbackGeneratorConfig(generatorId))
      await load()
    } catch {
      setError("Rollback gagal. Tidak ada versi sebelumnya.")
    } finally {
      setBusy(false)
    }
  }

  const newVersion = async (id: string) => {
    setBusy(true)
    setError(null)
    try {
      setSelected(await createGeneratorConfigVersion(id, selected?.configJson ?? configJson))
      await load()
    } catch {
      setError("Gagal membuat versi baru.")
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-semibold">Generator Soal</h1>
      {error && <p className="text-sm text-destructive">{error}</p>}

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Konfigurasi Baru</CardTitle>
          <CardDescription>
            Definisikan parameter, operasi, jawaban, dan pengecoh secara deterministik.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          <div className="space-y-1.5">
            <Label htmlFor="generator-name">Nama</Label>
            <Input
              id="generator-name"
              value={name}
              onChange={(event) => setName(event.target.value)}
              placeholder="Penjumlahan Dasar"
            />
          </div>
          <div className="space-y-1.5">
            <Label>Kategori</Label>
            <Select value={category} onValueChange={setCategory}>
              <SelectTrigger className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {CATEGORIES.map((item) => (
                  <SelectItem key={item} value={item}>
                    {item}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="generator-config">Konfigurasi JSON</Label>
            <Textarea
              id="generator-config"
              value={configJson}
              onChange={(event) => setConfigJson(event.target.value)}
              rows={12}
              className="font-mono text-xs"
            />
          </div>
          <div className="flex gap-2">
            <Button variant="outline" disabled={busy || !configJson} onClick={() => void runPreview()}>
              Pratinjau
            </Button>
            <Button disabled={busy || !name || !configJson} onClick={() => void handleCreate()}>
              Simpan sebagai Draft
            </Button>
          </div>
        </CardContent>
      </Card>

      {previews.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Pratinjau Soal</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            {previews.map((question) => (
              <div key={`${question.difficulty}-${question.seed}`} className="rounded-md border p-3">
                <p className="text-xs text-muted-foreground">
                  {DIFFICULTY_LABELS[question.difficulty]} · seed {question.seed}
                </p>
                <p className="mt-1 text-sm">{question.prompt}</p>
                <p className="mt-1 text-sm font-medium">Jawaban: {question.correctAnswer}</p>
                <p className="text-xs text-muted-foreground">
                  Opsi: {question.options.join(" · ")}
                </p>
              </div>
            ))}
          </CardContent>
        </Card>
      )}

      <Separator />

      <div className="space-y-3">
        <h2 className="text-sm font-semibold text-muted-foreground">Daftar Konfigurasi</h2>
        {configs.map((config) => (
          <Card key={config.id}>
            <CardHeader>
              <CardTitle className="text-base">{config.name}</CardTitle>
              <CardDescription>
                {config.generatorId} · v{config.version} · {config.category}
              </CardDescription>
            </CardHeader>
            <CardContent className="flex flex-wrap items-center gap-2">
              <Badge variant={config.status === "Published" ? "default" : "secondary"}>
                {config.status}
              </Badge>
              {config.massTestPassed && <Badge variant="outline">Mass test lulus</Badge>}
              <Button variant="outline" size="sm" disabled={busy} onClick={() => void openDetail(config.id)}>
                Detail
              </Button>
            </CardContent>
          </Card>
        ))}
      </div>

      {selected && (
        <Card>
          <CardHeader>
            <CardTitle className="text-base">
              {selected.name} · v{selected.version}
            </CardTitle>
            <CardDescription>
              {selected.generatorId} · {selected.status}
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="flex flex-wrap gap-2">
              <Button
                variant="outline"
                size="sm"
                disabled={busy || selected.status !== "Draft"}
                onClick={() => void runMassTest(selected.id)}
              >
                Mass Test
              </Button>
              <Button
                size="sm"
                disabled={busy || selected.status !== "Draft"}
                onClick={() => void publish(selected.id)}
              >
                Publikasikan
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={busy || selected.status !== "Published"}
                onClick={() => void rollback(selected.generatorId)}
              >
                Rollback
              </Button>
              <Button
                variant="outline"
                size="sm"
                disabled={busy || selected.status === "Draft"}
                onClick={() => void newVersion(selected.id)}
              >
                Versi Baru
              </Button>
            </div>

            {report && (
              <div className="rounded-md border p-3 text-sm">
                <p className="font-medium">
                  {report.passed ? "Lulus" : "Tidak lulus"} · {report.distinctQuestions}/
                  {report.totalQuestions} unik · duplikat {report.duplicateRate}% · invalid{" "}
                  {report.invalidCount} · deterministik: {report.deterministic ? "ya" : "tidak"}
                </p>
                {report.issues.length > 0 && (
                  <ul className="mt-1 list-disc pl-5 text-xs text-muted-foreground">
                    {report.issues.map((issue) => (
                      <li key={issue}>{issue}</li>
                    ))}
                  </ul>
                )}
              </div>
            )}
          </CardContent>
        </Card>
      )}
    </div>
  )
}
