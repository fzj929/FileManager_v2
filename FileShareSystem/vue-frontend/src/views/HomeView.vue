<template>
  <div class="home-view">
    <!-- Hero Section -->
    <section class="hero-section">
      <div class="hero-content">
        <div class="hero-text">
          <h1 class="hero-title">专业文件共享</h1>
          <p class="hero-description">
企业级文件共享平台，具备高级安全性、协作工具，<br>
为现代团队设计的直观界面。
          </p>
          <div class="hero-actions">
            <el-button
              v-if="!authStore.isAuthenticated"
              type="primary"
              size="large"
              class="cta-button"
              @click="$router.push('/register')"
            >
              <el-icon><Key /></el-icon>
              免费开始使用
            </el-button>
            <el-button
              v-if="authStore.isAuthenticated"
              type="primary"
              size="large"
              class="cta-button"
              @click="$router.push('/files')"
            >
              <el-icon><FolderOpened /></el-icon>
              访问仪表板
            </el-button>
            <el-button
              v-if="!authStore.isAuthenticated"
              size="large"
              class="secondary-button"
              @click="$router.push('/login')"
            >
              <el-icon><User /></el-icon>
              登录
            </el-button>
          </div>
        </div>
        <div class="hero-visual">
          <div class="feature-showcase">
            <div class="showcase-item">
              <el-icon :size="48" color="#409eff"><Upload /></el-icon>
              <span>快速上传</span>
            </div>
            <div class="showcase-item">
              <el-icon :size="48" color="#67c23a"><Lock /></el-icon>
              <span>安全存储</span>
            </div>
            <div class="showcase-item">
              <el-icon :size="48" color="#e6a23c"><Share /></el-icon>
              <span>轻松分享</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="features-section">
      <div class="section-header">
        <h2>专业功能</h2>
        <p>安全文件管理所需的一切功能</p>
      </div>

      <div class="features-grid">
        <div class="feature-card enhanced">
          <div class="feature-icon">
            <el-icon :size="64" color="#409eff"><Upload /></el-icon>
          </div>
          <h3>极速上传</h3>
        <p>拖放界面，支持多文件和实时进度跟踪</p>
          <ul class="feature-list">
            <li><el-icon><Check /></el-icon> 批量文件上传</li>
            <li><el-icon><Check /></el-icon> 进度跟踪</li>
            <li><el-icon><Check /></el-icon> 续传支持</li>
          </ul>
        </div>

        <div class="feature-card enhanced">
          <div class="feature-icon">
            <el-icon :size="64" color="#67c23a"><Lock /></el-icon>
          </div>
          <h3>企业级安全</h3>
        <p>军用级加密，具备细粒度访问控制和审计跟踪</p>
          <ul class="feature-list">
            <li><el-icon><Check /></el-icon> AES-256 加密</li>
            <li><el-icon><Check /></el-icon> 基于角色的访问</li>
            <li><el-icon><Check /></el-icon> 审计日志</li>
          </ul>
        </div>

        <div class="feature-card enhanced">
          <div class="feature-icon">
            <el-icon :size="64" color="#e6a23c"><Share /></el-icon>
          </div>
          <h3>高级分享</h3>
        <p>灵活的分享选项，支持到期日期和权限控制</p>
          <ul class="feature-list">
            <li><el-icon><Check /></el-icon> 限时链接</li>
            <li><el-icon><Check /></el-icon> 密码保护</li>
            <li><el-icon><Check /></el-icon> 下载跟踪</li>
          </ul>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section class="stats-section">
      <div class="stats-container">
        <div class="stat-item">
          <div class="stat-number">1M+</div>
          <div class="stat-label">共享文件</div>
        </div>
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
    </section>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
</script>

<style scoped>
.home-view {
  padding: 0;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
}

.hero-section {
  padding: 80px 40px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.hero-content {
  max-width: 1400px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 80px;
  align-items: center;
}

.hero-text {
  text-align: left;
}

.hero-title {
  font-size: 56px;
  font-weight: 700;
  margin-bottom: 24px;
  line-height: 1.2;
}

.hero-description {
  font-size: 20px;
  line-height: 1.6;
  margin-bottom: 32px;
  opacity: 0.9;
}

.hero-actions {
  display: flex;
  gap: 16px;
  margin-bottom: 40px;
}

.cta-button {
  background: linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%);
  border: none;
  font-weight: 600;
  padding: 16px 32px;
  height: auto;
  font-size: 16px;
}

