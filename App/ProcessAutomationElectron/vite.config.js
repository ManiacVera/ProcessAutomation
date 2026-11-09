import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  root: ".",
  base: "./",
  build: {
    outDir: "dist",
    rollupOptions: {
      input: {
        main: path.resolve(__dirname, 'index.html'),
        dialog: path.resolve(__dirname, 'dialog.html')
      }
    }
  },
  resolve: {
    alias: {
      "@pages": path.resolve(__dirname, "Pages"),
      "@scripts": path.resolve(__dirname, "Scripts")
    }
  }
})