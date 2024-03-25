import { defineConfig } from 'vite';

import { nxViteTsPaths } from '@nx/vite/plugins/nx-tsconfig-paths.plugin';
import react from '@vitejs/plugin-react';

export default defineConfig(({ command, mode }) => {
  const config = {
    cacheDir: '../../node_modules/.vite/edit-multiple-customers',

    server: {
      port: 4203,
      host: 'localhost',
    },

    preview: {
      port: 4300,
      host: 'localhost',
    },

    plugins: [react(), nxViteTsPaths()],
    build: {
      rollupOptions: {
        output: {
          entryFileNames: '[name].js',
          chunkFileNames: '[name].js',
          assetFileNames: '[name].[ext]',
        },
      },
    },
    // Uncomment this if you are using workers.
    // worker: {
    //  plugins: [ nxViteTsPaths() ],
    // },

    test: {
      globals: true,
      cache: { dir: '../../node_modules/.vitest' },
      environment: 'jsdom',
      include: ['src/**/*.{test,spec}.{js,mjs,cjs,ts,mts,cts,jsx,tsx}'],
    },
  };

  if (command === 'build') {
    return { ...config, base: mode === 'PROD' ? '/edit-multiple-customers' : `/${mode}/edit-multiple-customers` };
  }

  return config;
});
