import path from 'path';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin(), tailwindcss()],
  server: {
    port: 26312,
    cors: true,
    headers: {
      'Access-Control-Allow-Origin': '*',
    },
    fs: {
      allow: ['..', './public'],
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
      'pdfjs-dist': path.resolve(
        __dirname,
        './node_modules/pdfjs-dist/build/pdf',
      ),
    },
  },
  optimizeDeps: {
    include: ['react-pdf', 'pdfjs-dist'],
  },
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          pdfjs: ['pdfjs-dist'],
        },
      },
    },
  },
});
