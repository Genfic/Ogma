import type { CookieOptions } from "@h/cookies";
import { compact } from "es-toolkit";

const cookieRegex = (name: string) => new RegExp(`(^|;)\\s*${name}\\s*=\\s*([^;]+)`, "i");

/**
 * Hand-rolled fallback for browsers without the Cookie Store API.
 * Loaded on-demand from @h/cookies.
 */

/**
 * Reads cookie value by name
 * @param {string} name Name of the cookie to get value from
 * @returns {string} Returns the value of the cookie
 */
export const getCookieValue = (name: string): string | undefined => {
	const b = document.cookie.match(cookieRegex(name));
	return b?.at(2);
};

/**
 * Sets cookie of name to a value
 * @param {string} name Name of the cookie
 * @param {string} value Value of the cookie
 * @param {CookieOptions} options Options for the cookie
 */
export function setCookie(name: string, value: string, { expires, secure, sameSite, path }: CookieOptions = {}): void {
	const cookie = [
		`${name}=${value}`,
		expires && `expires=${expires instanceof Date ? expires.toUTCString() : new Date(expires).toUTCString()}`,
		secure && "Secure",
		sameSite && `samesite=${sameSite}`,
		path && `path=${path}`,
	];
	document.cookie = compact(cookie).join("; ");
}
