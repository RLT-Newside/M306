import { defineStore } from 'pinia';
import { ref } from 'vue';

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

interface Notification {
  id: number;
  message: string;
  type: NotificationType;
  timeout: number;
}

export const useNotificationStore = defineStore('notification', () => {
  const notifications = ref<Notification[]>([]);
  let nextId = 0;

  function show(message: string, type: NotificationType = 'info', timeout: number = 3000) {
    const id = nextId++;
    notifications.value.push({ id, message, type, timeout });
  }

  function showSuccess(message: string, timeout: number = 3000) {
    show(message, 'success', timeout);
  }

  function showError(message: string, timeout: number = 5000) {
    show(message, 'error', timeout);
  }

  function showWarning(message: string, timeout: number = 4000) {
    show(message, 'warning', timeout);
  }

  function showInfo(message: string, timeout: number = 3000) {
    show(message, 'info', timeout);
  }

  function remove(id: number) {
    const index = notifications.value.findIndex((n) => n.id === id);
    if (index !== -1) {
      notifications.value.splice(index, 1);
    }
  }

  return {
    notifications,
    show,
    showSuccess,
    showError,
    showWarning,
    showInfo,
    remove,
  };
});
