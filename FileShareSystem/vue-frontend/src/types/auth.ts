export interface AuthResponse {
  token: string
  username: string
  email: string
  expiresAt: Date
}

export interface LoginForm {
  usernameOrEmail: string
  password: string
}

export interface RegisterForm {
  username: string
  email: string
  password: string
}

export interface FileItem {
  id: number
  fileName: string
  originalName: string
  fileSize: number
  contentType: string
  isShared: boolean
  sharedToken?: string
  sharedExpiry: string | null
  createdAt: string
}

export interface ShareFileResponse {
  shareUrl: string
  token: string
  expiresAt: string | null
}