<template>
  <div class="login-view">
    <div class="login-container">
      <div class="login-content">
        <!-- Left Panel - Branding -->
        <div class="branding-panel">
          <div class="branding-content">
            <div class="logo-section">
              <h1 class="brand-logo">文件共享专业版</h1>
              <p class="brand-tagline">专业文件共享平台</p>
            </div>

            <div class="features-list">
              <div class="feature-item">
                <el-icon :size="24" color="#60a5fa"><Lock /></el-icon>
                <span>安全的云存储</span>
              </div>
              <div class="feature-item">
                <el-icon :size="24" color="#60a5fa"><Share /></el-icon>
                <span>高级分享控制</span>
              </div>
              <div class="feature-item">
                <el-icon :size="24" color="#60a5fa"><Upload /></el-icon>
                <span>无限上传</span>
              </div>
              <div class="feature-item">
                <el-icon :size="24" color="#60a5fa"><Monitor /></el-icon>
                <span>实时协作</span>
              </div>
            </div>

            <div class="stats-section">
              <div class="stat-item">
                <div class="stat-number">50K+</div>
                <div class="stat-label">活跃用户</div>
              </div>
              <div class="stat-item">
                <div class="stat-number">99.9%</div>
                <div class="stat-label">运行时间</div>
              </div>
              <div class="stat-item">
                <div class="stat-number">24/7</div>
                <div class="stat-label">技术支持</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Panel - Login Form -->
        <div class="form-panel">
          <div class="form-container">
            <div class="form-header">
              <h2>欢迎回来</h2>
              <p>登录以访问您的专业工作区</p>
            </div>

            <el-form
              ref="loginFormRef"
              :model="loginForm"
              :rules="loginRules"
              label-position="top"
              @submit.prevent="handleLogin"
              class="login-form"
            >
              <el-form-item label="用户名或邮箱" prop="usernameOrEmail">
                <el-input
                  v-model="loginForm.usernameOrEmail"
                  placeholder="输入您的用户名或邮箱"
                  prefix-icon="User"
                  size="large"
                />
              </el-form-item>

              <el-form-item label="密码" prop="password">
                <el-input
                  v-model="loginForm.password"
                  type="password"
                  placeholder="输入您的密码"
                  prefix-icon="Lock"
                  size="large"
                  show-password
                />
              </el-form-item>

              <el-form-item class="remember-forgot">
                <el-checkbox v-model="rememberMe">记住我</el-checkbox>
                <a href="#" class="forgot-link">忘记密码？</a>
              </el-form-item>

              <el-form-item>
                <el-button
                  type="primary"
                  size="large"
                  :loading="loading"
                  @click="handleLogin"
                  class="submit-button"
                >
                  <el-icon><Key /></el-icon>
                  登录到工作区
                </el-button>
              </el-form-item>
            </el-form>

            <div class="divider">
              <span>或</span>
            </div>

            <div class="social-login">
              <p class="social-text">还没有账户？</p>
              <el-button
                type="info"
                plain
                size="large"
                @click="$router.push('/register')"
                class="social-button"
              >
                <el-icon><User /></el-icon>
                创建新账户
              </el-button>
            </div>

            <div class="security-note">
              <p>🔒 您的连接是安全且加密的</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { FormInstance, FormRules } from 'element-plus'
import { ElMessage } from 'element-plus'

const router = useRouter()
const authStore = useAuthStore()
const loginFormRef = ref<FormInstance>()
const loading = ref(false)

const loginForm = reactive({
  usernameOrEmail: '',
  password: ''
})

const rememberMe = ref(false)

