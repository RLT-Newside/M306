import { ref } from 'vue';
import { defineStore } from 'pinia';

export const useNavigationStore = defineStore('navigation', () => {
  const isNavigationDrawerOpen = ref<boolean>(false);

  function toggleNavigation() {
    isNavigationDrawerOpen.value = !isNavigationDrawerOpen.value;
  }

  return { isNavigationDrawerOpen, toggleNavigation };
});
