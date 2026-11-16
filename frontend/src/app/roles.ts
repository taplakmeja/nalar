export const Roles = {
  User: "User",
  Administrator: "Administrator",
  ContentManager: "ContentManager",
} as const

export type Role = (typeof Roles)[keyof typeof Roles]
