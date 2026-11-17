import { apiFetch } from "@/lib/api"
import type { AdConfig } from "./types"

export function getAdConfig(): Promise<AdConfig> {
  return apiFetch("/ads/config")
}
