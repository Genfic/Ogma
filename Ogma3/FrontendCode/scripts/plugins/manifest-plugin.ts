import { relative } from "node:path";
import type { BunPlugin } from "bun";
import { Parallel } from "../helpers/promises";

export interface ManifestOptions {
	filename?: string;
}

export function manifestPlugin(options?: ManifestOptions): BunPlugin {
	const { filename = "manifest.txt" } = options ?? {};

	return {
		name: "manifest-plugin",
		setup(build) {
			build.onEnd(async (result) => {
				const outdir = build.config.outdir;
				if (!outdir || !result.outputs) return;

				const lines = await Parallel.forEach(
					result.outputs.filter((o) => !!o.path && o.kind !== "sourcemap"),
					async (output) => {
						const relPath = relative(outdir, output.path).replace(/\\/g, "/");
						const buffer = await output.arrayBuffer();
						const hashBuffer = await crypto.subtle.digest("SHA-256", buffer);
						const vHash = Buffer.from(hashBuffer).toString("base64url");

						return `${relPath}:${vHash}`;
					},
				);

				lines.sort((a, b) => a.localeCompare(b));

				const manifest = `${new Date().toISOString()}\n${lines.join("\n")}`;

				await Bun.write(`${outdir}/${filename}`, manifest);
			});
		},
	};
}
