export interface CookieOptions {
	expires?: Date | number;
	secure?: boolean;
	sameSite?: "Strict" | "Lax" | undefined;
	path?: string;
}

/**
 * Reads a cookie value by name
 * @param {string} name Name of the cookie to get value from
 * @returns {string | undefined} Value of the cookie, if present
 */
export const getCookieValue = async (name: string): Promise<string | undefined> => {
	if (typeof window.cookieStore !== "undefined") {
		return (await window.cookieStore.get(name))?.value;
	}
	const { getCookieValue: fallback } = await import("@h/cookies-legacy");
	return fallback(name);
};

const samesite = { Strict: "strict", Lax: "lax" } as const;

/**
 * Sets a cookie with the given name, value and options
 * @param {string} name Name of the cookie
 * @param {string} value Value of the cookie
 * @param {CookieOptions} options Options for the cookie
 */
export const setCookie = async (name: string, value: string, options: CookieOptions = {}): Promise<void> => {
	if (typeof window.cookieStore !== "undefined") {
		await window.cookieStore.set({
			name,
			value,
			expires: options.expires instanceof Date ? options.expires.getTime() : options.expires,
			sameSite: options.sameSite ? samesite[options.sameSite] : undefined,
			path: options.path,
		});
		return;
	}
	const { setCookie: fallback } = await import("@h/cookies-legacy");
	fallback(name, value, options);
};
