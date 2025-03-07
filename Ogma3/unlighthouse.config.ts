/// <reference types="unlighthouse" />

export default defineUnlighthouseConfig({
	site: "https://localhost:5001",
	scanner: {
		exclude: ["/rss/*", "/admin/*"],
	},
	puppeteerOptions: {
		channel: "stable",
	},
});
