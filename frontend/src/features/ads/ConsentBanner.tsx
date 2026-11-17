import { useEffect, useState } from "react"
import { Button } from "@/components/ui/button"
import { getAdConsent, setAdConsent } from "./consent"

export function ConsentBanner() {
  const [visible, setVisible] = useState(false)

  useEffect(() => {
    setVisible(getAdConsent() === null)
  }, [])

  if (!visible) {
    return null
  }

  return (
    <div
      data-testid="consent-banner"
      className="fixed inset-x-0 bottom-16 z-20 mx-auto w-[calc(100%-2rem)] max-w-3xl rounded-lg border bg-background/95 p-3 shadow-lg backdrop-blur"
    >
      <p className="text-sm">
        Kami menggunakan cookie iklan pihak ketiga untuk mendukung layanan gratis
        ini. Iklan tidak pernah ditampilkan selama sesi ujian berlangsung.
      </p>
      <div className="mt-2 flex justify-end gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={() => {
            setAdConsent("denied")
            setVisible(false)
          }}
        >
          Tolak
        </Button>
        <Button
          size="sm"
          onClick={() => {
            setAdConsent("granted")
            setVisible(false)
          }}
        >
          Setuju
        </Button>
      </div>
    </div>
  )
}
