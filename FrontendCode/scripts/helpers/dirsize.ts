import { Glob } from "bun";

export const dirsize = async (glob: string) => {
	const files = new Glob(glob).scan();

	let size = 0;
	for await (const file of files) {
		const { size: s } = await Bun.file(file).stat();
		size += s;
	}

	return size;
};
