import { useLocation } from "react-router-dom"

const ACTIVE_SESSION_PATTERN = /^\/sesi\/[^/]+$/

export function useIsExamActive(): boolean {
  const { pathname } = useLocation()
  return ACTIVE_SESSION_PATTERN.test(pathname)
}
