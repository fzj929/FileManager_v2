<template>
  <div class="files-view">
    <!-- 统计卡片 -->
    <div class="stats-bar">
      <div class="stat-card">
        <el-icon :size="22" color="#409eff"><Files /></el-icon>
        <div class="stat-info">
          <span class="stat-num">{{ files.length }}</span>
          <span class="stat-label">全部文件</span>
        </div>
      </div>
      <div class="stat-card">
        <el-icon :size="22" color="#67c23a"><Share /></el-icon>
        <div class="stat-info">
          <span class="stat-num">{{ sharedCount }}</span>
          <span class="stat-label">已分享</span>
        </div>
      </div>
      <div class="stat-card">
        <el-icon :size="22" color="#e6a23c"><DataLine /></el-icon>
        <div class="stat-info">
          <span class="stat-num">{{ formatFileSize(totalSize) }}</span>
          <span class="stat-label">已用空间</span>
        </div>
      </div>
    </div>

    <!-- 顶部操作栏 -->
    <el-row justify="space-between" align="middle" class="files-header">
      <el-col :span="14">
        <el-input
          v-model="searchQuery"
          placeholder="搜索文件..."
          :prefix-icon="Search"
          clearable
          class="search-input"
        />
      </el-col>
      <el-col :span="10" class="header-actions">
        <input
          type="file"
          ref="fileInput"
          @change="handleFileSelect"
          style="display: none"
          accept="*/*"
        />
        <el-button type="primary" :icon="Upload" @click="openFileDialog" :disabled="uploading">
          上传文件
        </el-button>
        <el-button :icon="Refresh" @click="loadFiles" :loading="loading" circle />
      </el-col>
    </el-row>

    <!-- 上传进度条 -->
    <div v-if="uploading" class="upload-progress-bar">
      <span class="progress-label">
        <el-icon class="progress-icon"><Loading /></el-icon>
        正在上传...
      </span>
      <el-progress
        :percentage="uploadProgress"
        :stroke-width="10"
        striped
        striped-flow
        :duration="10"
        status="success"
        class="progress"
      />
    </div>

    <el-table
      v-loading="loading"
      :data="filteredFiles"
      empty-text="尚未上传文件"
      style="width: 100%"
      class="files-table"
    >
      <el-table-column prop="originalName" label="文件名" min-width="220">
        <template #default="{ row }">
          <div class="file-name-cell">
            <el-icon class="file-icon" :style="{ color: getFileIconColor(row.contentType) }">
              <component :is="getFileIconComponent(row.contentType)" />
            </el-icon>
            <span class="file-name-text">{{ row.originalName }}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column prop="fileSize" label="大小" width="110">
        <template #default="{ row }">
          {{ formatFileSize(row.fileSize) }}
        </template>
      </el-table-column>
      <el-table-column prop="contentType" label="类型" width="160" show-overflow-tooltip />
      <el-table-column prop="isShared" label="分享状态" width="100" align="center">
        <template #default="{ row }">
          <el-tag v-if="row.isShared" type="success" size="small" effect="light">已分享</el-tag>
          <el-tag v-else type="info" size="small" effect="plain">私有</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createdAt" label="上传日期" width="170">
        <template #default="{ row }">
          {{ formatDate(row.createdAt) }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="210" align="center">
        <template #default="{ row }">
          <el-button-group>
            <el-button
              size="small"
              type="primary"
              :icon="Download"
              @click="downloadFile(row)"
            >
              下载
            </el-button>
            <el-dropdown trigger="click" @command="(cmd: string) => handleCommand(cmd, row)">
              <el-button size="small" type="info" :icon="MoreFilled" />
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="share" :icon="Share">
                    {{ row.isShared ? '管理分享' : '分享文件' }}
                  </el-dropdown-item>
                  <el-dropdown-item
                    v-if="row.isShared"
                    command="unshare"
                    :icon="Lock"
                  >
                    取消分享
                  </el-dropdown-item>
                  <el-dropdown-item command="delete" :icon="Delete" divided style="color: #f56c6c">
                    删除
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </el-button-group>
        </template>
      </el-table-column>
    </el-table>

    <!-- Share Dialog -->
    <el-dialog
      v-model="shareDialogVisible"
      title="分享文件"
      width="500px"
    >
      <div v-if="selectedFile">
        <p><strong>文件：</strong> {{ selectedFile.originalName }}</p>
        <div v-if="selectedFile.isShared">
          <p><strong>分享链接：</strong></p>
          <el-input v-model="shareUrl" readonly>
            <template #append>
              <el-button @click="copyShareUrl">复制</el-button>
            </template>
          </el-input>
          <p v-if="selectedFile.sharedExpiry" class="expiry-text">
            <el-icon><Timer /></el-icon>
            <strong> 到期：</strong> {{ formatDate(selectedFile.sharedExpiry) }}
          </p>
          <p v-else class="expiry-text"><el-icon><Infinity /></el-icon> <strong>永久有效</strong></p>
          <el-divider />
          <el-button
            type="danger"
            plain
            :loading="unsharing"
            @click="unshareFile"
            style="width: 100%"
          >
            <el-icon><Lock /></el-icon>
            取消分享
          </el-button>
        </div>
        <div v-else>
          <p>设置文件分享：</p>
          <el-form-item label="到期天数（0 = 无期限）">
            <el-input-number
              v-model="shareExpiryDays"
              :min="0"
              :max="365"
            />
          </el-form-item>
          <el-button
            type="primary"
            :loading="sharing"
            @click="shareFile"
            style="width: 100%; margin-top: 12px"
          >
            创建分享链接
          </el-button>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  Upload,
  Document,
  Download,
  Share,
  Delete,
  MoreFilled,
  Search,
  Refresh,
  Lock,
  Picture,
  VideoCamera,
  Headset,
  Files,
  DataLine,
  Timer,
  Loading
} from '@element-plus/icons-vue'
import type { FileItem } from '@/types/auth'
import { fileService } from '@/services/fileService'

