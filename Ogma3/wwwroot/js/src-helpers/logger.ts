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
export const log = {
	log: (o: any) => console.log(isObject(o) ? _deepCopy(o) : o),
	info: (o: any) => console.info(isObject(o) ? _deepCopy(o) : o),
	warn: (o: any) => console.warn(isObject(o) ? _deepCopy(o) : o),
	error: (o: any) => console.error(isObject(o) ? _deepCopy(o) : o),
	debug: (o: any) => console.debug(isObject(o) ? _deepCopy(o) : o)
};