import { apiFetch } from "@/lib/api"
import type {
  AnswerFeedback,
  ExamProfileDto,
  QuestionDto,
  SessionResultDto,
  StartSessionRequest,
  StartSessionResponse,
} from "./types"

export function startSession(body: StartSessionRequest): Promise<StartSessionResponse> {
  return apiFetch("/sessions", { method: "POST", body: JSON.stringify(body) })
}

export function getSessionQuestion(sessionId: string, order: number): Promise<QuestionDto> {
  return apiFetch(`/sessions/${sessionId}/questions/${order}`)
}

export function submitAnswer(
  sessionId: string,
  questionId: string,
  answer: string,
): Promise<AnswerFeedback> {
  return apiFetch(`/sessions/${sessionId}/answers`, {
    method: "POST",
    body: JSON.stringify({ questionId, answer }),
  })
}

export function finishSession(sessionId: string): Promise<SessionResultDto> {
  return apiFetch(`/sessions/${sessionId}/finish`, { method: "POST" })
}

export function getSessionResult(sessionId: string): Promise<SessionResultDto> {
  return apiFetch(`/sessions/${sessionId}/result`)
}

export function listExamProfiles(): Promise<ExamProfileDto[]> {
  return apiFetch("/exam-profiles")
}
