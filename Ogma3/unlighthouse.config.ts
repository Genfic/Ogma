/// <reference types="unlighthouse" />
import { defineConfig } from 'unlighthouse'

export default defineConfig({
	// examplebtn-basic
	site: 'https://localhost:5001',
	scanner: {
		exclude: ['/rss/*', '/admin/*']
	},
})