import { $, type ShellError } from "bun";

try {
	const text = await $`bunx biome ci --reporter=summary`.text();
	await Bun.write("biome.report.txt", text);
} catch (err) {
	const e = err as ShellError;
	console.log(`Failed with code ${e.exitCode}`);
	console.log(e.stderr.toString());
	await Bun.write("biome.report.e.txt", e.stdout.toString());
}
