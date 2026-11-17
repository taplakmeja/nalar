import { apiFetch } from "@/lib/api"
import type {
  AdminExamProfile,
  AdminUser,
  AppSetting,
  GeneratorConfig,
  GeneratorConfigSummary,
  MassTestReport,
  PreviewQuestion,
} from "./types"

export function listUsers(): Promise<AdminUser[]> {
  return apiFetch("/admin/users")
}

export function setUserRoles(userId: string, roles: string[]): Promise<void> {
  return apiFetch(`/admin/users/${userId}/roles`, {
    method: "PUT",
    body: JSON.stringify({ roles }),
  })
}

export function setUserLockout(userId: string, locked: boolean): Promise<void> {
  return apiFetch(`/admin/users/${userId}/lockout`, {
    method: "PUT",
    body: JSON.stringify({ locked }),
  })
}

export function getSettings(): Promise<AppSetting[]> {
  return apiFetch("/admin/settings")
}

export function updateSetting(key: string, value: string): Promise<AppSetting> {
  return apiFetch(`/admin/settings/${encodeURIComponent(key)}`, {
    method: "PUT",
    body: JSON.stringify({ value }),
  })
}

export function listGeneratorConfigs(): Promise<GeneratorConfigSummary[]> {
  return apiFetch("/generator-configs")
}

export function getGeneratorConfig(id: string): Promise<GeneratorConfig> {
  return apiFetch(`/generator-configs/${id}`)
}

export function createGeneratorConfig(body: {
  name: string
  category: string
  configJson: string
}): Promise<GeneratorConfig> {
  return apiFetch("/generator-configs", { method: "POST", body: JSON.stringify(body) })
}

export function createGeneratorConfigVersion(id: string, configJson: string): Promise<GeneratorConfig> {
  return apiFetch(`/generator-configs/${id}/versions`, {
    method: "POST",
    body: JSON.stringify({ configJson }),
  })
}

export function deleteGeneratorConfig(id: string): Promise<void> {
  return apiFetch(`/generator-configs/${id}`, { method: "DELETE" })
}

export function massTestGeneratorConfig(id: string): Promise<MassTestReport> {
  return apiFetch(`/generator-configs/${id}/mass-test`, { method: "POST" })
}

export function publishGeneratorConfig(id: string): Promise<GeneratorConfig> {
  return apiFetch(`/generator-configs/${id}/publish`, { method: "POST" })
}

export function rollbackGeneratorConfig(generatorId: string): Promise<GeneratorConfig> {
  return apiFetch(`/generator-configs/rollback/${generatorId}`, { method: "POST" })
}

export function previewGeneratorConfig(body: {
  configJson: string
  category: string
  samples: number
}): Promise<PreviewQuestion[]> {
  return apiFetch("/generator-configs/preview", { method: "POST", body: JSON.stringify(body) })
}

export function listAllExamProfiles(): Promise<AdminExamProfile[]> {
  return apiFetch("/exam-profiles/admin")
}

export function createExamProfile(body: AdminExamProfile & { slug: string }): Promise<AdminExamProfile> {
  return apiFetch("/exam-profiles", { method: "POST", body: JSON.stringify(body) })
}

export function updateExamProfile(profile: AdminExamProfile): Promise<AdminExamProfile> {
  return apiFetch(`/exam-profiles/${profile.id}`, {
    method: "PUT",
    body: JSON.stringify({
      name: profile.name,
      description: profile.description,
      categoryWeightsJson: profile.categoryWeightsJson,
      questionCount: profile.questionCount,
      durationMinutes: profile.durationMinutes,
      passingScorePercent: profile.passingScorePercent,
      isPublic: profile.isPublic,
    }),
  })
}

export function deleteExamProfile(id: string): Promise<void> {
  return apiFetch(`/exam-profiles/${id}`, { method: "DELETE" })
}
