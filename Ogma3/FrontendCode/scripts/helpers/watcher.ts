import watcher, { type Event } from "@parcel/watcher";
import c from "chalk";
import ct from "chalk-template";
import { isVerbose, log } from "./logger";

type WatchParams<T> = {
	transformer: (events: Event[]) => T;
	predicate: (result: T) => boolean;
	action: (result: T) => void | Promise<void>;
};

export const watch = async <T>(directory: string, { transformer, predicate, action }: WatchParams<T>) => {
	console.log(c.blue("👀 Watching..."));

	const subscription = await watcher.subscribe(directory, async (err, events) => {
		if (isVerbose()) {
			for (const { type, path } of events) {
				console.info(ct`{yellow ${type}: ${path}}`);
			}
		}

		if (err) {
			console.error(c.bgRed(err.message));
			log.verbose(err);
			return;
		}

		const result = transformer(events);
		if (predicate(result)) {
			console.log(c.blueBright("🔔 Files changed, recompiling!"));
			await action(result);
		}
	});

	process.on("SIGINT", async () => {
		// close watcher when Ctrl-C is pressed
		console.log("🚪 Closing watcher...");
		await subscription.unsubscribe();
		process.exit(0);
	});
};
