import { readdir } from "node:fs/promises";
import { join } from "node:path";
import Bun from "bun";

const baseDir = join(import.meta.dir, "..", "..", "..", "styles", "themes");
const hasher = new Bun.CryptoHasher("sha256");

export const getThemes = async () => {
	const files = await readdir(baseDir);
	const out: { [key: string]: string } = {};
	for (const filename of files) {
		const path = join(baseDir, filename);
		const file = Bun.file(path);
		const buffer = await file.arrayBuffer();
		hasher.update(buffer);

		out[filename.replace(".scss", "")] = hasher.digest("base64url");
	}
	return out;
};
