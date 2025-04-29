import { describe, expect, it } from "bun:test";
import { addToDate } from "@h/date-helpers";

describe("addToDate", () => {
	it("adds years correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, { years: 3 });
		expect(result.getFullYear()).toBe(2023);
	});

	it("adds months correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, { months: 2 });
		expect(result.getMonth()).toBe(2);
	});

	it("adds days correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, { days: 30 });
		expect(result.getDate()).toBe(31);
	});

	it("adds hours correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, { hours: 5 });
		expect(result.getHours()).toBe(5);
	});

	it("adds minutes correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, { minutes: 45 });
		expect(result.getMinutes()).toBe(45);
	});

	it("adds seconds correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, { seconds: 30 });
		expect(result.getSeconds()).toBe(30);
	});

	it("adds milliseconds correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, { milliseconds: 500 });
		expect(result.getMilliseconds()).toBe(500);
	});

	it("handles multiple deltas correctly", () => {
		const date = new Date(2020, 0, 1);
		const result = addToDate(date, {
			years: 1,
			months: 1,
			days: 1,
			hours: 1,
			minutes: 1,
			seconds: 1,
			milliseconds: 1,
		});
		expect(result).toEqual(new Date(2021, 1, 2, 1, 1, 1, 1));
	});
});
