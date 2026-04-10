import { ref } from 'vue';
import { defineStore } from 'pinia';
import type { ClientWithSecrets } from '@/models/idsrv/client';

export const useClientStore = defineStore('client', () => {
  const createdClient = ref<ClientWithSecrets | null>(null);

  return { createdClient };
});
