import { join, relative } from "node:path";
import { Glob } from "bun";

/**
 * Processes and expands SCSS `@use` statements in the given SCSS string.
 *
 * The function scans the input SCSS for `@use` statements and replaces any wildcard-import
 * paths (indicated by `*`) with their fully resolved file names. Non-`@use` statements
 * are returned unchanged. For `@use` statements with wildcard paths, it utilizes file
 * globbing to identify matching SCSS files in the specified base path, resolves their
 * relative paths, and generates the corresponding `@use` statements.
 *
 * @param scss - The input SCSS string that may contain `@use` statements.
 * @param basePath - The base directory path to resolve wildcard `@use` imports.
 * @return An array of processed SCSS lines where wildcard imports
 *                    are expanded into specific `@use` statements.
 */
export const expandScss = (scss: string, basePath: string) =>
	scss.split("\n").flatMap((l) => {
		if (!l.startsWith("@use")) return [l];

		const p = l.split('"')[1];

		console.log(p);

		if (!p.endsWith("*")) {
			return [l];
		}

		const g = join(basePath, `${p}.scss`);
		return [...new Glob(g).scanSync()]
			.map((i) => relative(basePath, i))
			.map((i) => i.replace(".scss", ""))
			.map((i) => i.replaceAll("\\", "/"))
			.map((i) => `@use "${i}";`);
	});
