/**
 * Reads cookie value by name
 * @param {string} name Name of the cookie to get value from
 * @returns {string} Returns the value of the cookie
 */
// eslint-disable-next-line no-redeclare
export const getCookieValue = (name: string): string => {
	let b = document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)');
	return b ? b.pop() : '';
};

/**
 * Sets cookie of name to a value
 * @param {string} name Name of the cookie
 * @param {string} value Value of the cookie
 * @param {Date|null} expires Expiration date
 * @param {boolean} secure Whether the cookie is secure
 * @param {string|null} sameSite SameSite setting
 */
export function setCookie(name: string, value: string, expires: Date | null = null, secure: boolean = false, sameSite: string | null = null) {
	let cookie = `${name}=${value}`;
	if (expires) cookie += `; expires=${expires.toUTCString()}`;
	if (secure) cookie += `; secure=${String(secure)}`;
	if (sameSite) cookie += `; samesite=${sameSite}`;
	document.cookie = cookie;
}