import { resolve } from "node:path";

export const fileHash = async (path: string) => {
	const resolved = resolve(import.meta.dir, "..", "..", path);

	const file = await Bun.file(resolved).arrayBuffer();
	const hash = await crypto.subtle.digest("SHA-256", file);
	return Buffer.from(hash).toString("hex");
};
