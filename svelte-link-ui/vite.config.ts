import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
	plugins: [tailwindcss(), sveltekit()],
	build: {
		sourcemap: true
	},
	resolve: {
		extensions: ['.mjs', '.js', '.ts', '.json', '.svelte'],
		alias: {
			$components: '/src/lib/components',
			$lib: '/src/lib',
			$routes: '/src/routes'
		}
	},
	optimizeDeps: {
		include: ['lucide-svelte']
	}
});
