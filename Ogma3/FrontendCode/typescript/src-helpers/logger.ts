/**
 * Check if the given variable is an object
 * @param {any} obj Variable to check
 * @returns {boolean} True if the variable is an object, false if it's a primitive
 */
function _isObject(obj: unknown): boolean {
	return obj === Object(obj);
}

/**
 * Creates a deep copy of the object through parsing and serializing JSON
 */
function _deepCopy(o: object): object {
	return JSON.parse(JSON.stringify({ ...o, __isCopied__: true }));
}

const _getMessage = (o: unknown) => (_isObject(o) ? _deepCopy(o as object) : o);

/**
 * Logger object to create better logging experience
 */
export const log = {
	log: (o: unknown) => console.log(_getMessage(o)),
	info: (o: unknown) => console.info(_getMessage(o)),
	warn: (o: unknown) => console.warn(_getMessage(o)),
	error: (o: unknown) => console.error(_getMessage(o)),
	debug: (o: unknown) => console.debug(_getMessage(o)),
};
