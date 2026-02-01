import { defineStore } from 'pinia'

export const useNotificationStore = defineStore('notification', {
  state: () => ({
    notifications: JSON.parse(localStorage.getItem('notifications') || '[]')
  }),

  getters: {
    unreadCount: (state) => state.notifications.filter(n => !n.read).length,
    recentNotifications: (state) => state.notifications.slice(0, 10).sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
  },

  actions: {
    addNotification(notification) {
      const newNotification = {
        id: Date.now().toString(),
        type: notification.type || 'info', // success, error, warning, info
        title: notification.title,
        message: notification.message,
        timestamp: new Date().toISOString(),
        read: false,
        icon: notification.icon || '💳'
      }
      
      this.notifications.unshift(newNotification)
      
      // Garder seulement les 50 dernières notifications
      if (this.notifications.length > 50) {
        this.notifications = this.notifications.slice(0, 50)
      }
      
      this.saveToLocalStorage()
    },

    markAsRead(notificationId) {
      const notification = this.notifications.find(n => n.id === notificationId)
      if (notification) {
        notification.read = true
        this.saveToLocalStorage()
      }
    },

    markAllAsRead() {
      this.notifications.forEach(n => n.read = true)
      this.saveToLocalStorage()
    },

    deleteNotification(notificationId) {
      this.notifications = this.notifications.filter(n => n.id !== notificationId)
      this.saveToLocalStorage()
    },

    clearAll() {
      this.notifications = []
      this.saveToLocalStorage()
    },

    saveToLocalStorage() {
      localStorage.setItem('notifications', JSON.stringify(this.notifications))
    }
  }
})
