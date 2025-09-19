<template>
  <div class="files-view">
    <el-row justify="space-between" align="middle" class="files-header">
      <el-col>
        <h2>我的文件</h2>
      </el-col>
      <el-col>
        <input
          type="file"
          ref="fileInput"
          @change="handleFileSelect"
          style="display: none"
          accept="*/*"
        />
        <el-button type="primary" :icon="Upload" @click="openFileDialog">
          上传文件
        </el-button>
      </el-col>
    </el-row>

    <el-table
      v-loading="loading"
      :data="files"
      empty-text="尚未上传文件"
      style="width: 100%"
    >
      <el-table-column prop="originalName" label="文件名" min-width="200">
        <template #default="{ row }">
          <div class="file-name-cell">
            <el-icon class="file-icon"><Document /></el-icon>
            <span>{{ row.originalName }}</span>
          </div>
        </template>
      </el-table-column>
      <el-table-column prop="fileSize" label="大小" width="120">
        <template #default="{ row }">
          {{ formatFileSize(row.fileSize) }}
        </template>
      </el-table-column>
      <el-table-column prop="contentType" label="类型" width="150" />
      <el-table-column prop="isShared" label="分享状态" width="100" align="center">
        <template #default="{ row }">
          <el-tag v-if="row.isShared" type="success" size="small">已分享</el-tag>
          <el-tag v-else type="info" size="small">私有</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createdAt" label="上传日期" width="180">
        <template #default="{ row }">
          {{ formatDate(row.createdAt) }}
        </template>
      </el-table-column>
      <el-table-column label="操作" width="200" align="center">
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
            <el-dropdown trigger="click" @command="(cmd) => handleCommand(cmd, row)">
              <el-button size="small" type="info" :icon="MoreFilled" />
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="share" :icon="Share">
                    {{ row.isShared ? '管理分享' : '分享' }}
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
          <p v-if="selectedFile.sharedExpiry"><strong>到期：</strong> {{ formatDate(selectedFile.sharedExpiry) }}</p>
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
          >
            创建分享链接
          </el-button>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  Upload,
  Document,
  Download,
  Share,
  Delete,
  MoreFilled
} from '@element-plus/icons-vue'
import type { FileItem } from '@/types/auth'
import { fileService } from '@/services/fileService'

const files = ref<FileItem[]>([])
const loading = ref(false)
const sharing = ref(false)
const shareDialogVisible = ref(false)
const selectedFile = ref<FileItem | null>(null)
const shareUrl = ref('')
const shareExpiryDays = ref(7)
const fileInput = ref<HTMLInputElement>()

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
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString()
}

const beforeUpload = (file: File) => {
  const maxSize = 500 * 1024 * 1024 // 500MB
  if (file.size > maxSize) {
    ElMessage.error('文件大小不能超过500MB')
    return false
  }
  return true
}

const openFileDialog = () => {
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    const file = target.files[0]
    if (beforeUpload(file)) {
      uploadFile(file)
    }
  }
}

const uploadFile = async (file: File) => {
  try {
    await fileService.uploadFile(file)
    ElMessage.success('文件上传成功')
    loadFiles()
  } catch (error) {
    ElMessage.error('文件上传失败')
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
      // 使用分享token来构建分享链接
      shareUrl.value = `${window.location.origin}/shared/${file.sharedToken}`
    }
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
    // 同时更新本地文件信息
    if (selectedFile.value) {
      selectedFile.value.isShared = true
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
  } catch (error) {
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
  padding: 20px;
}

.files-header {
  margin-bottom: 20px;
}

.files-header h2 {
  margin: 0;
  color: #303133;
}

.file-name-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.file-icon {
  color: #409eff;
}

:deep(.el-table) {
  border-radius: 8px;
  overflow: hidden;
}
</style>