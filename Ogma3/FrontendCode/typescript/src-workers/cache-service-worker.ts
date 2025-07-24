import manifest from "@g/manifest.json";

declare let self: ServiceWorkerGlobalScope;

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
	await clearCache(manifest.generated);

	const paths = manifest.files.map((p) => {
		const [file, hash] = p.split(":");
		return `/js/${file}.js?v=${hash}`;
	});

	event.waitUntil(caches.open(manifest.generated).then((cache) => cache.addAll(paths)));
});