// Infinity icon shim (not in @element-plus/icons-vue, use a simple component)
const Infinity = { render: () => null }

const files = ref<FileItem[]>([])
const loading = ref(false)
const sharing = ref(false)
const unsharing = ref(false)
const uploading = ref(false)
const uploadProgress = ref(0)
const shareDialogVisible = ref(false)
const selectedFile = ref<FileItem | null>(null)
const shareUrl = ref('')
const shareExpiryDays = ref(7)
const fileInput = ref<HTMLInputElement>()
const searchQuery = ref('')

// Computed stats
const sharedCount = computed(() => files.value.filter(f => f.isShared).length)
const totalSize = computed(() => files.value.reduce((sum, f) => sum + f.fileSize, 0))

// Filtered files based on search
const filteredFiles = computed(() => {
  if (!searchQuery.value.trim()) return files.value
  const q = searchQuery.value.toLowerCase()
  return files.value.filter(f => f.originalName.toLowerCase().includes(q) || f.contentType.toLowerCase().includes(q))
})

// File type icon mapping
const getFileIconComponent = (contentType: string) => {
  if (contentType.startsWith('image/')) return Picture
  if (contentType.startsWith('video/')) return VideoCamera
  if (contentType.startsWith('audio/')) return Headset
  if (contentType.includes('zip') || contentType.includes('rar') || contentType.includes('tar') || contentType.includes('gzip') || contentType.includes('7z')) return Files
  return Document
}

const getFileIconColor = (contentType: string) => {
  if (contentType.startsWith('image/')) return '#e6a23c'
  if (contentType.startsWith('video/')) return '#f56c6c'
  if (contentType.startsWith('audio/')) return '#9c27b0'
  if (contentType.includes('zip') || contentType.includes('rar') || contentType.includes('tar')) return '#67c23a'
  return '#409eff'
}

const loadFiles = async () => {
  loading.value = true
  try {
    const response = await fileService.getFiles()
    files.value = response.data
  } catch (error) {
    ElMessage.error('加载文件失败')
  } finally {
    loading.value = false
  }
}

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-CN')
}

const openFileDialog = () => {
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    const file = target.files[0]
    const maxSize = 500 * 1024 * 1024
    if (file.size > maxSize) {
      ElMessage.error('文件大小不能超过 500MB')
      return
    }
    uploadFile(file)
  }
  // Reset so same file can be selected again
  if (fileInput.value) fileInput.value.value = ''
}

const uploadFile = async (file: File) => {
  uploading.value = true
  uploadProgress.value = 0
  try {
    await fileService.uploadFile(file, (percent) => {
      uploadProgress.value = percent
    })
    ElMessage.success('文件上传成功')
    loadFiles()
  } catch (error) {
    ElMessage.error('文件上传失败')
  } finally {
    uploading.value = false
    uploadProgress.value = 0
  }
}

const downloadFile = async (file: FileItem) => {
  try {
    const response = await fileService.downloadFile(file.id)
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', file.originalName)
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
    ElMessage.success('文件下载成功')
  } catch (error) {
    ElMessage.error('文件下载失败')
  }
}

