import { brotliCompressSync } from "node:zlib";
import { Glob } from "bun";

const libs = new Glob("../../wwwroot/lib/**/*.*");

for await (const file of libs.scan()) {
	console.log(file);
	const content = await Bun.file(file).text();
	await Bun.write(`${file}.gz`, Bun.gzipSync(content));
	await Bun.write(`${file}.br`, brotliCompressSync(content));
	await Bun.write(`${file}.zst`, Bun.zstdCompressSync(content));
}
