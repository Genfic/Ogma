// domUtils.test.js
import { describe, expect, it } from "bun:test";
import { parseDom } from "@h/dom";

describe("parseDom", () => {
	it("should parse a simple HTML string into a DOM element", () => {
		const html = '<div class="test">Hello, World!</div>';
		const element = parseDom(html);
		expect(element).not.toBeNull();
		expect(element.tagName).toBe("DIV");
		expect(element.className).toBe("test");
		expect(element.textContent).toBe("Hello, World!");
	});

	it("should parse an HTML string with multiple elements", () => {
		const html = '<div><span class="inner">Inner Text</span></div>';
		const element = parseDom(html);
		expect(element).not.toBeNull();
		expect(element.tagName).toBe("DIV");
		expect(element.querySelector(".inner")).not.toBeNull();
		expect(element.querySelector(".inner")?.textContent).toBe("Inner Text");
	});

	it("should return null if the HTML string is empty", () => {
		const html = "";
		const element = parseDom(html);
		expect(element).toBeNull();
	});

	it("should return null if the HTML string does not contain any elements", () => {
		const html = "Just some text";
		const element = parseDom(html);
		expect(element).toBeNull();
	});

	it("should parse an HTML string with attributes correctly", () => {
		const html = '<a href="https://example.com" id="testLink">Example</a>';
		const element = parseDom(html);
		expect(element).not.toBeNull();
		expect(element.tagName).toBe("A");
		expect(element.getAttribute("href")).toBe("https://example.com");
		expect(element.getAttribute("id")).toBe("testLink");
		expect(element.textContent).toBe("Example");
	});

	it("should parse an HTML string with nested elements correctly", () => {
		const html = '<ul><li class="item">Item 1</li><li class="item">Item 2</li></ul>';
		const element = parseDom(html);
		expect(element).not.toBeNull();
		expect(element.tagName).toBe("UL");
		expect(element.querySelectorAll(".item").length).toBe(2);
		expect(element.querySelectorAll(".item")[0].textContent).toBe("Item 1");
		expect(element.querySelectorAll(".item")[1].textContent).toBe("Item 2");
	});
});
