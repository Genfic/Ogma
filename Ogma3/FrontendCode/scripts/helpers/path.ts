import { parse } from "node:path";

export const hasExtension = (path: string, ...extensions: string[]): boolean => {
	const ext = parse(path).ext.slice(1);
	return extensions.includes(ext);
};
