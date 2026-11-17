import { AdSenseProvider } from "./adsense-provider"
import type { AdProvider } from "./types"

export function createAdProvider(providerId: string): AdProvider | null {
  if (providerId === "adsense") {
    return new AdSenseProvider()
  }

  return null
}
