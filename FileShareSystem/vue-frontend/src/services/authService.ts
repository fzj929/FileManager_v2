import api from './api'
import type { LoginForm, RegisterForm } from '@/types/auth'

export const authService = {
  login(credentials: LoginForm) {
    return api.post('/auth/login', credentials)
  },

  register(userData: RegisterForm) {
    return api.post('/auth/register', userData)
  },

  logout() {
    return api.post('/auth/logout')
  }
}