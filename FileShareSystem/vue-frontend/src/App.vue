<template>
  <div id="app">
    <el-container class="layout-container">
      <el-header class="header">
        <div class="header-content">
          <h1 class="logo">FileShare</h1>
          <nav class="nav-menu">
            <router-link to="/" class="nav-link">
              <el-icon><HomeFilled /></el-icon> 首页
            </router-link>
            <router-link v-if="authStore.isAuthenticated" to="/files" class="nav-link">
              <el-icon><Files /></el-icon> 我的文件
            </router-link>
            <router-link v-if="!authStore.isAuthenticated" to="/login" class="nav-link">
              <el-icon><User /></el-icon> 登录
            </router-link>
            <router-link v-if="!authStore.isAuthenticated" to="/register" class="nav-link">
              <el-icon><UserFilled /></el-icon> 注册
            </router-link>
            <el-dropdown v-if="authStore.isAuthenticated" @command="handleCommand">
              <span class="el-dropdown-link nav-link">
                <el-icon><Avatar /></el-icon> {{ authStore.user?.username }}
              </span>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="profile">个人资料</el-dropdown-item>
                  <el-dropdown-item command="logout" divided>退出登录</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </nav>
        </div>
      </el-header>

      <el-main class="main-content">
        <router-view />
      </el-main>

      <el-footer class="footer">
        <p>© 2024 文件共享系统. 基于 Vue.js 和 .NET 8 构建。</p>
      </el-footer>
    </el-container>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ElMessage } from 'element-plus'

const authStore = useAuthStore()
const router = useRouter()

const handleCommand = (command: string) => {
  if (command === 'logout') {
    authStore.logout()
    ElMessage.success('退出登录成功')
    router.push('/')
  } else if (command === 'profile') {
    ElMessage.info('个人资料功能即将推出！')
  }
}

onMounted(() => {
  authStore.checkAuth()
})
</script>

<style scoped>
.layout-container {
  min-height: 100vh;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0;
  box-shadow: 0 2px 12px rgba(0,0,0,0.1);
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 40px;
  height: 70px;
}

.logo {
  margin: 0;
  font-size: 28px;
  font-weight: 700;
  letter-spacing: -0.5px;
}

.nav-menu {
  display: flex;
  align-items: center;
  gap: 25px;
}

.nav-link {
  color: white;
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  border-radius: 6px;
  transition: all 0.3s ease;
  font-weight: 500;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.15);
  transform: translateY(-1px);
}

.nav-link.router-link-active {
  background-color: rgba(255, 255, 255, 0.25);
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}

.el-dropdown-link {
  cursor: pointer;
  font-weight: 500;
}

.main-content {
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: calc(100vh - 120px);
  max-width: 1400px;
  margin: 0 auto;
  padding: 40px;
}

.footer {
  background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%);
  text-align: center;
  color: #ecf0f1;
  padding: 20px;
}

@media (max-width: 768px) {
  .header-content {
    padding: 0 20px;
  }

  .main-content {
    padding: 20px;
  }
}
</style>