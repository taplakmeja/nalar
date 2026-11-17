import { useEffect, useRef, useState } from "react"
import { getAdConfig } from "./api"
import { getAdConsent } from "./consent"
import { createAdProvider } from "./provider-factory"
import { useIsExamActive } from "./useIsExamActive"
import type { AdConfig, AdPlacement } from "./types"

export function AdSlot({ placement }: { placement: string }) {
  const isExamActive = useIsExamActive()
  const containerRef = useRef<HTMLDivElement | null>(null)
  const [config, setConfig] = useState<AdConfig | null>(null)
  const [consent, setConsentState] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false
    getAdConfig()
      .then((loaded) => {
        if (!cancelled) {
          setConfig(loaded)
          setConsentState(getAdConsent())
        }
      })
      .catch(() => {})
    return () => {
      cancelled = true
    }
  }, [])

  const enabled =
    config !== null &&
    config.enabled &&
    consent === "granted" &&
    !isExamActive

  useEffect(() => {
    if (!enabled || !config || !containerRef.current) {
      return
    }

    let placements: AdPlacement[] = []
    try {
      placements = JSON.parse(config.placementsJson) as AdPlacement[]
    } catch {
      return
    }

    const match = placements.find((item) => item.placement === placement)
    const provider = createAdProvider(config.provider)
    if (!match || !provider) {
      return
    }

    provider.mount(containerRef.current, match, config.client)
    return () => provider.unmount()
  }, [enabled, config, placement])

  if (!enabled) {
    return null
  }

  return <div ref={containerRef} data-testid={`ad-slot-${placement}`} className="my-2" />
}
