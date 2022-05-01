import { defineConfig } from 'vite';
import preact from '@preact/preset-vite';

export default defineConfig({
    plugins: [
        // preact()
    ],
    build:{
        // generate manifest.json in outDir
        manifest: true,
        rollupOptions: {
            // overwrite default .html entry
            input: {
                main: 'Ogma3/wwwroot/js/src/main.ts',
                // components: 'Ogma3/wwwroot/js/src/native-components/main.ts'
            }
        },
        outDir: 'Ogma3/wwwroot/bundles',
        polyfillModulePreload: false
    },
    server: {
        proxy:{
            '*' : {
                target: 'http://localhost:5001',
                changeOrigin: true
            }
        },
        hmr: {
            protocol: 'ws'
        }
    }
})