import { expect, test } from "bun:test";
import { getData } from "../js/src-helpers/dataset-helpers";

test("returns single value from document", () => {
	document.body.innerHTML = `<div data-foo="bar" data-baz="qux"></div>`;
	expect(getData("div", "foo")).toEqual("bar");
});

test("returns multiple values from document", () => {
	document.body.innerHTML = `<div data-foo="bar" data-baz="qux"></div>`;
	expect(getData("div", ["foo", "baz"])).toEqual({ foo: "bar", baz: "qux" });
});

test("returns single value from element", () => {
	document.body.innerHTML = `<div class="ph"><div data-foo="bar" data-baz="qux"></div></div>`;
	const el = document.querySelector(".ph") as HTMLElement;
	expect(getData("div", "foo", el)).toEqual("bar");
});

test("returns multiple values from element", () => {
	document.body.innerHTML = `<div class="ph"><div data-foo="bar" data-baz="qux"></div></div>`;
	const el = document.querySelector(".ph") as HTMLElement;
	expect(getData("div", ["foo", "baz"], el)).toEqual({ foo: "bar", baz: "qux" });
});
