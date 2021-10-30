// @ts-ignore
import {expandGlob, expandGlobSync} from "https://deno.land/std@0.113.0/fs/mod.ts";

interface FileData {
    name: string;
    size: number;
}

const files: FileData[] = [...expandGlobSync(`/Ogma3/wwwroot/js/dist/**/*.min.js`)]
    .filter(x => !x.path.includes('/admin/'))
    .map(x => ({ name: x.name, size: Deno.statSync(x.path).size }));
files.sort((a, b) => b.size - a.size);

console.table(files);

let totalSize = 0;

for (const file of files) {
    totalSize += file.size;
}

console.log(`TOTAL SIZE: ${totalSize}`)