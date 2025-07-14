import { defineConfig } from 'vite';
import tailwindcss from '@tailwindcss/vite'
import plugin from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin(), tailwindcss()],
  server: {
    port: 13022,
  }
})
