import { readdir } from "node:fs/promises";
import { dirname, join } from "node:path";

const root = dirname(Bun.main);
const _base = join(root, "FrontendCode", "styles", "themes");

export const getThemes = async () => {
	const files = await readdir(_base);
	const out: { [key: string]: string } = {};
	for (const filename of files) {
		const path = join(_base, filename);
		const file = Bun.file(path);
		const buffer = await file.arrayBuffer();
		const hashBuffer = crypto.subtle.digest("SHA-256", buffer);

		out[filename.replace(".scss", "")] = Buffer.from(await hashBuffer).toString("base64url");
	}
	return out;
};
