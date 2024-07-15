/// <reference lib="webworker" />
(function (this: ServiceWorkerGlobalScope) {
	const clearCache = async (key: string) => {
		const keys = await caches.keys();
		if (keys.includes(key)) {
			return;
		}

		console.info("New version of JS manifest found, purging SW cache.");
		await Promise.all(keys.map((k) => caches.delete(k)));
		console.info("SW cache purged.");
	};

	this.addEventListener("install", async (event) => {
		const res = await fetch("/manifest.js.json");
		const manifest: Manifest = await res.json();

		await clearCache(manifest.GeneratedAt);

		const paths = Object.keys(manifest.Files).map((k) => `${k}?v=${manifest.Files[k]}`);

		event.waitUntil(caches.open(manifest.GeneratedAt).then((cache) => cache.addAll(paths)));
	});
}).call(self);

interface Manifest {
	GeneratedAt: string;
	Files: { [key: string]: string };
}
