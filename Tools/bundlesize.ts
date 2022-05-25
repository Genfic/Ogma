import { expandGlobSync } from "https://deno.land/std@0.113.0/fs/mod.ts";

interface FileData {
	name: string;
	size: number;
}

const paths = [
	`/Ogma3/wwwroot/js/dist/**/*.js`,
	`/Ogma3/wwwroot/js/bundle/**/*.js`,
];

for (const path of paths) {
	const files: FileData[] = [...expandGlobSync(path)]
		.filter((x) => !x.path.includes('/admin/'))
		.map((x) => ({ name: x.name, size: Deno.statSync(x.path).size }));
	files.sort((a, b) => b.size - a.size);

	console.log(`Filesizes for ${path}`);
	console.table(files);

	let totalSize = 0;

	for (const file of files) {
		totalSize += file.size;
	}

	console.log(`TOTAL SIZE: ${totalSize}`);
}