const handleCommand = (command: string, file: FileItem) => {
  if (command === 'share') {
    selectedFile.value = file
    shareDialogVisible.value = true
    if (file.isShared && file.sharedToken) {
      shareUrl.value = `${window.location.origin}/shared/${file.sharedToken}`
    }
  } else if (command === 'unshare') {
    confirmUnshare(file)
  } else if (command === 'delete') {
    deleteFile(file)
  }
}

const shareFile = async () => {
  if (!selectedFile.value) return

  sharing.value = true
  try {
    const response = await fileService.shareFile(selectedFile.value.id, shareExpiryDays.value)
    shareUrl.value = response.data.shareUrl
    if (selectedFile.value) {
      selectedFile.value.isShared = true
      selectedFile.value.sharedToken = response.data.token
      selectedFile.value.sharedExpiry = response.data.expiresAt
    }
    ElMessage.success('文件分享成功')
    loadFiles()
  } catch (error) {
    ElMessage.error('文件分享失败')
  } finally {
    sharing.value = false
  }
}

const unshareFile = async () => {
  if (!selectedFile.value) return

  unsharing.value = true
  try {
    await fileService.unshareFile(selectedFile.value.id)
    if (selectedFile.value) {
      selectedFile.value.isShared = false
      selectedFile.value.sharedToken = undefined
      selectedFile.value.sharedExpiry = null
    }
    shareDialogVisible.value = false
    ElMessage.success('已取消分享')
    loadFiles()
  } catch (error) {
    ElMessage.error('取消分享失败')
  } finally {
    unsharing.value = false
  }
}

const confirmUnshare = async (file: FileItem) => {
  try {
    await ElMessageBox.confirm(
      `确定要取消分享"${file.originalName}"吗？原分享链接将立即失效。`,
      '取消分享',
      {
        confirmButtonText: '确认取消分享',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )
    await fileService.unshareFile(file.id)
    ElMessage.success('已取消分享')
    loadFiles()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('取消分享失败')
    }
  }
}

const copyShareUrl = () => {
  navigator.clipboard.writeText(shareUrl.value)
  ElMessage.success('分享链接已复制到剪贴板')
}

const deleteFile = async (file: FileItem) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除文件"${file.originalName}"吗？`,
      '删除文件',
      {
        confirmButtonText: '删除',
        cancelButtonText: '取消',
        type: 'warning',
      }
    )

    await fileService.deleteFile(file.id)
    ElMessage.success('文件删除成功')
    loadFiles()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('文件删除失败')
    }
  }
}

onMounted(() => {
  loadFiles()
})
</script>

<style scoped>
.files-view {
  padding: 24px;
}

/* Stats Bar */
.stats-bar {
  display: flex;
  gap: 16px;
  margin-bottom: 20px;
}

.stat-card {
  display: flex;
  align-items: center;
  gap: 12px;
  background: white;
  border-radius: 10px;
  padding: 14px 20px;
  flex: 1;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  border: 1px solid #f0f0f0;
}

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-num {
  font-size: 20px;
  font-weight: 700;
  color: #1e293b;
  line-height: 1.2;
}

.stat-label {
  font-size: 12px;
  color: #94a3b8;
  margin-top: 2px;
}

/* Header */
.files-header {
  margin-bottom: 16px;
}

.search-input {
  width: 100%;
  max-width: 360px;
}

.header-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  align-items: center;
}

/* Upload Progress */
.upload-progress-bar {
  background: #ecf5ff;
  border: 1px solid #b3d8ff;
  border-radius: 8px;
  padding: 12px 16px;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  gap: 16px;
}

.progress-label {
  display: flex;
  align-items: center;
  gap: 6px;
  color: #409eff;
  font-size: 14px;
  white-space: nowrap;
  font-weight: 500;
}

.progress-icon {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.progress {
  flex: 1;
}

/* Table */
.files-table {
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.file-name-cell {
  display: flex;
  align-items: center;
  gap: 10px;
}

.file-icon {
  flex-shrink: 0;
  font-size: 18px;
}

.file-name-text {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Expiry text in dialog */
.expiry-text {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-top: 10px;
  color: #606266;
  font-size: 14px;
}

:deep(.el-table) {
  border-radius: 10px;
}

@media (max-width: 768px) {
  .stats-bar {
    flex-direction: column;
  }

  .files-header .el-col {
    width: 100%;
    margin-bottom: 8px;
  }

  .header-actions {
    justify-content: flex-start;
  }
}
</style>