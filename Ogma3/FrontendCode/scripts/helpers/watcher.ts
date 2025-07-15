import { relative } from "node:path";
import watcher, { type Event, type EventType } from "@parcel/watcher";
import c from "chalk";
import ct from "chalk-template";
import { isVerbose, Logger } from "./logger";

type WatchParams<T> = {
	transformer: (events: Event[]) => T;
	predicate: (result: T) => boolean;
	action: (result: T) => void | Promise<void>;
};

const logger = new Logger();

export const watch = async <T>(
	directory: string,
	on: EventType[],
	{ transformer, predicate, action }: WatchParams<T>,
) => {
	logger.log(c.blue("👀  Watching..."));

	const subscription = await watcher.subscribe(directory, async (err, events) => {
		if (isVerbose()) {
			for (const { type, path } of events) {
				logger.info(ct`{yellow ${type}: ${path}}`);
			}
		}

		if (err) {
			logger.error(c.bgRed(err.message));
			logger.verbose(err);
			return;
		}

		const filteredEvents = events.filter((e) => on.includes(e.type));
		const result = transformer(filteredEvents);
		if (predicate(result)) {
			logger.log(c.blueBright("🔔  Files changed, recompiling!"));
			for (const { path } of filteredEvents) {
				logger.log(c.blueBright.dim(`└→ ${relative(directory, path)}`));
			}
			await action(result);
		}
	});

	process.on("exit", async () => {
		// close watcher when Ctrl-C is pressed
		logger.log("🚪 Closing watcher...");
		await subscription.unsubscribe();
		process.exit(0);
	});
};
