import * as $path from "node:path";

export const hasExtension = (path: string, ...extensions: string[]): boolean => {
	const ext = $path.parse(path).ext.slice(1);
	return extensions.includes(ext);
};
