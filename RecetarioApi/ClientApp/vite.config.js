import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()],
    server: {
        port: 5173,
        proxy: {
            '/api': {
                // Ajusta target al puerto donde ejecutas tu API .NET
                target: 'https://localhost:7230',
                changeOrigin: true,
                secure: false
            }
        }
    }
});