import { describe, expect, it } from "bun:test";
import { getData } from "../src-helpers/dataset-helpers";

describe("getData", () => {
	it("returns single value from document", () => {
		document.body.innerHTML = `<div data-foo="bar" data-baz="qux"></div>`;
		expect(getData("div", "foo")).toEqual("bar");
	});

	it("returns multiple values from document", () => {
		document.body.innerHTML = `<div data-foo="bar" data-baz="qux"></div>`;
		expect(getData("div", ["foo", "baz"])).toEqual({ foo: "bar", baz: "qux" });
	});

	it("returns single value from element", () => {
		document.body.innerHTML = `<div class="ph"><div data-foo="bar" data-baz="qux"></div></div>`;
		const el = document.querySelector(".ph") as HTMLElement;
		expect(getData("div", "foo", el)).toEqual("bar");
	});

	it("returns multiple values from element", () => {
		document.body.innerHTML = `<div class="ph"><div data-foo="bar" data-baz="qux"></div></div>`;
		const el = document.querySelector(".ph") as HTMLElement;
		expect(getData("div", ["foo", "baz"], el)).toEqual({ foo: "bar", baz: "qux" });
	});
});
