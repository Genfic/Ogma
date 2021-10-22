/////////////////////////////////////////////////////////////////////////////////////////
///                                                                                   ///
///                              EXTENSION METHODS                                    /// 
///                                                                                   ///
/////////////////////////////////////////////////////////////////////////////////////////

interface String {
	replaceAll(oldCh: string, newCh: string): string,
	ifNullOrEmpty(alternative: string): string,
	properSplit(split: string|RegExp): Array<string>
}

/**
 * Replaces all instances of `oldCh` character with `newCh`
 * @param {string} oldCh Old character
 * @param {string} newCh New character
 * @returns {string} Resulting string
 */
String.prototype.replaceAll = function (oldCh: string, newCh: string): string {
	let out = '';
	for (let c of this) {
		out += c === oldCh ? newCh : c;
	}
	return out;
};

/**
 * If the string is null or empty, return the alternative string. If not, return the string itself.
 * @param {string} alternative Alternative string.
 * @returns {string} Source string, or alternative if source is null or empty.
 */
String.prototype.ifNullOrEmpty = function (alternative: string): string {
	return this === null || this.length <= 0 ? alternative : this;
};

/**
 * Properly splits a string, that is returns an empty array of the string is empty, null, or undefined
 * @param {string|RegExp} split What to split the string on
 * @returns {Array<string>}
 */
String.prototype.properSplit = function (split: string|RegExp): Array<string> {
	return this.length === 0 || this === null || this === undefined ? [] : this.split(split);
};
 
interface Array<T> {
	remove(element: T): void,
	pushUnique(element: T): void,
}

/**
 * Removes the given element from an array
 * @param {any} element Element to remove
 */
Array.prototype.remove = function (element): void {
	let idx = this.indexOf(element);
	if (idx > -1) {
		this.splice(idx, 1);
	}
};

/**
 * Pushes the given element to an array, provided the array doesn't already include it
 * @param {object} element Element to push
 */
Array.prototype.pushUnique = function (element) {
	if (this.includes(element)) return;
	this.push(element);
};

interface Number {
	normalize(min: number, max: number): number,
	clamp(min: number, max: number): number,
}

/**
 * Normalizes a number within a given [min, max] range to a [0, 1] range
 * @param {number} min The minimum value the given number can take
 * @param {number} max The maximum value the given number can take
 * @returns {number} The given number normalized into [0, 1] range
 */
Number.prototype.normalize = function (min: number, max: number): number {
	if (max < min) throw 'Max cannot be less than min';
	return (this - min) / (max - min);
};

/**
 * Clamps a given number to a given [min, max] range
 * @param {number} min The lower edge to clamp to, by default 0
 * @param {number} max The upper edge to clamp to, by default 1
 * @returns {number}
 */
Number.prototype.clamp = function (min: number = 0, max: number = 1): number {
	if (max < min) throw 'Max cannot be less than min';
	if (this < min) return min;
	if (this > max) return max;
	return this;
};

/**
 * Check if the given variable is an object
 * @param {any} obj Variable to check
 * @returns {boolean} True if the variable is an object, false if it's a primitive
 */
const isObject = (obj: any): boolean => obj === Object(obj);


/**
 * Creates a deep copy of the object through parsing and serializing JSON
 */
const _deepCopy = function(o: object): object {
	return JSON.parse(JSON.stringify({...o, __isCopied__: true}));
}


/////////////////////////////////////////////////////////////////////////////////////////
///                                                                                   ///
///                              GLOBAL FUNCTIONS                                     /// 
///                                                                                   ///
/////////////////////////////////////////////////////////////////////////////////////////


/**
 * Converts hexadecimal color, and an alpha into an argb color
 * @param {string} hex Hexadecimal number like `#FFFFFF` or `FFFFFF`
 * @param {number} alpha Opacity value between 0 and 1
 * @returns {string} Resulting RGBA value formatted as `rgba(255, 255, 255, 1)`
 */
const hexToRgba = (hex: string, alpha: number = 1): string => {
	let str = hex.replace('#', '');
	let values = str.match(/.{1,2}/g);
	let rgb = values.map((c: string) => parseInt(c, 16));
	return `rgba(${rgb[0]}, ${rgb[1]}, ${rgb[2]}, ${alpha})`;
} 

/**
 * Reads cookie value by name
 * @param {string} name Name of the cookie to get value from
 * @returns {string} Returns the value of the cookie
 */
const getCookieValue = (name: string): string => {
	let b = document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)');
	return b ? b.pop() : '';
}

/**
 * Sets cookie of name to a value
 * @param {string} name Name of the cookie
 * @param {string} value Value of the cookie
 * @param {Date|null} expires Expiration date
 * @param {boolean} secure Whether the cookie is secure
 * @param {string|null} sameSite SameSite setting
 */
const setCookie = (name: string, value: string, expires: Date|null = null, secure: boolean = false, sameSite: string|null = null) => {
	let cookie = `${name}=${value}`;
	if (expires) cookie += `; expires=${expires.toUTCString()}`;
	if (secure) cookie += `; secure=${String(secure)}`;
	if (sameSite) cookie += `; samesite=${sameSite}`;
	document.cookie = cookie;
}

/**
 * Logger object to create better logging experience
 * @type {{warn: (function(*): void), debug: (function(*): void), log: (function(*): void), error: (function(*): void), info: (function(*): void)}}
 */
((window || global) as any).log = {
	log:	o => console.log(isObject(o) ? _deepCopy(o) : o),
	info:	o => console.info(isObject(o) ? _deepCopy(o) : o),
	warn:	o => console.warn(isObject(o) ? _deepCopy(o) : o),
	error:	o => console.error(isObject(o) ? _deepCopy(o) : o),
	debug:	o => console.debug(isObject(o) ? _deepCopy(o) : o)
}


// Set Vue error handling
// @ts-ignore
Vue.config.errorHandler = function (err) {
	log.info(err.message); // "Oops"
};