<template>
  <div class="register-view">
    <div class="register-container">
      <div class="register-content">
        <!-- Left Panel - Branding -->
        <div class="branding-panel">
          <div class="branding-content">
            <div class="logo-section">
              <h1 class="brand-logo">文件共享专业版</h1>
              <p class="brand-tagline">专业文件共享平台</p>
            </div>

            <div class="features-list">
              <div class="feature-item">
                <el-icon :size="24" color="#10b981"><Check /></el-icon>
                <span>企业级安全性</span>
              </div>
              <div class="feature-item">
                <el-icon :size="24" color="#10b981"><Check /></el-icon>
                <span>无限文件存储</span>
              </div>
              <div class="feature-item">
                <el-icon :size="24" color="#10b981"><Check /></el-icon>
                <span>高级分享控制</span>
              </div>
              <div class="feature-item">
                <el-icon :size="24" color="#10b981"><Check /></el-icon>
                <span>24/7客户支持</span>
              </div>
            </div>

            <div class="testimonial">
              <p class="testimonial-text">
                "文件共享专业版改变了我们团队的协作方式。
                安全功能让我们在分享敏感客户文件时感到安心。"
              </p>
              <div class="testimonial-author">
                <strong>Sarah Johnson</strong>
                <span>IT总监，TechCorp</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Right Panel - Registration Form -->
        <div class="form-panel">
          <div class="form-container">
            <div class="form-header">
              <h2>创建专业账户</h2>
              <p>加入使用文件共享专业版的数千名专业人士</p>
            </div>

            <el-form
              ref="registerFormRef"
              :model="registerForm"
              :rules="registerRules"
              label-position="top"
              @submit.prevent="handleRegister"
              class="register-form"
            >
              <div class="form-row">
                <el-form-item label="全名" prop="username" class="form-item-half">
                  <el-input
                    v-model="registerForm.username"
                    placeholder="输入您的全名"
                    prefix-icon="User"
                    size="large"
                  />
                </el-form-item>

                <el-form-item label="邮箱地址" prop="email" class="form-item-half">
                  <el-input
                    v-model="registerForm.email"
                    type="email"
                    placeholder="输入您的企业邮箱"
                    prefix-icon="Message"
                    size="large"
                  />
                </el-form-item>
              </div>

              <div class="form-row">
                <el-form-item label="密码" prop="password" class="form-item-half">
                  <el-input
                    v-model="registerForm.password"
                    type="password"
                    placeholder="创建强密码"
                    prefix-icon="Lock"
                    size="large"
                    show-password
                  />
                </el-form-item>

                <el-form-item label="确认密码" prop="confirmPassword" class="form-item-half">
                  <el-input
                    v-model="registerForm.confirmPassword"
                    type="password"
                    placeholder="确认您的密码"
                    prefix-icon="Lock"
                    size="large"
                    show-password
                  />
                </el-form-item>
              </div>

              <el-form-item>
                <el-button
                  type="primary"
                  size="large"
                  :loading="loading"
                  @click="handleRegister"
                  class="submit-button"
                >
                  <el-icon><Key /></el-icon>
                  创建专业账户
                </el-button>
              </el-form-item>

              <div class="divider">
                <span>或</span>
              </div>

              <div class="social-login">
                <p class="social-text">已有账户？</p>
                <el-button
                  type="info"
                  plain
                  size="large"
                  @click="$router.push('/login')"
                  class="social-button"
                >
                  <el-icon><User /></el-icon>
                  登录现有账户
                </el-button>
              </div>

              <div class="terms-text">
                <p>创建账户即表示您同意我们的 <a href="#">服务条款</a> <br>
                和 <a href="#">隐私政策</a></p>
              </div>
            </el-form>
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
const registerFormRef = ref<FormInstance>()
const loading = ref(false)

const registerForm = reactive({
  username: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const validateConfirmPassword = (rule: any, value: string, callback: any) => {
  if (value === '') {
    callback(new Error('请确认您的密码'))
  } else if (value !== registerForm.password) {
    callback(new Error('密码不匹配'))
  } else {
    callback()
  }
}

const registerRules: FormRules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, message: '用户名必须至少3个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入您的邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入有效的邮箱地址', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码必须至少6个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, validator: validateConfirmPassword, trigger: 'blur' }
  ]
}

const handleRegister = async () => {
  if (!registerFormRef.value) return

  await registerFormRef.value.validate(async (valid) => {
    if (valid) {
      loading.value = true
      try {
        const result = await authStore.register({
          username: registerForm.username,
          email: registerForm.email,
          password: registerForm.password
        })
        if (result.success) {
          ElMessage.success('注册成功！')
          router.push('/files')
        } else {
          ElMessage.error(result.error || '注册失败')
        }
      } catch (error) {
        ElMessage.error('注册失败，请重试。')
      } finally {
        loading.value = false
      }
    }
  })
}
</script>

<style scoped>
.register-view {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.register-container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}

.register-content {
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

.testimonial {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 30px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.testimonial-text {
  font-style: italic;
  margin: 0 0 20px 0;
  line-height: 1.6;
  font-size: 16px;
}

.testimonial-author strong {
  display: block;
  font-weight: 600;
  margin-bottom: 4px;
}

.testimonial-author span {
  opacity: 0.7;
  font-size: 14px;
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

.register-form {
  width: 100%;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-bottom: 20px;
}

.form-item-half {
  margin-bottom: 0;
}

.form-item-half :deep(.el-form-item__label) {
  color: #374151;
  font-weight: 500;
  margin-bottom: 8px;
}

.submit-button {
  width: 100%;
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border: none;
  font-weight: 600;
  padding: 16px;
  height: auto;
  font-size: 16px;
  margin-top: 10px;
}

.submit-button:hover {
  transform: translateY(-1px);
  box-shadow: 0 8px 25px rgba(16, 185, 129, 0.3);
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

.terms-text {
  text-align: center;
  margin-top: 24px;
  font-size: 13px;
  color: #6b7280;
}

.terms-text a {
  color: #3b82f6;
  text-decoration: none;
}

.terms-text a:hover {
  text-decoration: underline;
}

@media (max-width: 968px) {
  .register-content {
    grid-template-columns: 1fr;
  }

  .branding-panel {
    padding: 40px 30px;
  }

  .form-panel {
    padding: 40px 30px;
  }

  .form-row {
    grid-template-columns: 1fr;
    gap: 0;
  }
}

@media (max-width: 640px) {
  .register-view {
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
}
</style>