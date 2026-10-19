import type { Message } from "./workers/cache-service.worker";

const workers = [{ name: "cache", path: "/js/workers/cache-service.worker.js", scope: "/" }];

const registerServiceWorkers = async () => {
	for (const { name, path, scope } of workers) {
		if ("serviceWorker" in navigator) {
			try {
				const registration = await navigator.serviceWorker.register(path, {
					scope: scope,
				});
				if (registration.installing) {
					console.info(`Service worker ${name} installing`);
				} else if (registration.waiting) {
					console.info(`Service worker ${name} installed`);
				} else if (registration.active) {
					console.info(`Service worker ${name} active`);
				}
			} catch (error) {
				const e = error instanceof Error ? error.message : String(error);
				console.error(`Registration of ${name} failed with ${e}`);
			}

			navigator.serviceWorker.addEventListener("message", (e: MessageEvent<Message>) => {
				if ("type" in e.data && e.data.type === "log") {
					const { level, from, msg, args } = e.data;
					console[level](`%c[${from} says]`, "color: #999", msg, ...args);
				}
			});
		} else {
			console.warn("Service workers are not supported");
		}
	}
};

await registerServiceWorkers();
