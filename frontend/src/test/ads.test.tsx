import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { MemoryRouter, Route, Routes } from "react-router-dom"
import { describe, expect, it, vi, beforeEach } from "vitest"
import { AdSlot } from "@/features/ads/AdSlot"
import { ConsentBanner } from "@/features/ads/ConsentBanner"
import { createAdProvider } from "@/features/ads/provider-factory"

vi.mock("@/lib/api", () => ({
  apiFetch: vi.fn(),
}))

import { apiFetch } from "@/lib/api"
const mockedApiFetch = vi.mocked(apiFetch)

const enabledConfig = {
  enabled: true,
  provider: "adsense",
  client: "ca-pub-test",
  placementsJson: JSON.stringify([{ placement: "below-header", slotId: "12345" }]),
}

beforeEach(() => {
  vi.resetAllMocks()
  localStorage.clear()
})

describe("AdSlot", () => {
  it("renders nothing when ads are disabled", async () => {
    mockedApiFetch.mockResolvedValue({ ...enabledConfig, enabled: false })

    render(
      <MemoryRouter initialEntries={["/"]}>
        <AdSlot placement="below-header" />
      </MemoryRouter>,
    )

    await waitFor(() => expect(mockedApiFetch).toHaveBeenCalled())
    expect(screen.queryByTestId("ad-slot-below-header")).toBeNull()
  })

  it("renders nothing without consent even when enabled", async () => {
    mockedApiFetch.mockResolvedValue(enabledConfig)

    render(
      <MemoryRouter initialEntries={["/"]}>
        <AdSlot placement="below-header" />
      </MemoryRouter>,
    )

    await waitFor(() => expect(mockedApiFetch).toHaveBeenCalled())
    expect(screen.queryByTestId("ad-slot-below-header")).toBeNull()
  })

  it("renders nothing while an exam session is active", async () => {
    mockedApiFetch.mockResolvedValue(enabledConfig)
    localStorage.setItem("psikotes.adConsent", "granted")

    render(
      <MemoryRouter initialEntries={["/sesi/session-123"]}>
        <Routes>
          <Route path="/sesi/:sessionId" element={<AdSlot placement="below-header" />} />
        </Routes>
      </MemoryRouter>,
    )

    await waitFor(() => expect(mockedApiFetch).toHaveBeenCalled())
    expect(screen.queryByTestId("ad-slot-below-header")).toBeNull()
  })

  it("renders the ad container with consent outside exams", async () => {
    mockedApiFetch.mockResolvedValue(enabledConfig)
    localStorage.setItem("psikotes.adConsent", "granted")

    render(
      <MemoryRouter initialEntries={["/"]}>
        <AdSlot placement="below-header" />
      </MemoryRouter>,
    )

    await waitFor(() =>
      expect(screen.getByTestId("ad-slot-below-header")).toBeInTheDocument(),
    )
  })

  it("renders nothing when consent is denied", async () => {
    mockedApiFetch.mockResolvedValue(enabledConfig)
    localStorage.setItem("psikotes.adConsent", "denied")

    render(
      <MemoryRouter initialEntries={["/"]}>
        <AdSlot placement="below-header" />
      </MemoryRouter>,
    )

    await waitFor(() => expect(mockedApiFetch).toHaveBeenCalled())
    expect(screen.queryByTestId("ad-slot-below-header")).toBeNull()
  })
})

describe("ConsentBanner", () => {
  it("appears without stored consent and stores the choice", async () => {
    const user = userEvent.setup()
    render(<ConsentBanner />)

    expect(screen.getByTestId("consent-banner")).toBeInTheDocument()

    await user.click(screen.getByRole("button", { name: "Setuju" }))

    expect(localStorage.getItem("psikotes.adConsent")).toBe("granted")
    expect(screen.queryByTestId("consent-banner")).toBeNull()
  })

  it("stays hidden when consent was already stored", () => {
    localStorage.setItem("psikotes.adConsent", "denied")
    render(<ConsentBanner />)

    expect(screen.queryByTestId("consent-banner")).toBeNull()
  })
})

describe("createAdProvider", () => {
  it("returns an AdSense provider for adsense", () => {
    expect(createAdProvider("adsense")?.id).toBe("adsense")
  })

  it("returns null for unknown providers", () => {
    expect(createAdProvider("mystery")).toBeNull()
  })
})
