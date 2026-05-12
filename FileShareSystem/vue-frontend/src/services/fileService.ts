import api from './api'
import type { FileItem, ShareFileResponse } from '@/types/auth'

export const fileService = {
  getFiles() {
    return api.get<FileItem[]>('/files')
  },

  uploadFile(file: File, onProgress?: (percent: number) => void) {
    const formData = new FormData()
    formData.append('file', file)

    return api.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      timeout: 300000, // 5 minutes timeout for large files
      onUploadProgress: (progressEvent) => {
        if (progressEvent.total && onProgress) {
          const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          onProgress(percentCompleted)
        }
      },
    })
  },

  downloadFile(fileId: number) {
    return api.get(`/files/download/${fileId}`, {
      responseType: 'blob',
    })
  },

  deleteFile(fileId: number) {
    return api.delete(`/files/${fileId}`)
  },

  shareFile(fileId: number, expiryDays: number = 0) {
    return api.post<ShareFileResponse>(`/files/${fileId}/share`, { expiryDays })
  },

  unshareFile(fileId: number) {
    return api.delete(`/files/${fileId}/share`)
  },

  getSharedFile(token: string) {
    return api.get(`/files/shared/${token}`, {
      responseType: 'blob',
    })
  },

  getSharedFileInfo(token: string) {
    return api.get(`/files/shared/${token}/info`)
  }
}