import manifest from "@g/manifest.json";

const clearCache = async (key: string) => {
	const keys = await caches.keys();
	if (keys.includes(key)) {
		return;
	}

	console.info("New version of JS manifest found, purging SW cache.");
	await Promise.all(keys.map((k) => caches.delete(k)));
	console.info("SW cache purged.");
};

self.addEventListener("activate", async (event: ExtendableEvent) => {
	await clearCache(manifest.GeneratedAt);

	const paths = Object.entries(manifest.Files).map(([k, v]) => `${k}?v=${v}`);

	event.waitUntil(caches.open(manifest.GeneratedAt).then((cache) => cache.addAll(paths)));
});
