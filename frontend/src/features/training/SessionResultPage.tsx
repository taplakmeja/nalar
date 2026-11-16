import { useEffect, useState } from "react"
import { Link, useParams } from "react-router-dom"
import { CheckCircle2, Loader2, MinusCircle, XCircle } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { getSessionResult } from "./api"
import type { SessionResultDto } from "./types"

export function SessionResultPage() {
  const { sessionId } = useParams<{ sessionId: string }>()
  const [result, setResult] = useState<SessionResultDto | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!sessionId) return
    let cancelled = false
    getSessionResult(sessionId)
      .then((r) => {
        if (!cancelled) setResult(r)
      })
      .catch((err) => {
        if (!cancelled) setError(err instanceof Error ? err.message : "Gagal memuat hasil.")
      })
    return () => {
      cancelled = true
    }
  }, [sessionId])

  if (error) {
    return (
      <div className="space-y-4 text-center">
        <p className="text-sm text-destructive">{error}</p>
        <Button asChild>
          <Link to="/latihan">Kembali ke Latihan</Link>
        </Button>
      </div>
    )
  }

  if (!result) {
    return (
      <div className="flex justify-center py-16">
        <Loader2 className="animate-spin text-muted-foreground" />
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <Card>
        <CardHeader className="items-center text-center">
          <CardTitle className="text-3xl tabular-nums">{result.score}</CardTitle>
          <CardDescription>
            {result.isPassed ? "Selamat, Anda lulus!" : "Belum mencapai batas kelulusan."}
          </CardDescription>
        </CardHeader>
        <CardContent className="grid grid-cols-2 gap-3 text-sm">
          <div className="flex items-center gap-2">
            <CheckCircle2 className="size-4 text-emerald-500" /> Benar: {result.correctCount}
          </div>
          <div className="flex items-center gap-2">
            <XCircle className="size-4 text-red-500" /> Salah: {result.incorrectCount}
          </div>
          <div className="flex items-center gap-2">
            <MinusCircle className="size-4 text-muted-foreground" /> Kosong: {result.unansweredCount}
          </div>
          <div className="flex items-center gap-2">
            Total: {result.totalQuestions}
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Rincian Jawaban</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          {result.questions.map((q) => (
            <div key={q.order} className="flex items-center justify-between gap-2 text-sm">
              <span className="flex items-center gap-2">
                {q.submittedAnswer === null ? (
                  <MinusCircle className="size-4 text-muted-foreground" />
                ) : q.isCorrect ? (
                  <CheckCircle2 className="size-4 text-emerald-500" />
                ) : (
                  <XCircle className="size-4 text-red-500" />
                )}
                {q.order}. {q.category}
              </span>
              <span className="text-xs text-muted-foreground">
                {q.submittedAnswer === null ? "—" : q.submittedAnswer}
                {q.submittedAnswer !== null && !q.isCorrect ? ` → ${q.correctAnswer}` : ""}
              </span>
            </div>
          ))}
        </CardContent>
      </Card>

      <Button asChild className="w-full">
        <Link to="/latihan">Latihan Lagi</Link>
      </Button>
    </div>
  )
}
