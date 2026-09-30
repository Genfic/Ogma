type SystemStatsUpdate = {
	sampledAt: string;
	hostCpuUsage: string;
	hostMemoryUsed: string;
	hostMemoryAvailable: string;
	hostDiskUsed: string;
	hostDiskAvailable: string;
	ogmaWorkingSet: string;
	ogmaCpuUsage: string;
	garnetMemory: string;
	postgresDatabaseSize: string;
};

const root = document.getElementById("system-stats");
const streamUrl = root?.dataset.streamUrl;

if (root && streamUrl) {
	const eventSource = new EventSource(streamUrl);

	eventSource.addEventListener("message", (event: MessageEvent<string>) => {
		const update = JSON.parse(event.data) as SystemStatsUpdate;

		for (const [key, value] of Object.entries(update)) {
			if (key === "sampledAt") continue;

			const target = root.querySelector<HTMLElement>(`[data-stat="${key}"]`);
			if (target) target.textContent = value;
		}

		const sampledAt = root.querySelector<HTMLTimeElement>("#sampled-at");
		if (sampledAt) {
			const date = new Date(update.sampledAt);
			sampledAt.dateTime = date.toISOString();
			sampledAt.textContent = date.toISOString().slice(0, 19).replace("T", " ");
		}
	});
}