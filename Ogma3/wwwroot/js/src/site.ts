(async () => {
	// Set Vue error handling
	// @ts-ignore
	Vue.config.errorHandler = (err: Error) => {
		console.info(err.message); // "Oops"
	};
	// @ts-ignore
	Vue.config.ignoredElements = [/o-*/];

	const registerServiceWorker = async () => {
		if ("serviceWorker" in navigator) {
			try {
				const registration = await navigator.serviceWorker.register("/js/dist/sw/cache-service-worker.js", {
					scope: "/",
				});
				if (registration.installing) {
					console.info("Service worker installing");
				} else if (registration.waiting) {
					console.info("Service worker installed");
				} else if (registration.active) {
					console.info("Service worker active");
				}
			} catch (error) {
				console.error(`Registration failed with ${error}`);
			}
		} else {
			console.warn("Service workers are not supported");
		}
	};

	await registerServiceWorker();
})();
