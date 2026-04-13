import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import vuetify from 'vite-plugin-vuetify';
import basicSsl from '@vitejs/plugin-basic-ssl';

export default defineConfig({
  plugins: [
    vue(),
    vuetify({
      autoImport: true,
    }),
    basicSsl(),
  ],
  build: {
    chunkSizeWarningLimit: 700,
  },
  server: {
    port: 5173,
    proxy: {
      '/bffw': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        secure: false,
        xfwd: true,
        followRedirects: false,
      },
      '/v2': {
        target: 'http://localhost:8080',
        changeOrigin: true,
        secure: false,
      },
    },
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  envPrefix: 'GIBZAPP',
});
