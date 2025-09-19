<template>
  <div class="shared-file-view">
    <!-- Header -->
    <div class="header">
      <h1 class="title">文件共享</h1>
      <el-button type="primary" @click="$router.push('/')">
        <el-icon><HomeFilled /></el-icon>
        返回首页
      </el-button>
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <!-- 加载状态 -->
      <div v-if="loading" class="content-card">
        <div class="loading-section">
          <el-icon size="48" class="loading-icon"><Loading /></el-icon>
          <h3>正在加载文件信息...</h3>
          <p>请稍候</p>
        </div>
      </div>

      <!-- 错误状态 -->
      <div v-else-if="error" class="content-card error-card">
        <div class="error-section">
          <el-icon size="64" color="#f56c6c"><CircleClose /></el-icon>
          <h3>加载失败</h3>
          <p class="error-text">{{ error }}</p>
          <div class="error-actions">
            <el-button type="primary" @click="loadFileInfo">重试</el-button>
            <el-button @click="$router.push('/')">返回首页</el-button>
          </div>
        </div>
      </div>

      <!-- 文件信息 -->
      <div v-else-if="fileInfo" class="content-card">
        <div class="file-section">
          <!-- 文件图标 -->
          <div class="file-icon">
            <el-icon size="80" color="#409eff"><Document /></el-icon>
          </div>

          <!-- 文件名 -->
          <h2 class="file-name">{{ fileInfo.originalName }}</h2>

          <!-- 文件信息 -->
          <div class="file-details">
            <div class="detail-row">
              <span class="label">文件大小:</span>
              <span class="value">{{ formatFileSize(fileInfo.fileSize) }}</span>
            </div>
            <div class="detail-row">
              <span class="label">文件类型:</span>
              <span class="value">{{ fileInfo.contentType }}</span>
            </div>
            <div v-if="fileInfo.sharedExpiry" class="detail-row">
              <span class="label">到期时间:</span>
              <span class="value">{{ formatDate(fileInfo.sharedExpiry) }}</span>
            </div>
          </div>

          <!-- 下载按钮 -->
          <div class="download-section">
            <el-button
              type="success"
              size="large"
              :icon="Download"
              :loading="downloading"
              @click="downloadFile"
              class="download-btn"
            >
              {{ downloading ? '正在下载...' : '下载文件' }}
            </el-button>
          </div>
        </div>
      </div>

      <!-- 空状态 -->
      <div v-else class="content-card">
        <div class="empty-section">
          <el-icon size="64" color="#c0c4cc"><Document /></el-icon>
          <h3>未找到文件</h3>
          <p>分享链接可能已失效</p>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <div class="footer">
      <p>© 2024 文件共享系统</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { 
  CircleClose, 
  Document, 
  Download, 
  HomeFilled, 
  Loading
} from '@element-plus/icons-vue'
import { fileService } from '@/services/fileService'
import api from '@/services/api'

// 响应式数据
const route = useRoute()
const loading = ref(true)
const downloading = ref(false)
const error = ref('')
const fileInfo = ref<any>(null)
const token = ref('')

// 格式化文件大小
const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN')
}

// 加载文件信息
const loadFileInfo = async () => {
  loading.value = true
  error.value = ''
  
  try {
    const response = await api.get(`/files/shared/${token.value}/info`)
    fileInfo.value = response.data
  } catch (err: any) {
    console.error('加载文件信息失败:', err)
    if (err.response?.status === 404) {
      error.value = '文件不存在或已被删除'
    } else if (err.response?.status === 410) {
      error.value = '分享链接已过期'
    } else {
      error.value = err.response?.data?.message || '加载失败，请重试'
    }
  } finally {
    loading.value = false
  }
}

// 下载文件
const downloadFile = async () => {
  if (!token.value) return

  downloading.value = true
  try {
    const response = await fileService.getSharedFile(token.value)
    
    // 获取文件名
    let filename = fileInfo.value?.originalName || 'download'
    const contentDisposition = response.headers['content-disposition']
    if (contentDisposition) {
      const match = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/)
      if (match && match[1]) {
        filename = match[1].replace(/['"]/g, '')
      }
    }

    // 创建下载
    const blob = new Blob([response.data])
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = filename
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)

    ElMessage.success('下载成功')
  } catch (err: any) {
    console.error('下载失败:', err)
    ElMessage.error(err.response?.data?.message || '下载失败')
  } finally {
    downloading.value = false
  }
}

// 页面初始化
onMounted(() => {
  token.value = route.params.token as string
  if (token.value) {
    loadFileInfo()
  } else {
    error.value = '无效的分享链接'
    loading.value = false
  }
})
</script>

<style scoped>
.shared-file-view {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  flex-direction: column;
}

/* Header */
.header {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  padding: 20px 40px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.title {
  color: white;
  margin: 0;
  font-size: 28px;
  font-weight: 600;
}

/* Main Content */
.main-content {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.content-card {
  background: white;
  border-radius: 16px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  padding: 40px;
  max-width: 600px;
  width: 100%;
  text-align: center;
}

.error-card {
  border-left: 4px solid #f56c6c;
}

/* Loading */
.loading-section h3 {
  margin: 20px 0 10px;
  color: #333;
}

.loading-section p {
  color: #666;
  margin: 0;
}

.loading-icon {
  animation: spin 1s linear infinite;
  color: #409eff;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Error */
.error-section h3 {
  margin: 20px 0 16px;
  color: #333;
}

.error-text {
  color: #f56c6c;
  margin: 0 0 30px;
  font-size: 16px;
}

.error-actions {
  display: flex;
  gap: 16px;
  justify-content: center;
}

/* File Info */
.file-section {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.file-icon {
  margin-bottom: 24px;
}

.file-name {
  margin: 0 0 30px;
  color: #333;
  font-size: 24px;
  font-weight: 600;
  word-break: break-word;
}

.file-details {
  width: 100%;
  margin-bottom: 30px;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 20px;
  margin-bottom: 8px;
  background: #f8f9fa;
  border-radius: 8px;
}

.label {
  color: #666;
  font-weight: 500;
}

.value {
  color: #333;
  font-weight: 600;
}

.download-section {
  margin-top: 20px;
}

.download-btn {
  padding: 14px 32px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 8px;
}

/* Empty */
.empty-section h3 {
  margin: 20px 0 16px;
  color: #333;
}

.empty-section p {
  color: #666;
  margin: 0;
}

/* Footer */
.footer {
  background: rgba(0, 0, 0, 0.1);
  color: white;
  text-align: center;
  padding: 20px;
}

.footer p {
  margin: 0;
  opacity: 0.8;
}

/* 响应式 */
@media (max-width: 768px) {
  .header {
    padding: 16px 20px;
    flex-direction: column;
    gap: 16px;
  }

  .title {
    font-size: 24px;
  }

  .main-content {
    padding: 20px 16px;
  }

  .content-card {
    padding: 30px 20px;
  }

  .file-name {
    font-size: 20px;
  }

  .detail-row {
    flex-direction: column;
    gap: 8px;
    text-align: left;
  }

  .error-actions {
    flex-direction: column;
  }

  .download-btn {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .content-card {
    padding: 24px 16px;
    margin: 0 8px;
  }

  .file-name {
    font-size: 18px;
  }

  .detail-row {
    padding: 10px 16px;
  }
}
</style>