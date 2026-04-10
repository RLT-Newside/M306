import { authenticationAlova } from '../alova';

// Get user information
export const getUserInfo = () =>
  authenticationAlova.Get<{ type: string; value: string }[]>('/user', {
    headers: { 'X-CSRF': '1' },
  });
