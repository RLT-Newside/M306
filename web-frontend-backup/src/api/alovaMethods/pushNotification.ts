import type { NotificationRequest } from '@/models/pushNotification/notificationRequest';
import { useRequest } from 'alova/client';
import { genericAlovaInstance } from '../alova';

export const sendPushNotificationRequest = (
  apiEndpoint: string,
  notificationRequest: NotificationRequest
) => {
  return useRequest(
    genericAlovaInstance.Post('https://localhost:5001/bffw/proxy', notificationRequest, {
      headers: { 'X-CSRF': '1', target: apiEndpoint, method: 'POST' },
      credentials: 'include',
    }),
    {
      immediate: false,
    }
  );
};
