export interface AdPlacement {
  placement: string
  slotId: string
}

export interface AdConfig {
  enabled: boolean
  provider: string
  client: string
  placementsJson: string
}

export type AdConsent = "granted" | "denied"

export interface AdProvider {
  readonly id: string
  mount(container: HTMLElement, placement: AdPlacement, client: string): void
  unmount(): void
}
