// cookieUtils.test.js
import { beforeEach, describe, expect, it } from "bun:test";
import { getCookieValue, setCookie } from "../src-helpers/cookies";

describe("Cookie Utils", () => {
	beforeEach(() => {
		// Clear cookies before each test
		for (const cookie of document.cookie.split(";")) {
			const eqPos = cookie.indexOf("=");
			const name = eqPos > -1 ? cookie.substring(0, eqPos) : cookie;
			document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT`;
		}
	});

	describe("getCookieValue", () => {
		it("should return the value of an existing cookie", () => {
			document.cookie = "testCookie=testValue";
			expect(getCookieValue("testCookie")).toBe("testValue");
		});

		it("should return undefined if the cookie does not exist", () => {
			expect(getCookieValue("nonExistentCookie")).toBeUndefined();
		});

		it("should handle cookies with spaces correctly", () => {
			document.cookie = "testCookie=test Value";
			expect(getCookieValue("testCookie")).toBe("test Value");
		});

		it("should handle multiple cookies correctly", () => {
			document.cookie = "cookie1=value1";
			document.cookie = "cookie2=value2";
			expect(getCookieValue("cookie1")).toBe("value1");
			expect(getCookieValue("cookie2")).toBe("value2");
		});
	});

	describe("setCookie", () => {
		it("should set a cookie with the given name and value", () => {
			setCookie("testCookie", "testValue");
			expect(document.cookie).toContain("testCookie=testValue");
		});
	});
});
