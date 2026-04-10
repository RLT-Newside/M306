import { authenticationAlova } from '../alova';

interface TokenResponse {
  accessToken?: string;
  idToken?: string;
  refreshToken?: string;
}

// Get JWT tokens (dev only)
export const getDevTokens = () =>
  authenticationAlova.Get<TokenResponse>('/dev-tokens', {
    headers: { 'X-CSRF': '1' },
  });
