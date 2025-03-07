import c from "chalk";
import ct from "chalk-template";
import watcher, { type Event } from "@parcel/watcher";

type WatchParams<T> = {
	verbose: boolean;
	transformer: (events: Event[]) => T;
	predicate: (result: T) => boolean;
	action: (result: T) => void | Promise<void>;
};

export const watch = async <T>(directory: string, { verbose, transformer, predicate, action }: WatchParams<T>) => {
	const log = (...data: unknown[]) => verbose && console.log(data);

	console.log(c.blue("👀 Watching..."));

	const subscription = await watcher.subscribe(directory, async (err, events) => {
		if (verbose) {
			for (const { type, path } of events) {
				console.info(ct`{yellow ${type}: ${path}}`);
			}
		}

		if (err) {
			console.error(c.bgRed(err.message));
			log(err);
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
