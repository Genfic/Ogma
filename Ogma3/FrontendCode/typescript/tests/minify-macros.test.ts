import { describe, expect, it } from "bun:test";
import { minifyHtml } from "../src-helpers/minify.macro";

describe("minifyHtml", () => {
	it("should minify a simple HTML string", () => {
		const input = `
			<div>
				<p>Hello, World!</p>
			</div>
		`;
		const expected = "<div><p>Hello, World!</p></div>";
		const result = minifyHtml(input);
		expect(result).toBe(expected);
	});

	it("should handle nested elements correctly", () => {
		const input = `
			<ul>
				<li>Item 1</li>
				<li>Item 2</li>
			</ul>
		`;
		const expected = "<ul><li>Item 1</li><li>Item 2</li></ul>";
		const result = minifyHtml(input);
		expect(result).toBe(expected);
	});

	it("should handle attributes correctly", () => {
		const input = `
			<a href="https://example.com" id="testLink">Example</a>
		`;
		const expected = '<a href="https://example.com" id="testLink">Example</a>';
		const result = minifyHtml(input);
		expect(result).toBe(expected);
	});

	it("should handle self-closing tags correctly", () => {
		const input = `
			<img src="image.png" alt="Example" />
		`;
		const expected = '<img src="image.png" alt="Example" />';
		const result = minifyHtml(input);
		expect(result).toBe(expected);
	});

	it("should handle empty input correctly", () => {
		const input = "";
		const expected = "";
		const result = minifyHtml(input);
		expect(result).toBe(expected);
	});

	it("should handle input with only whitespace correctly", () => {
		const input = "   \n   \n   ";
		const expected = "";
		const result = minifyHtml(input);
		expect(result).toBe(expected);
	});

	it("should handle input with mixed content correctly", () => {
		const input = `
			<div>
				<p>Hello, <strong>World</strong>!</p>
				<ul>
					<li>Item 1</li>
					<li>Item 2</li>
				</ul>
			</div>
		`;
		const expected = "<div><p>Hello, <strong>World</strong>!</p><ul><li>Item 1</li><li>Item 2</li></ul></div>";
		const result = minifyHtml(input);
		expect(result).toBe(expected);
	});
});
