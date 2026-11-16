import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { MemoryRouter, Route, Routes } from "react-router-dom"
import { describe, expect, it, vi, beforeEach } from "vitest"
import { AuthProvider, type CurrentUser } from "@/features/auth/auth-context"
import { RequireAuth, RequireRole } from "@/app/guards"

const tokenStore = vi.hoisted(() => ({
  accessToken: null as string | null,
  refreshToken: null as string | null,
  set: vi.fn(),
  clear: vi.fn(),
}))

vi.mock("@/lib/api", () => ({
  tokenStorage: tokenStore,
  apiFetch: vi.fn(),
}))

import { apiFetch } from "@/lib/api"
const mockedApiFetch = vi.mocked(apiFetch)

const testUser: CurrentUser = {
  id: "1",
  email: "uji@test.local",
  displayName: "Pengguna Uji",
  roles: ["User"],
}

beforeEach(() => {
  vi.resetAllMocks()
  tokenStore.accessToken = null
  tokenStore.refreshToken = null
})

describe("RequireAuth", () => {
  it("redirects unauthenticated users to /login", async () => {
    mockedApiFetch.mockRejectedValue(new Error("401"))

    render(
      <MemoryRouter initialEntries={["/"]}>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<p>halaman-login</p>} />
            <Route element={<RequireAuth />}>
              <Route path="/" element={<p>halaman-terproteksi</p>} />
            </Route>
          </Routes>
        </AuthProvider>
      </MemoryRouter>,
    )

    await waitFor(() => expect(screen.getByText("halaman-login")).toBeInTheDocument())
    expect(screen.queryByText("halaman-terproteksi")).not.toBeInTheDocument()
  })

  it("renders protected content for authenticated users", async () => {
    tokenStore.accessToken = "token-aktif"
    mockedApiFetch.mockResolvedValue(testUser)

    render(
      <MemoryRouter initialEntries={["/"]}>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<p>halaman-login</p>} />
            <Route element={<RequireAuth />}>
              <Route path="/" element={<p>halaman-terproteksi</p>} />
            </Route>
          </Routes>
        </AuthProvider>
      </MemoryRouter>,
    )

    await waitFor(() => expect(screen.getByText("halaman-terproteksi")).toBeInTheDocument())
  })
})

describe("RequireRole", () => {
  it("blocks users without the required role", async () => {
    mockedApiFetch.mockResolvedValue(testUser)

    render(
      <MemoryRouter initialEntries={["/admin"]}>
        <AuthProvider>
          <Routes>
            <Route path="/" element={<p>beranda</p>} />
            <Route element={<RequireRole role="Administrator" />}>
              <Route path="/admin" element={<p>panel-admin</p>} />
            </Route>
          </Routes>
        </AuthProvider>
      </MemoryRouter>,
    )

    await waitFor(() => expect(screen.getByText("beranda")).toBeInTheDocument())
    expect(screen.queryByText("panel-admin")).not.toBeInTheDocument()
  })

  it("allows users with the required role", async () => {
    tokenStore.accessToken = "token-aktif"
    mockedApiFetch.mockResolvedValue({ ...testUser, roles: ["User", "Administrator"] })

    render(
      <MemoryRouter initialEntries={["/admin"]}>
        <AuthProvider>
          <Routes>
            <Route path="/" element={<p>beranda</p>} />
            <Route element={<RequireRole role="Administrator" />}>
              <Route path="/admin" element={<p>panel-admin</p>} />
            </Route>
          </Routes>
        </AuthProvider>
      </MemoryRouter>,
    )

    await waitFor(() => expect(screen.getByText("panel-admin")).toBeInTheDocument())
  })
})

describe("LoginPage", () => {
  it("submits credentials and navigates on success", async () => {
    const user = userEvent.setup()
    mockedApiFetch.mockResolvedValue({
      accessToken: "a",
      refreshToken: "r",
      user: testUser,
    })

    const { LoginPage } = await import("@/features/auth/LoginPage")
    render(
      <MemoryRouter initialEntries={["/login"]}>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={<p>beranda</p>} />
          </Routes>
        </AuthProvider>
      </MemoryRouter>,
    )

    await user.type(screen.getByLabelText("Email"), "uji@test.local")
    await user.type(screen.getByLabelText("Kata Sandi"), "Password#123")
    await user.click(screen.getByRole("button", { name: "Masuk" }))

    await waitFor(() => expect(screen.getByText("beranda")).toBeInTheDocument())
    expect(mockedApiFetch).toHaveBeenCalledWith(
      "/auth/login",
      expect.objectContaining({ method: "POST" }),
    )
  })

  it("shows an error message on failure", async () => {
    const user = userEvent.setup()
    mockedApiFetch.mockRejectedValue(new Error("Email atau kata sandi salah."))

    const { LoginPage } = await import("@/features/auth/LoginPage")
    render(
      <MemoryRouter initialEntries={["/login"]}>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
          </Routes>
        </AuthProvider>
      </MemoryRouter>,
    )

    await user.type(screen.getByLabelText("Email"), "uji@test.local")
    await user.type(screen.getByLabelText("Kata Sandi"), "salah-banget")
    await user.click(screen.getByRole("button", { name: "Masuk" }))

    await waitFor(() =>
      expect(screen.getByRole("alert")).toHaveTextContent("Email atau kata sandi salah."),
    )
  })
})
