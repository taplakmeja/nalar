import type { AdConsent } from "./types"

const STORAGE_KEY = "psikotes.adConsent"

export function getAdConsent(): AdConsent | null {
  const value = localStorage.getItem(STORAGE_KEY)
  return value === "granted" || value === "denied" ? value : null
}

export function setAdConsent(consent: AdConsent): void {
  localStorage.setItem(STORAGE_KEY, consent)
}
