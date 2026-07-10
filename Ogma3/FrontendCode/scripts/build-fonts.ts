import path, { dirname, join } from "node:path";
import convert from "convert";
import { createFont, type FontEditor, woff2 } from "fonteditor-core";

const asciiChars = Array.from({ length: 95 }, (_, i) => String.fromCodePoint(i + 32)).join("");

const subsets: [name: string, chars: string, scale: number][] = [
	["IMFellEnglish-Italic.ttf", asciiChars, 1.2],
	["IMFellEnglish-Regular.ttf", asciiChars, 1.2],
];

const _root = dirname(Bun.main);
const _source = join(_root, "..", "fonts");
const _dest = join(_root, "..", "..", "wwwroot", "fonts");

await woff2.init();

const supportedFont = (value: string): value is FontEditor.FontType => {
	switch (value) {
		case "ttf":
		case "otf":
		case "eot":
		case "woff":
		case "woff2":
		case "svg":
			return true;
		default:
			return false;
	}
};

const writeBinary = (
	font: FontEditor.Font,
	type: FontEditor.FontType,
	options: Omit<FontEditor.FontWriteOptions, "type">,
): ArrayBuffer | Buffer => {
	const out = font.write({ ...options, type });
	if (typeof out === "string") {
		throw new TypeError(`Unexpected string output for ${type}`);
	}
	return out;
};

const best = (bytes: number) => {
	const { quantity, unit } = convert(bytes, "bytes").to("best");
	return `${quantity.toFixed(3)} ${unit}`;
};

for (const [file, chars, _scale] of subsets) {
	const { ext, name } = path.parse(file);

	const input = join(_source, file);
	const output = join(_dest, name);

	try {
		const buffer = await Bun.file(input).arrayBuffer();
		const ogSize = buffer.byteLength;

		const unicodes = chars
			.split("")
			.map((c) => c.codePointAt(0))
			.filter((c) => c !== undefined);

		const type = ext.slice(1);

		if (!supportedFont(type)) {
			console.error(`Unsupported font type: ${type}`);
			continue;
		}

		const font = createFont(buffer, {
			type,
			subset: unicodes,
			hinting: true,
			kerning: true,
		});

		// const obj = font.get();
		// obj.head.unitsPerE = Math.round(obj.head.unitsPerEm / scale);
		// font.set(obj);

		const woff2buffer = writeBinary(font,"woff2",{ hinting: true });
		const woffbuffer =  writeBinary(font,"woff", {hinting: true });

		await Bun.write(`${output}.woff2`, woff2buffer);
		await Bun.write(`${output}.woff`, woffbuffer);

		console.log(
			`${file} (${best(ogSize)}) -> woff (${best(woffbuffer.byteLength)}) & woff2 (${best(woff2buffer.byteLength)})`,
		);
	} catch (e) {
		console.error(`Error processing ${file}:`, e);
	}
}
