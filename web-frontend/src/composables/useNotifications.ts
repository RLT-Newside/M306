import { getHttpStatus } from '@/api/httpError';
import { useNotificationStore } from '@/stores/notification';

export interface NotificationMessages {
  notFound: string;
  forbidden: string;
  conflict: string;
  fallback: string;
}

export function useNotifications() {
  const notification = useNotificationStore();

  function showSuccess(message: string): void {
    notification.showSuccess(message);
  }

  function showError(error: unknown, messages: NotificationMessages): number | undefined {
    const status = getHttpStatus(error);

    if (status === 404) {
      notification.showWarning(messages.notFound);
      return status;
    }

    if (status === 401 || status === 403) {
      notification.showError(messages.forbidden);
      return status;
    }

    if (status === 409) {
      notification.showWarning(messages.conflict);
      return status;
    }

    notification.showError(messages.fallback);
    return status;
  }

  return { showSuccess, showError };
}
