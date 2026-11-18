import { beforeEach, describe, expect, it, mock } from "bun:test";
import { getCookieValue, setCookie } from "@h/cookies";

const writableWindow = window as Omit<typeof window, "cookieStore"> & { cookieStore?: CookieStore };

const clearCookies = () => {
	for (const cookie of document.cookie.split(";")) {
		const eqPos = cookie.indexOf("=");
		const name = eqPos > -1 ? cookie.substring(0, eqPos) : cookie;
		document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 GMT`;
	}
};

describe("Cookie Utils", () => {
	beforeEach(() => {
		clearCookies();
		delete writableWindow.cookieStore;
	});

	describe("Modern API (cookieStore)", () => {
		it("should delegate get and set to window.cookieStore", async () => {
			const get = mock((_name: string) => Promise.resolve({ name: "testCookie", value: "testValue" }));
			const set = mock((_init: CookieInit) => Promise.resolve());
			writableWindow.cookieStore = { get, set } as unknown as CookieStore;

			expect(await getCookieValue("testCookie")).toBe("testValue");
			expect(get).toHaveBeenCalledWith("testCookie");

			const expires = new Date("2030-01-01T00:00:00Z");
			await setCookie("testCookie", "testValue", { expires, sameSite: "Lax" });
			expect(set).toHaveBeenCalledWith({
				name: "testCookie",
				value: "testValue",
				expires: expires.getTime(),
				sameSite: "lax",
				path: undefined,
			});
		});

		it("should return undefined if the cookie does not exist", async () => {
			const get = mock((_name: string) => Promise.resolve(null));
			writableWindow.cookieStore = {
				get,
				set: mock((_init: CookieInit) => Promise.resolve()),
			} as unknown as CookieStore;

			expect(await getCookieValue("nonExistentCookie")).toBeUndefined();
		});
	});

	describe("Legacy fallback", () => {
		it("should return the value of an existing cookie", async () => {
			document.cookie = "testCookie=testValue";
			expect(await getCookieValue("testCookie")).toBe("testValue");
		});

		it("should return undefined if the cookie does not exist", async () => {
			expect(await getCookieValue("nonExistentCookie")).toBeUndefined();
		});

		it("should handle cookies with spaces correctly", async () => {
			document.cookie = "testCookie=test Value";
			expect(await getCookieValue("testCookie")).toBe("test Value");
		});

		it("should handle multiple cookies correctly", async () => {
			document.cookie = "cookie1=value1";
			document.cookie = "cookie2=value2";
			expect(await getCookieValue("cookie1")).toBe("value1");
			expect(await getCookieValue("cookie2")).toBe("value2");
		});

		it("should set a cookie with the given name and value", async () => {
			await setCookie("testCookie", "testValue");
			expect(document.cookie).toContain("testCookie=testValue");
		});
	});
});
