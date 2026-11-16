export interface CategoryStatDto {
  category: string
  answered: number
  correct: number
  accuracyPercent: number
  consecutiveCorrect: number
  consecutiveIncorrect: number
}

export interface DifficultyStatDto {
  difficulty: number
  answered: number
  correct: number
  accuracyPercent: number
}

export interface SessionSummaryDto {
  sessionId: string
  mode: string
  status: string
  totalQuestions: number
  correctCount: number
  incorrectCount: number
  unansweredCount: number
  score: number
  completedAtUtc: string | null
}

export interface RecommendationDto {
  category: string
  message: string
}

export interface ReadinessDto {
  percent: number
  label: string
  disclaimer: string
}

export interface DashboardDto {
  sessionsCompleted: number
  totalAnswered: number
  accuracyPercent: number
  averageAnswerSeconds: number
  categories: CategoryStatDto[]
  difficulties: DifficultyStatDto[]
  recentSessions: SessionSummaryDto[]
  recommendations: RecommendationDto[]
  readiness: ReadinessDto
}
