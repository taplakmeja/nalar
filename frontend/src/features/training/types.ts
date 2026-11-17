export type TrainingMode =
  | "Practice"
  | "TimedTest"
  | "ExamSimulation"
  | "DailyChallenge"
  | "WeaknessTraining"
  | "Survival"

export type CategoryScope =
  | "Mixed"
  | "Numerical"
  | "Verbal"
  | "Logical"
  | "Abstract"
  | "Spatial"
  | "Kraepelin"

export interface StartSessionRequest {
  mode: TrainingMode
  categoryScope: CategoryScope
  difficulty: number
  questionCount: number
  examProfileSlug?: string | null
  useAdaptiveDifficulty?: boolean
}

export interface StartSessionResponse {
  sessionId: string
  mode: TrainingMode
  categoryScope: CategoryScope
  totalQuestions: number
  deadlineUtc: string | null
}

export interface QuestionDto {
  questionId: string
  order: number
  category: string
  difficulty: number
  prompt: string
  options: string[]
  svg: string | null
}

export interface AnswerFeedback {
  isCorrect: boolean
  correctAnswer: string
}

export interface SessionQuestionResultDto {
  order: number
  category: string
  generatorId: string
  difficulty: number
  isCorrect: boolean
  submittedAnswer: string | null
  correctAnswer: string
}

export interface SessionResultDto {
  sessionId: string
  mode: TrainingMode
  status: string
  totalQuestions: number
  correctCount: number
  incorrectCount: number
  unansweredCount: number
  score: number
  isPassed: boolean
  deadlineUtc: string | null
  questions: SessionQuestionResultDto[]
}

export interface ExamProfileDto {
  id: string
  slug: string
  name: string
  description: string
  questionCount: number
  durationMinutes: number
  passingScorePercent: number
}
