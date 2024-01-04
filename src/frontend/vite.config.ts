import { fileURLToPath, URL } from 'node:url'
import { resolve, dirname } from "node:path";
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import mkcert from 'vite-plugin-mkcert'
import VueI18nPlugin from "@intlify/unplugin-vue-i18n/vite";
import Components from 'unplugin-vue-components/vite'

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 3000,
    https: true,
    hmr: {
      port: 3000,
      host: 'localhost',
      protocol: 'wss'
    }
  },

  build: {
    target: "esnext"
  },
  
  preview: {
    port: 8080
  },

  plugins: [
    vue(),
    mkcert(),
    Components(),
    VueI18nPlugin({include: resolve(dirname(fileURLToPath(import.meta.url)), './src/locales/**')}),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
