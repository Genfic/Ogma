import { describe, expect, it } from "bun:test";
// normalize.test.js
import { clamp, normalize } from "@h/math-helpers";

describe("normalize", () => {
	it("should normalize a number within the given range to [0, 1]", () => {
		expect(normalize(50, 0, 100)).toBe(0.5);
		expect(normalize(0, 0, 100)).toBe(0);
		expect(normalize(100, 0, 100)).toBe(1);
		expect(normalize(75, 50, 100)).toBe(0.5);
	});

	it("should handle negative ranges correctly", () => {
		expect(normalize(-50, -100, 0)).toBe(0.5);
		expect(normalize(-100, -100, 0)).toBe(0);
		expect(normalize(0, -100, 0)).toBe(1);
	});

	it("should handle cases where min and max are the same", () => {
		expect(normalize(50, 50, 50)).toBe(50);
	});

	it("should handle cases where num is outside the range", () => {
		expect(normalize(150, 0, 100)).toBe(1.5);
		expect(normalize(-50, 0, 100)).toBe(-0.5);
	});
});

describe("clamp", () => {
	it("should clamp a number within the given range", () => {
		expect(clamp(0.5, 0, 1)).toBe(0.5);
		expect(clamp(-0.5, 0, 1)).toBe(0);
		expect(clamp(1.5, 0, 1)).toBe(1);
	});

	it("should use default min and max values if not provided", () => {
		expect(clamp(0.5)).toBe(0.5);
		expect(clamp(-0.5)).toBe(0);
		expect(clamp(1.5)).toBe(1);
	});

	it("should throw an error if max is less than min", () => {
		expect(() => clamp(0.5, 1, 0)).toThrow("Max (0) cannot be less than min (1)");
	});

	it("should handle cases where num is equal to min or max", () => {
		expect(clamp(0, 0, 1)).toBe(0);
		expect(clamp(1, 0, 1)).toBe(1);
	});

	it("should handle cases where min and max are the same", () => {
		expect(clamp(0.5, 0.5, 0.5)).toBe(0.5);
		expect(clamp(0, 0.5, 0.5)).toBe(0.5);
		expect(clamp(1, 0.5, 0.5)).toBe(0.5);
	});
});