.cta-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(255, 107, 107, 0.3);
}

.secondary-button {
  background: rgba(255, 255, 255, 0.2);
  border: 2px solid rgba(255, 255, 255, 0.3);
  color: white;
  font-weight: 600;
  padding: 16px 32px;
  height: auto;
  font-size: 16px;
}

.secondary-button:hover {
  background: rgba(255, 255, 255, 0.3);
  border-color: rgba(255, 255, 255, 0.5);
}

.hero-visual {
  display: flex;
  justify-content: center;
  align-items: center;
}

.feature-showcase {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
}

.showcase-item {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 24px;
  text-align: center;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
}

.showcase-item:hover {
  transform: translateY(-5px);
  background: rgba(255, 255, 255, 0.15);
}

.showcase-item span {
  display: block;
  margin-top: 12px;
  font-weight: 500;
}

.features-section {
  padding: 80px 40px;
  background: #f8fafc;
}

.section-header {
  text-align: center;
  margin-bottom: 60px;
}

.section-header h2 {
  font-size: 40px;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 16px;
}

.section-header p {
  font-size: 18px;
  color: #64748b;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 40px;
  max-width: 1400px;
  margin: 0 auto;
}

.feature-card.enhanced {
  background: white;
  border-radius: 16px;
  padding: 40px;
  text-align: left;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  border: none;
  transition: all 0.3s ease;
}

.feature-card.enhanced:hover {
  transform: translateY(-10px);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.15);
}

.feature-icon {
  text-align: center;
  margin-bottom: 24px;
}

.feature-card.enhanced h3 {
  font-size: 24px;
  font-weight: 600;
  color: #1e293b;
  margin-bottom: 16px;
}

.feature-card.enhanced p {
  color: #64748b;
  line-height: 1.6;
  margin-bottom: 24px;
}

.feature-list {
  list-style: none;
  padding: 0;
}

.feature-list li {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
  color: #374151;
}

.feature-list .el-icon {
  color: #10b981;
}

.stats-section {
  padding: 60px 40px;
  background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
  color: white;
}

.stats-container {
  max-width: 1400px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 40px;
  text-align: center;
}

.stat-item {
  padding: 20px;
}

.stat-number {
  font-size: 48px;
  font-weight: 700;
  margin-bottom: 8px;
  background: linear-gradient(135deg, #60a5fa 0%, #3b82f6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.stat-label {
  font-size: 16px;
  opacity: 0.8;
  text-transform: uppercase;
  letter-spacing: 1px;
}

@media (max-width: 1024px) {
  .hero-content {
    grid-template-columns: 1fr;
    gap: 40px;
    text-align: center;
  }

  .features-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .stats-container {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .hero-section {
    padding: 60px 20px;
  }

  .hero-title {
    font-size: 40px;
  }

  .features-grid {
    grid-template-columns: 1fr;
  }

  .stats-container {
    grid-template-columns: 1fr;
  }

  .hero-actions {
    flex-direction: column;
    align-items: center;
  }

  .feature-showcase {
    grid-template-columns: 1fr;
  }
}
</style>

<style scoped>
.home-view {
  padding: 40px 20px;
}

.hero-section {
  min-height: 400px;
  text-align: center;
  margin-bottom: 60px;
}

.hero-content {
  max-width: 600px;
}

.hero-title {
  font-size: 48px;
  font-weight: bold;
  color: #303133;
  margin-bottom: 20px;
}

.hero-description {
  font-size: 18px;
  color: #606266;
  line-height: 1.6;
  margin-bottom: 30px;
}

.hero-actions {
  display: flex;
  gap: 16px;
  justify-content: center;
}

.features-section {
  margin-top: 40px;
}

.feature-card {
  height: 100%;
  text-align: center;
}

.feature-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.feature-header h3 {
  margin: 0;
  color: #303133;
}

@media (max-width: 768px) {
  .hero-title {
    font-size: 36px;
  }

  .hero-actions {
    flex-direction: column;
    align-items: center;
  }
}
</style>