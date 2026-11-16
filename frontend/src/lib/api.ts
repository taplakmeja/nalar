const BASE_URL = "/api"

export class ApiError extends Error {
  status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

export const tokenStorage = {
  get accessToken() {
    return localStorage.getItem("psikotes.accessToken")
  },
  get refreshToken() {
    return localStorage.getItem("psikotes.refreshToken")
  },
  set(access: string, refresh: string) {
    localStorage.setItem("psikotes.accessToken", access)
    localStorage.setItem("psikotes.refreshToken", refresh)
  },
  clear() {
    localStorage.removeItem("psikotes.accessToken")
    localStorage.removeItem("psikotes.refreshToken")
  },
}

export interface AuthTokens {
  accessToken: string
  refreshToken: string
}

export async function apiFetch<T>(path: string, init: RequestInit = {}, retry = true): Promise<T> {
  const headers = new Headers(init.headers)
  headers.set("Content-Type", "application/json")

  const token = tokenStorage.accessToken
  if (token) {
    headers.set("Authorization", `Bearer ${token}`)
  }

  const response = await fetch(`${BASE_URL}${path}`, { ...init, headers })

  if (response.status === 401 && retry && tokenStorage.refreshToken) {
    const refreshed = await fetch(`${BASE_URL}/auth/refresh`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ refreshToken: tokenStorage.refreshToken }),
    })

    if (refreshed.ok) {
      const tokens = (await refreshed.json()) as AuthTokens
      tokenStorage.set(tokens.accessToken, tokens.refreshToken)
      return apiFetch<T>(path, init, false)
    }

    tokenStorage.clear()
  }

  if (!response.ok) {
    const detail = await response.json().catch(() => null)
    throw new ApiError(response.status, detail?.detail ?? detail?.title ?? "Terjadi kesalahan.")
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}
