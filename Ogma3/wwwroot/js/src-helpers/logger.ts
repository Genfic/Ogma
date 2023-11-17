/**
 * Check if the given variable is an object
 * @param {any} obj Variable to check
 * @returns {boolean} True if the variable is an object, false if it's a primitive
 */
function _isObject(obj: any): boolean {
	return obj === Object(obj);
}

/**
 * Creates a deep copy of the object through parsing and serializing JSON
 */
function _deepCopy(o: object): object {
	return JSON.parse(JSON.stringify({ ...o, __isCopied__: true }));
}

const _getMessage = (o: any) => _isObject(o) ? _deepCopy(o) : o;

/**
 * Logger object to create better logging experience
 */
export const log = {
	log: (o: any) => console.log(_getMessage(o)),
	info: (o: any) => console.info(_getMessage(o)),
	warn: (o: any) => console.warn(_getMessage(o)),
	error: (o: any) => console.error(_getMessage(o)),
	debug: (o: any) => console.debug(_getMessage(o))
};