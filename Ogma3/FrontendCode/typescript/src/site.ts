const workers = [{ name: "cache", path: "/js/workers/cache-service-worker.js", scope: "/" }];

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
				console.error(`Registration of ${name} failed with ${error}`);
			}
		} else {
			console.warn("Service workers are not supported");
		}
	}
};

await registerServiceWorkers();

export {};
