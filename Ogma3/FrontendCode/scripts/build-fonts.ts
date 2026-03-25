import path, { dirname, join } from "node:path";
import convert from "convert";
import { createFont, type FontEditor, woff2 } from "fonteditor-core";

const asciiChars = Array.from({ length: 95 }, (_, i) => String.fromCharCode(i + 32)).join("");

const subsets: [name: string, chars: string, scale: number][] = [
	["IMFellEnglish-Italic.ttf", asciiChars, 1.2],
	["IMFellEnglish-Regular.ttf", asciiChars, 1.2],
];

const _root = dirname(Bun.main);
const _source = join(_root, "..", "fonts");
const _dest = join(_root, "..", "..", "wwwroot", "fonts");

await woff2.init();

for (const [file, chars, scale] of subsets) {
	const { ext, name } = path.parse(file);

	const input = join(_source, file);
	const output = join(_dest, name);

	try {
		const buffer = await Bun.file(input).arrayBuffer();
		const ogSize = buffer.byteLength;

		const unicodes = chars.split("").map((c) => c.charCodeAt(0));

		const font = createFont(buffer, {
			type: ext.slice(1) as FontEditor.FontType,
			subset: unicodes,
			hinting: true,
			kerning: true,
		});

		// const obj = font.get();
		// obj.head.unitsPerE = Math.round(obj.head.unitsPerEm / scale);
		// font.set(obj);

		const woff2buffer = Buffer.from(font.write({ type: "woff2", hinting: true }) as ArrayBuffer);
		const woffbuffer = Buffer.from(font.write({ type: "woff", hinting: true }) as ArrayBuffer);

		await Bun.write(`${output}.woff2`, woff2buffer);
		await Bun.write(`${output}.woff`, woffbuffer);

		const c = (bytes: number) => {
			const { quantity, unit } = convert(bytes, "bytes").to("best");
			return `${quantity.toFixed(3)} ${unit}`;
		};

		console.log(
			`${file} (${c(ogSize)}) -> woff (${c(woffbuffer.byteLength)}) & woff2 (${c(woff2buffer.byteLength)})`,
		);
	} catch (e) {
		console.error(`Error processing ${file}:`, e);
	}
}
