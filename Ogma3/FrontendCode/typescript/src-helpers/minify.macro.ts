import { randomUUIDv7 } from "bun";
import { transform } from "lightningcss";

export const minifyHtml = (input: string): string => {
	return input
		.split("\n")
		.map((s) => s.trim().replaceAll(/<\s+/g, "<").replaceAll(/\s+>/g, ">"))
		.join("")
		.trim();
};

const encoder = new TextEncoder();
const decoder = new TextDecoder();

export const minifyCss = (input: string): string => {
	const res = transform({
		code: encoder.encode(input),
		sourceMap: false,
		filename: `${randomUUIDv7()}.css`,
		minify: true,
	});

	return decoder.decode(res.code);
};
