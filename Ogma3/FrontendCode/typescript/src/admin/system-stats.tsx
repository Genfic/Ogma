import { toCurrentTimezone } from "@h/date-helpers";
import { iso8601, long } from "@h/tinytime-templates";
import { component } from "@h/web-components";
import { convert } from "convert";
import { noShadowDOM } from "solid-element";
import { onCleanup, Show } from "solid-js";

type MemoryUsageDto = {
	totalBytes: number;
	availableBytes: number;
	usedBytes: number;
};

type SystemResourceSnapshot = {
	sampledAt: string;
	cpuUsagePercentage: number | null;
	memory: MemoryUsageDto | null;
	disk: MemoryUsageDto | null;
	ogmaWorkingSetBytes: number;
	ogmaCpuUsagePercentage: number;
	garnetMemoryBytes: number | null;
	postgresDatabaseBytes: number | null;
};

const unavailable = "Unavailable on this platform";
const unknown = "Unavailable";

const formatBytes = (bytes: number) => convert(bytes, "bytes").to("best").toString(2);

const formatPercentage = (percentage: number) => `${percentage.toFixed(1)}%`;

const formatBytesOr = (bytes: number | null, fallback: string) => (bytes === null ? fallback : formatBytes(bytes));

const formatPercentageOr = (percentage: number | null, fallback: string) =>
	percentage === null ? fallback : formatPercentage(percentage);

const formatUsed = (usage: MemoryUsageDto) => {
	const usedPercentage = (usage.usedBytes * 100) / usage.totalBytes;
	return `${formatBytes(usage.usedBytes)} / ${formatBytes(usage.totalBytes)} (${formatPercentage(usedPercentage)})`;
};

const formatUsedOr = (usage: MemoryUsageDto | null, fallback: string) => (usage === null ? fallback : formatUsed(usage));

const formatAvailableOr = (usage: MemoryUsageDto | null, fallback: string) =>
	usage === null ? fallback : formatBytes(usage.availableBytes);

const SystemStats = () => {
	noShadowDOM();

	let snapshot = $signal<SystemResourceSnapshot>();

	const eventSource = new EventSource("/admin/api/system-resources/StreamSystemResources");
	eventSource.addEventListener("message", (event: MessageEvent<string>) => {
		snapshot = JSON.parse(event.data);
	});
	onCleanup(() => eventSource.close());

	return (
		<Show when={snapshot} fallback={<p class="monospace">Waiting for system resource data...</p>}>
			{(data) => {
				const sampledAt = () => toCurrentTimezone(new Date(data().sampledAt));

				return (
					<>
						<p>
							Sampled at{" "}
							<time class="monospace" dateTime={data().sampledAt} title={long.render(sampledAt())}>
								{iso8601.render(sampledAt())}
							</time>
						</p>

						<br />

						<h2>Host</h2>
						<dl class="rows">
							<dt>CPU usage</dt>
							<dd class="tabular">{formatPercentageOr(data().cpuUsagePercentage, unavailable)}</dd>

							<dt>Memory used</dt>
							<dd class="tabular">{formatUsedOr(data().memory, unavailable)}</dd>

							<dt>Memory available</dt>
							<dd class="tabular">{formatAvailableOr(data().memory, unavailable)}</dd>

							<dt>Root filesystem used</dt>
							<dd class="tabular">{formatUsedOr(data().disk, unknown)}</dd>

							<dt>Root filesystem free</dt>
							<dd class="tabular">{formatAvailableOr(data().disk, unknown)}</dd>
						</dl>

						<br />

						<h2>Service usage</h2>
						<dl class="rows">
							<dt>Ogma process memory (working set)</dt>
							<dd class="tabular">{formatBytes(data().ogmaWorkingSetBytes)}</dd>

							<dt>Ogma process CPU</dt>
							<dd class="tabular">{formatPercentage(data().ogmaCpuUsagePercentage)}</dd>

							<dt>Garnet process private memory</dt>
							<dd class="tabular">{formatBytesOr(data().garnetMemoryBytes, unknown)}</dd>

							<dt>PostgreSQL database size</dt>
							<dd class="tabular">{formatBytesOr(data().postgresDatabaseBytes, unknown)}</dd>
						</dl>

						<br />
						<hr />

						<p>
							Garnet and PostgreSQL container CPU and total RAM are not exposed to Ogma by the current deployment.
							PostgreSQL size is the database&rsquo;s disk footprint, not the whole PostgreSQL container or volume.
						</p>
					</>
				);
			}}
		</Show>
	);
};

component("system-stats", {}, SystemStats);
