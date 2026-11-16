import { useCallback, useEffect, useRef, useState } from "react"
import { useLocation, useNavigate, useParams } from "react-router-dom"
import { CheckCircle2, ChevronLeft, ChevronRight, Flag, Loader2, XCircle } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { finishSession, getSessionQuestion, submitAnswer } from "./api"
import type { AnswerFeedback, QuestionDto } from "./types"

interface LocationState {
  totalQuestions: number
  deadlineUtc: string | null
}

function formatRemaining(ms: number): string {
  const total = Math.max(0, Math.floor(ms / 1000))
  const minutes = Math.floor(total / 60)
  const seconds = total % 60
  return `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`
}

export function SessionRunnerPage() {
  const { sessionId } = useParams<{ sessionId: string }>()
  const navigate = useNavigate()
  const location = useLocation()
  const state = (location.state ?? null) as LocationState | null

  const totalQuestions = state?.totalQuestions ?? 0
  const deadlineUtc = state?.deadlineUtc ?? null

  const [order, setOrder] = useState(1)
  const [question, setQuestion] = useState<QuestionDto | null>(null)
  const [selected, setSelected] = useState<string | null>(null)
  const [feedback, setFeedback] = useState<AnswerFeedback | null>(null)
  const [answeredOrders, setAnsweredOrders] = useState<Set<number>>(new Set())
  const [marked, setMarked] = useState<Set<number>>(new Set())
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [remainingMs, setRemainingMs] = useState<number | null>(null)
  const [reviewOpen, setReviewOpen] = useState(false)
  const finishingRef = useRef(false)

  const doFinish = useCallback(async () => {
    if (!sessionId || finishingRef.current) return
    finishingRef.current = true
    try {
      await finishSession(sessionId)
      navigate(`/sesi/${sessionId}/hasil`)
    } catch {
      finishingRef.current = false
    }
  }, [sessionId, navigate])

  useEffect(() => {
    if (!deadlineUtc) {
      setRemainingMs(null)
      return
    }
    const deadline = new Date(deadlineUtc).getTime()
    const tick = () => {
      const remaining = deadline - Date.now()
      setRemainingMs(remaining)
      if (remaining <= 0) {
        void doFinish()
      }
    }
    tick()
    const id = window.setInterval(tick, 1000)
    return () => window.clearInterval(id)
  }, [deadlineUtc, doFinish])

  useEffect(() => {
    if (!sessionId) return
    let cancelled = false
    setLoading(true)
    setQuestion(null)
    setSelected(null)
    setFeedback(null)
    getSessionQuestion(sessionId, order)
      .then((q) => {
        if (!cancelled) setQuestion(q)
      })
      .catch(() => {
        if (!cancelled) setQuestion(null)
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [sessionId, order])

  async function handleSubmit() {
    if (!sessionId || !question || !selected || submitting) return
    setSubmitting(true)
    try {
      const result = await submitAnswer(sessionId, question.questionId, selected)
      setFeedback(result)
      setAnsweredOrders((prev) => new Set(prev).add(order))
    } finally {
      setSubmitting(false)
    }
  }

  function goTo(newOrder: number) {
    if (newOrder < 1 || newOrder > totalQuestions) return
    setOrder(newOrder)
    setReviewOpen(false)
  }

  function toggleMark() {
    setMarked((prev) => {
      const next = new Set(prev)
      if (next.has(order)) {
        next.delete(order)
      } else {
        next.add(order)
      }
      return next
    })
  }

  if (!totalQuestions) {
    return (
      <div className="space-y-4 text-center">
        <p className="text-sm text-muted-foreground">Sesi tidak ditemukan. Mulai latihan dari halaman mode.</p>
        <Button onClick={() => navigate("/latihan")}>Pilih Mode</Button>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <span className="text-sm font-medium">
          Soal {order} / {totalQuestions}
        </span>
        {remainingMs !== null ? (
          <span className={remainingMs < 60_000 ? "text-sm font-semibold text-destructive" : "text-sm font-semibold"}>
            {formatRemaining(remainingMs)}
          </span>
        ) : (
          <Button variant="ghost" size="sm" onClick={toggleMark}>
            <Flag className={marked.has(order) ? "fill-primary" : undefined} />
            Tandai
          </Button>
        )}
      </div>

      <Card>
        <CardContent className="space-y-4 p-4">
          {loading ? (
            <div className="flex justify-center py-10">
              <Loader2 className="animate-spin text-muted-foreground" />
            </div>
          ) : question ? (
            <>
              {question.svg && (
                <div
                  className="mx-auto max-w-xs [&>svg]:h-auto [&>svg]:w-full"
                  dangerouslySetInnerHTML={{ __html: question.svg }}
                />
              )}
              <p className="text-sm leading-relaxed">{question.prompt}</p>
              <div className="space-y-2">
                {question.options.map((option) => {
                  const isSelected = selected === option
                  const isCorrectOption = feedback?.correctAnswer === option
                  const showState = feedback !== null
                  return (
                    <Button
                      key={option}
                      type="button"
                      variant="outline"
                      disabled={feedback !== null}
                      className={
                        "w-full justify-start " +
                        (showState && isCorrectOption
                          ? "border-emerald-500 bg-emerald-500/10 text-foreground"
                          : showState && isSelected
                            ? "border-red-500 bg-red-500/10 text-foreground"
                            : isSelected
                              ? "border-primary"
                              : "")
                      }
                      onClick={() => setSelected(option)}
                    >
                      {option}
                    </Button>
                  )
                })}
              </div>
              {!feedback && selected && (
                <Button className="w-full" onClick={() => void handleSubmit()} disabled={submitting}>
                  {submitting && <Loader2 className="animate-spin" />}
                  Jawab
                </Button>
              )}
              {feedback && (
                <div className="flex items-center gap-2 text-sm">
                  {feedback.isCorrect ? (
                    <CheckCircle2 className="size-4 text-emerald-500" />
                  ) : (
                    <XCircle className="size-4 text-red-500" />
                  )}
                  <span>
                    {feedback.isCorrect ? "Benar" : `Salah. Jawaban benar: ${feedback.correctAnswer}`}
                  </span>
                </div>
              )}
            </>
          ) : (
            <p className="py-10 text-center text-sm text-muted-foreground">
              Soal tidak dapat dimuat.
            </p>
          )}
        </CardContent>
      </Card>

      <div className="grid grid-cols-2 gap-2">
        <Button variant="outline" onClick={() => goTo(order - 1)} disabled={order <= 1}>
          <ChevronLeft /> Sebelumnya
        </Button>
        {order < totalQuestions ? (
          <Button onClick={() => goTo(order + 1)}>
            Berikutnya <ChevronRight />
          </Button>
        ) : (
          <Button onClick={() => void doFinish()} disabled={finishingRef.current}>
            Selesai
          </Button>
        )}
      </div>

      {feedback && order < totalQuestions && (
        <Button className="w-full" onClick={() => goTo(order + 1)}>
          Soal Berikutnya <ChevronRight />
        </Button>
      )}

      <Button variant="ghost" className="w-full" onClick={() => setReviewOpen(true)}>
        Navigasi Soal
      </Button>

      <Dialog open={reviewOpen} onOpenChange={setReviewOpen}>
        <DialogContent className="max-w-xs">
          <DialogHeader>
            <DialogTitle>Navigasi Soal</DialogTitle>
          </DialogHeader>
          <div className="grid grid-cols-5 gap-2">
            {Array.from({ length: totalQuestions }, (_, i) => i + 1).map((n) => (
              <Button
                key={n}
                variant={n === order ? "default" : answeredOrders.has(n) ? "secondary" : "outline"}
                size="sm"
                className="relative"
                onClick={() => goTo(n)}
              >
                {n}
                {marked.has(n) && (
                  <span className="absolute right-1 top-1 size-1.5 rounded-full bg-amber-500" />
                )}
              </Button>
            ))}
          </div>
          <Button onClick={() => void doFinish()} disabled={finishingRef.current}>
            Kumpulkan Sekarang
          </Button>
        </DialogContent>
      </Dialog>
    </div>
  )
}
