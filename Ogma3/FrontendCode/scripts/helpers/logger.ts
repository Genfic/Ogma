import { parseArgs } from "node:util";

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

export const log = {
	verbose: (...data: unknown[]) => values.verbose && console.log(data),
};
