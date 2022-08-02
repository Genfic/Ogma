/**
 * Reads cookie value by name
 * @param {string} name Name of the cookie to get value from
 * @returns {string} Returns the value of the cookie
 */
function getCookieValue(name: string): string {
	let b = document.cookie.match("(^|;)\\s*" + name + "\\s*=\\s*([^;]+)");
	return b ? b.pop() : "";
}

/**
 * Sets cookie of name to a value
 * @param {string} name Name of the cookie
 * @param {string} value Value of the cookie
 * @param {Date|null} expires Expiration date
 * @param {boolean} secure Whether the cookie is secure
 * @param {string|null} sameSite SameSite setting
 * @param {string|null} path Path for the cookie
 */
function setCookie(
	name: string, 
	value: string, 
	expires: Date | null = null, 
	secure: boolean = false, 
	sameSite: string | null = null, 
	path?: string | null
) {
	let cookie = `${name}=${value}`;
	if (expires) cookie += `; expires=${expires.toUTCString()}`;
	if (secure) cookie += `; secure=${String(secure)}`;
	if (sameSite) cookie += `; samesite=${sameSite}`;
	if (path) cookie += `; path=${path}`;
	document.cookie = cookie;
}

/**
 * Check if the given variable is an object
 * @param {any} obj Variable to check
 * @returns {boolean} True if the variable is an object, false if it's a primitive
 */
function isObject(obj: any): boolean {
	return obj === Object(obj);
}

/**
 * Creates a deep copy of the object through parsing and serializing JSON
 */
function _deepCopy(o: object): object {
	return JSON.parse(JSON.stringify({ ...o, __isCopied__: true }));
}

/**
 * Logger object to create better logging experience
 */
((window || global) as any).log = {
	log: o => console.log(isObject(o) ? _deepCopy(o) : o),
	info: o => console.info(isObject(o) ? _deepCopy(o) : o),
	warn: o => console.warn(isObject(o) ? _deepCopy(o) : o),
	error: o => console.error(isObject(o) ? _deepCopy(o) : o),
	debug: o => console.debug(isObject(o) ? _deepCopy(o) : o)
};


// Set Vue error handling
// @ts-ignore
Vue.config.errorHandler = function(err) {
	console.info(err.message); // "Oops"
};
Vue.config.ignoredElements = [/o-*/];