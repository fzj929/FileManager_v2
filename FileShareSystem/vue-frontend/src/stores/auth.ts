import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { AuthResponse } from '@/types/auth'
import { authService } from '@/services/authService'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<{ username: string; email: string } | null>(null)
  const expiresAt = ref<string | null>(localStorage.getItem('expiresAt'))

  const isAuthenticated = computed(() => {
    if (!token.value || !expiresAt.value) return false
    return new Date(expiresAt.value) > new Date()
  })

  const login = async (credentials: { usernameOrEmail: string; password: string }) => {
    try {
      const response = await authService.login(credentials)
      const authData: AuthResponse = response.data

      setAuthData(authData)
      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        error: error.response?.data?.message || 'Login failed'
      }
    }
  }

  const register = async (userData: { username: string; email: string; password: string }) => {
    try {
      const response = await authService.register(userData)
      const authData: AuthResponse = response.data

      setAuthData(authData)
      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        error: error.response?.data?.message || 'Registration failed'
      }
    }
  }

  const logout = () => {
    token.value = null
    user.value = null
    expiresAt.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('expiresAt')
    localStorage.removeItem('username')
    localStorage.removeItem('email')
  }

  const checkAuth = () => {
    if (isAuthenticated.value) {
      const username = localStorage.getItem('username')
      const email = localStorage.getItem('email')
      if (username && email) {
        user.value = { username, email }
      }
    } else {
      logout()
    }
  }

  const setAuthData = (authData: AuthResponse) => {
    token.value = authData.token
    user.value = { username: authData.username, email: authData.email }
    expiresAt.value = authData.expiresAt.toString()

    localStorage.setItem('token', authData.token)
    localStorage.setItem('expiresAt', authData.expiresAt.toString())
    localStorage.setItem('username', authData.username)
    localStorage.setItem('email', authData.email)
  }

  return {
    token,
    user,
    isAuthenticated,
    login,
    register,
    logout,
    checkAuth
  }
})