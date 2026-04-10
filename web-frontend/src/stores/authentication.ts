import { getUserInfo } from '@/api/alovaMethods/authentication';
import { defineStore } from 'pinia';
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import { useRequest } from 'alova/client';

export const useAuthenticationStore = defineStore('authentication', () => {
  const isAuthenticated = computed(() => userData.value !== undefined);
  const userRoles = computed(() => {
    if (userData.value === undefined) {
      return [];
    }

    const roleClaims = userData.value.filter((claim) => claim.type === 'role');
    return roleClaims.map((roleClaim) => roleClaim.value);
  });

  const route = useRoute();

  function login() {
    window.location.href = `/bffw/login?returnUrl=${route.fullPath}`;
  }

  function logout() {
    if (userData.value !== undefined) {
      const logoutUrlClaims = userData.value.filter((claim) => claim.type === 'bff:logout_url');
      if (logoutUrlClaims.length > 0 && logoutUrlClaims[0]) {
        const logoutUrl = logoutUrlClaims[0].value;
        window.location.href = logoutUrl;
      }
    }
  }

  const { data: userData, send: loadUserData } = useRequest(() => getUserInfo(), {
    immediate: false,
  });

  return { userData, isAuthenticated, userRoles, login, logout, loadUserData };
});
