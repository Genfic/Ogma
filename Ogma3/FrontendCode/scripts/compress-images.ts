import { basename, dirname, extname, join } from "node:path";
import { Glob } from "bun";
import sharp from "sharp";
import { Logger } from "./helpers/logger";
import { Stopwatch } from "./helpers/stopwatch";

const timer = new Stopwatch();
const logger = new Logger();

const _root = dirname(Bun.main);
const _path = join(_root, "..", "..", "wwwroot", "img");

const images = new Glob("**/*.{png,jpg,jpeg}");

for await (const file of images.scan(_path)) {
	logger.log(`Compressing ${file}...`);

	const dir = dirname(file);
	const name = basename(file, extname(file));

	const srcPath = join(_path, file);
	const webpPath = join(_path, dir, `${name}.webp`);
	const avifPath = join(_path, dir, `${name}.avif`);

	const srcFile = Bun.file(srcPath);
	const webpFile = Bun.file(webpPath);
	const avifFile = Bun.file(avifPath);

	const srcTime = srcFile.lastModified;

	if (!(await webpFile.exists()) || webpFile.lastModified < srcTime) {
		logger.log("\t→ to WebP");
		await sharp(srcPath).webp({ quality: 75 }).toFile(webpPath);
	} else if (!(await avifFile.exists()) || avifFile.lastModified < srcTime) {
		logger.log("\t→ to AVIF");
		await sharp(srcPath).avif({ quality: 65 }).toFile(avifPath);
	} else {
		logger.log("\t→ Compressed files up to date!");
	}
}

logger.log("Done in", timer);
