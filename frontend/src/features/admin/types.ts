export interface AdminUser {
  id: string
  email: string
  displayName: string
  roles: string[]
  locked: boolean
}

export interface AppSetting {
  key: string
  value: string
  updatedAtUtc: string
}

export interface GeneratorConfigSummary {
  id: string
  generatorId: string
  version: number
  name: string
  category: string
  status: string
  createdAtUtc: string
  publishedAtUtc: string | null
  massTestPassed: boolean
}

export interface GeneratorConfig {
  id: string
  generatorId: string
  version: number
  name: string
  category: string
  status: string
  configJson: string
  createdAtUtc: string
  publishedAtUtc: string | null
  massTestJson: string | null
  reviewNotes: string | null
}

export interface MassTestReport {
  generatorId: string
  generatorVersion: number
  totalQuestions: number
  distinctQuestions: number
  duplicateRate: number
  invalidCount: number
  deterministic: boolean
  issues: string[]
  passed: boolean
}

export interface PreviewQuestion {
  difficulty: number
  seed: number
  prompt: string
  correctAnswer: string
  options: string[]
}

export interface AdminExamProfile {
  id: string
  slug: string
  name: string
  description: string
  categoryWeightsJson: string
  questionCount: number
  durationMinutes: number
  passingScorePercent: number
  isPublic: boolean
}
