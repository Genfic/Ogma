declare let self: ServiceWorkerGlobalScope;

const MANIFEST_URL = "/js/manifest.txt";

export type ConsoleMethods = keyof Pick<typeof console, "error" | "warn" | "info" | "log">;
export type Message = { type: "log"; from: "cache-worker"; level: ConsoleMethods; msg: string; args: unknown[] };

const log = (level: ConsoleMethods, msg: string, ...args: unknown[]) => {
	console[level](msg, ...args);

	self.clients.matchAll().then((clients) => {
		for (const client of clients) {
			client.postMessage({ type: "log", from: "cache-worker", level, msg, args } satisfies Message);
		}
	});
};

const loadManifest = async () => {
	const response = await fetch(MANIFEST_URL);
	const text = await response.text();

	const lines = text.split("\n");

	const stamp = new Date(lines[0]).getTime();

	return {
		stamp: `precache-${stamp}`,
		files: lines.slice(1).map((l) => {
			const [url, hash] = l.split(":");
			return `${url}?v=${hash ?? ""}`;
		}),
	};
};

const precache = async () => {
	const { stamp, files } = await loadManifest();
	const cache = await caches.open(stamp);

	await Promise.all(
		files.map(async (url) => {
			try {
				const res = await fetch(url, { cache: "reload" });
				if (!res.ok) {
					log("warn", "Failed to fetch for precache:", url);
				}

				await cache.put(url, res);
			} catch (err) {
				log("error", "Error precaching", url, err);
			}
		}),
	);

	log("info", `Precached ${files.length} assets into cache ${stamp}`);
};

const clearOldCaches = async () => {
	const { stamp } = await loadManifest();
	const keys = await caches.keys();

	await Promise.all(keys.filter((key) => key !== stamp).map((key) => caches.delete(key)));

	await self.clients.claim();

	log("info", "Cleared old caches. Active cache:", stamp);
};

self.addEventListener("install", (e) => {
	e.waitUntil(precache());
});

self.addEventListener("activate", (e) => {
	e.waitUntil(clearOldCaches());
});
