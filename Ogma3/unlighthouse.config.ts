/// <reference types="unlighthouse" />

export default defineUnlighthouseConfig({
	site: "https://localhost:5001",
	cache: false,
	scanner: {
		exclude: ["/rss/*", "/admin/*"],
		samples: 3,
	},
	puppeteerOptions: {
		channel: "stable",
	},
});
