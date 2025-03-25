import { compact } from "es-toolkit";

const cookieRegex = (name: string) => new RegExp(`(^|;)\\s*${name}\\s*=\\s*([^;]+)`, "i");

/**
 * Reads cookie value by name
 * @param {string} name Name of the cookie to get value from
 * @returns {string} Returns the value of the cookie
 */
export const getCookieValue = (name: string): string | undefined => {
	const b = document.cookie.match(cookieRegex(name));
	return b?.at(2);
};

export interface CookieOptions {
	expires?: Date;
	secure?: boolean;
	httpOnly?: boolean;
	sameSite?: "Strict" | "Lax" | undefined;
	path?: string;
}

/**
 * Sets cookie of name to a value
 * @param {string} name Name of the cookie
 * @param {string} value Value of the cookie
 * @param {CookieOptions} options Options for the cookie
 */
export function setCookie(
	name: string,
	value: string,
	{ expires, secure, httpOnly, sameSite, path }: CookieOptions = {},
): void {
	const cookie = [
		`${name}=${value}`,
		expires && `expires=${expires.toUTCString()}`,
		secure && "Secure",
		httpOnly && "HttpOnly",
		sameSite && `samesite=${sameSite}`,
		path && `path=${path}`,
	];
	document.cookie = compact(cookie).join("; ");
}