const loginRules: FormRules = {
  usernameOrEmail: [
    { required: true, message: '请输入您的用户名或邮箱', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入您的密码', trigger: 'blur' },
    { min: 6, message: '密码必须至少6个字符', trigger: 'blur' }
  ]
}

const handleLogin = async () => {
  if (!loginFormRef.value) return

  await loginFormRef.value.validate(async (valid) => {
    if (valid) {
      loading.value = true
      try {
        const result = await authStore.login(loginForm)
        if (result.success) {
          ElMessage.success('登录成功！')
          router.push('/files')
        } else {
          ElMessage.error(result.error || '登录失败')
        }
      } catch (error) {
        ElMessage.error('登录失败，请重试。')
      } finally {
        loading.value = false
      }
    }
  })
}
</script>

<style scoped>
.login-view {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.login-container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}

.login-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0;
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.15);
  min-height: 700px;
}

.branding-panel {
  background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
  color: white;
  padding: 60px;
  display: flex;
  align-items: center;
}

.branding-content {
  width: 100%;
}

.logo-section {
  margin-bottom: 50px;
}

.brand-logo {
  font-size: 36px;
  font-weight: 700;
  margin: 0 0 10px 0;
  background: linear-gradient(135deg, #60a5fa 0%, #3b82f6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.brand-tagline {
  font-size: 18px;
  opacity: 0.8;
  margin: 0;
}

.features-list {
  margin-bottom: 50px;
}

.feature-item {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 20px;
  font-size: 16px;
}

.feature-item span {
  opacity: 0.9;
}

.stats-section {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
  text-align: center;
}

.stat-item {
  padding: 15px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.stat-number {
  font-size: 24px;
  font-weight: 700;
  margin-bottom: 4px;
  color: #60a5fa;
}

.stat-label {
  font-size: 12px;
  opacity: 0.7;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.form-panel {
  padding: 60px;
  display: flex;
  align-items: center;
  background: #f8fafc;
}

.form-container {
  width: 100%;
  max-width: 400px;
  margin: 0 auto;
}

.form-header {
  text-align: center;
  margin-bottom: 40px;
}

.form-header h2 {
  font-size: 28px;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 8px 0;
}

.form-header p {
  color: #64748b;
  font-size: 16px;
  margin: 0;
}

.login-form {
  width: 100%;
}

.remember-forgot {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.forgot-link {
  color: #3b82f6;
  text-decoration: none;
  font-size: 14px;
}

.forgot-link:hover {
  text-decoration: underline;
}

.submit-button {
  width: 100%;
  background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
  border: none;
  font-weight: 600;
  padding: 16px;
  height: auto;
  font-size: 16px;
  margin-top: 10px;
}

.submit-button:hover {
  transform: translateY(-1px);
  box-shadow: 0 8px 25px rgba(59, 130, 246, 0.3);
}

.divider {
  text-align: center;
  margin: 30px 0;
  position: relative;
}

.divider::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: #e5e7eb;
}

.divider span {
  background: #f8fafc;
  padding: 0 16px;
  color: #6b7280;
  font-size: 14px;
  position: relative;
  z-index: 1;
}

.social-login {
  text-align: center;
}

.social-text {
  color: #6b7280;
  margin: 0 0 16px 0;
  font-size: 14px;
}

.social-button {
  width: 100%;
  border: 2px solid #d1d5db;
  color: #374151;
  font-weight: 500;
  padding: 14px;
  height: auto;
}

.social-button:hover {
  border-color: #9ca3af;
  background: #f9fafb;
}

.security-note {
  text-align: center;
  margin-top: 24px;
  font-size: 13px;
  color: #10b981;
  font-weight: 500;
}

@media (max-width: 968px) {
  .login-content {
    grid-template-columns: 1fr;
  }

  .branding-panel {
    padding: 40px 30px;
  }

  .form-panel {
    padding: 40px 30px;
  }
}

@media (max-width: 640px) {
  .login-view {
    padding: 10px;
  }

  .branding-panel {
    padding: 30px 20px;
  }

  .form-panel {
    padding: 30px 20px;
  }

  .brand-logo {
    font-size: 28px;
  }

  .form-header h2 {
    font-size: 24px;
  }

  .stats-section {
    grid-template-columns: 1fr;
  }
}
</style>