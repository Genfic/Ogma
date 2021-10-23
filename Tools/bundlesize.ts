// @ts-ignore
import {recursiveReaddir} from "https://deno.land/x/recursive_readdir/mod.ts";

const path = new URL("../Ogma3/wwwroot/js/dist/", import.meta.url).pathname.substr(1);

class FileData {
    name: string;
    size: number;

    constructor(name: string, size: number) {
        this.name = name;
        this.size = size;
    }
}

const files = (await recursiveReaddir(decodeURI(path)))
    .filter(x => !x.includes('/admin/'))
    .filter(x => x.endsWith('.min.js'))
    .map(x => new FileData(x, Deno.statSync(x).size));
files.sort((a, b) => b.size - a.size);

console.table(files);

let totalSize = 0;

for (const file of files) {
    totalSize += file.size;
}