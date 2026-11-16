import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react"
import { apiFetch, tokenStorage } from "@/lib/api"

export interface CurrentUser {
  id: string
  email: string
  displayName: string
  roles: string[]
}

interface AuthContextValue {
  user: CurrentUser | null
  isLoading: boolean
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string, displayName: string) => Promise<void>
  logout: () => Promise<void>
  hasRole: (role: string) => boolean
}

const AuthContext = createContext<AuthContextValue | null>(null)

interface AuthResponse {
  accessToken: string
  refreshToken: string
  user: CurrentUser
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const controller = new AbortController()
    if (!tokenStorage.accessToken) {
      setIsLoading(false)
      return
    }

    apiFetch<CurrentUser>("/me", { signal: controller.signal })
      .then(setUser)
      .catch(() => tokenStorage.clear())
      .finally(() => setIsLoading(false))

    return () => controller.abort()
  }, [])

  const applyAuth = useCallback((response: AuthResponse) => {
    tokenStorage.set(response.accessToken, response.refreshToken)
    setUser(response.user)
  }, [])

  const login = useCallback(async (email: string, password: string) => {
    applyAuth(await apiFetch<AuthResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    }))
  }, [applyAuth])

  const register = useCallback(async (email: string, password: string, displayName: string) => {
    applyAuth(await apiFetch<AuthResponse>("/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, password, displayName }),
    }))
  }, [applyAuth])

  const logout = useCallback(async () => {
    try {
      await apiFetch<void>("/auth/logout", {
        method: "POST",
        body: JSON.stringify({ refreshToken: tokenStorage.refreshToken }),
      })
    } finally {
      tokenStorage.clear()
      setUser(null)
    }
  }, [])

  const hasRole = useCallback((role: string) => user?.roles.includes(role) ?? false, [user])

  const value = useMemo(
    () => ({ user, isLoading, login, register, logout, hasRole }),
    [user, isLoading, login, register, logout, hasRole],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider")
  }
  return context
}
