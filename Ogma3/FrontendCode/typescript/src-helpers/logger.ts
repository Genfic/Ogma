/**
 * Check if the given variable is an object
 * @param {any} obj Variable to check
 * @returns {boolean} True if the variable is an object, false if it's a primitive
 */
function isObject(obj: unknown): boolean {
	return obj === Object(obj);
}

/**
 * Creates a deep copy of the object through parsing and serializing JSON
 */
function deepCopy(o: object): object {
	return JSON.parse(JSON.stringify({ ...o, __isCopied__: true }));
}

const getMessage = (o: unknown) => (import.meta.env.DEV ? (isObject(o) && o instanceof Object ? deepCopy(o) : o) : o);

/**
 * Logger object to create a better logging experience
 */
export const log = {
	log: (o: unknown) => console.log(getMessage(o)),
	info: (o: unknown) => console.info(getMessage(o)),
	warn: (o: unknown) => console.warn(getMessage(o)),
	error: (o: unknown) => console.error(getMessage(o)),
	debug: (o: unknown) => console.debug(getMessage(o)),
};
