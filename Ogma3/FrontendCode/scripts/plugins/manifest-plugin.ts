import { writeFile } from "node:fs/promises";
import { relative } from "node:path";
import type { BunPlugin } from "bun";

const base64UrlEncode = (buffer: ArrayBuffer) => {
	const bytes = new Uint8Array(buffer);
	let binary = "";
	for (let i = 0; i < bytes.byteLength; i++) {
		binary += String.fromCharCode(bytes[i]);
	}
	const b64 = globalThis.btoa(binary);
	return b64.replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
};

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

				const lines: string[] = [];

				for (const output of result.outputs) {
					if (!output.path || output.kind === "sourcemap") continue;

					const relPath = relative(outdir, output.path).replace(/\\/g, "/");
					const buffer = await output.arrayBuffer();
					const hashBuffer = await crypto.subtle.digest("SHA-256", buffer);
					const vHash = base64UrlEncode(hashBuffer);

					lines.push(`${relPath}:${vHash}`);
				}

				lines.sort();

				const manifest = `${new Date().toISOString()}\n${lines.join("\n")}`;

				await writeFile(`${outdir}/${filename}`, manifest);
			});
		},
	};
}
