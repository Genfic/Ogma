import { parseArgs } from "node:util";
import ct from "chalk-template";
import { compact } from "es-toolkit";
import type { Stopwatch } from "./stopwatch";

const { values } = parseArgs({
	args: Bun.argv,
	options: {
		verbose: {
			type: "boolean",
			short: "v",
		},
	},
	strict: false,
	allowPositionals: true,
});

export const isVerbose = () => !!values.verbose;

export class Logger {
	private readonly prefix?: string;
	private readonly prefixPadding: number;

	constructor(prefix?: string, prefixPadding?: number) {
		this.prefix = prefix;
		this.prefixPadding = prefixPadding ?? prefix?.length ?? 0;
	}

	private getTime = (stopwatch: Stopwatch, precision?: number) => {
		const { time, unit } = stopwatch.lap(precision ?? 3);
		return ct`{reset.bold {underline ${time}} ${unit}}`;
	};

	private args = (msg: unknown, stopwatch?: Stopwatch) => {
		return compact([this.prefix?.padEnd(this.prefixPadding + 2), msg, stopwatch && this.getTime(stopwatch)]);
	};

	public log(msg: unknown, stopwatch?: Stopwatch) {
		console.log(...this.args(msg, stopwatch));
	}

	public info(msg: unknown, stopwatch?: Stopwatch) {
		console.info("ℹ️", ...this.args(msg, stopwatch));
	}

	public warn(msg: unknown, stopwatch?: Stopwatch) {
		console.warn("⚠️", ...this.args(msg, stopwatch));
	}

	public error(msg: unknown, stopwatch?: Stopwatch) {
		console.error("❌", ...this.args(msg, stopwatch));
	}

	public verbose(msg: unknown, stopwatch?: Stopwatch) {
		if (isVerbose()) {
			this.log(msg, stopwatch);
		}
	}
}
