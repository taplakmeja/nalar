import type { AdPlacement, AdProvider } from "./types"

const SCRIPT_ID = "psikotes-adsense-script"

export class AdSenseProvider implements AdProvider {
  readonly id = "adsense"
  private container: HTMLElement | null = null

  mount(container: HTMLElement, placement: AdPlacement, client: string): void {
    this.container = container
    this.ensureScript(client)

    const ins = document.createElement("ins")
    ins.className = "adsbygoogle"
    ins.style.display = "block"
    ins.setAttribute("data-ad-client", client)
    ins.setAttribute("data-ad-slot", placement.slotId)
    ins.setAttribute("data-ad-format", "auto")
    ins.setAttribute("data-full-width-responsive", "true")
    container.appendChild(ins)

    const w = window as unknown as { adsbygoogle?: unknown[] }
    w.adsbygoogle = w.adsbygoogle ?? []
    w.adsbygoogle.push({})
  }

  unmount(): void {
    if (this.container) {
      this.container.replaceChildren()
      this.container = null
    }
  }

  private ensureScript(client: string): void {
    if (document.getElementById(SCRIPT_ID)) {
      return
    }

    const script = document.createElement("script")
    script.id = SCRIPT_ID
    script.async = true
    script.src = `https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=${encodeURIComponent(client)}`
    script.crossOrigin = "anonymous"
    document.head.appendChild(script)
  }
}
