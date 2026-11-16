import { apiFetch } from "@/lib/api"
import type { DashboardDto } from "./types"

export function getDashboard(): Promise<DashboardDto> {
  return apiFetch("/analytics/dashboard")
}
