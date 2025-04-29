// timedString.test.js
import { describe, expect, it } from "bun:test";
import { timedString } from "@h/generators";

describe("timedString", () => {
	it("should return a string", () => {
		const result = timedString();
		expect(typeof result).toBe("string");
	});

	it('should return a string in the format "stamp-nonce"', () => {
		const result = timedString();
		const [stamp, nonce] = result.split("-");
		expect(stamp).toBeTruthy();
		expect(nonce).toBeTruthy();
	});

	it("should have a timestamp part that is a valid base-36 number", () => {
		const result = timedString();
		const [stamp] = result.split("-");
		expect(() => Number.parseInt(stamp, 36)).not.toThrow();
	});

	it("should have a nonce part that is a valid base-36 number", () => {
		const result = timedString();
		const [, nonce] = result.split("-");
		expect(() => Number.parseInt(nonce, 36)).not.toThrow();
	});

	it("should return different values on consecutive calls", () => {
		const result1 = timedString();
		const result2 = timedString();
		expect(result1).not.toBe(result2);
	});

	it("should have a timestamp part that is close to the current time", () => {
		const now = Date.now();
		const result = timedString();
		const [stamp] = result.split("-");
		const stampTime = Number.parseInt(stamp, 36);
		expect(stampTime).toBeGreaterThanOrEqual(now - 1000); // Allow for a 1-second difference
		expect(stampTime).toBeLessThanOrEqual(now + 1000); // Allow for a 1-second difference
	});
});
