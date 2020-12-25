/**
 * Replaces all instances of `oldCh` character with `newCh`
 * @param {String} oldCh Old character
 * @param {String} newCh New character
 * @returns {string} Resulting string
 */
String.prototype.replaceAll = function(oldCh, newCh) {
    let out = '';
    for (let c of this) {
        out += c === oldCh ? newCh : c;
    }
    return out;
}

/**
 * If the string is null or empty, return the alternative string. If not, return the string itself.
 * @param {String} alternative Alternative string.
 * @returns {String} Source string, or alternative if source is null or empty.
 */
String.prototype.ifNullOrEmpty = function (alternative) {
    return this === null || this.length <= 0
        ? alternative
        : this;
}

/**
 * Converts hexadecimal color, and an alpha into an argb color
 * @param {string} hex Hexadecimal number like `#FFFFFF` or `FFFFFF`
 * @param {number} alpha Opacity value between 0 and 1
 * @returns {string} Resulting RGBA value formatted as `rgba(255, 255, 255, 1)`
 */
function hexToArgb(hex, alpha = 1) {
    let str = hex.replace('#', '');
    let rgb = str.match(/.{1,2}/g);
    rgb = rgb.map(c => parseInt(c, 16));
    return 'rgba('+rgb[0]+', '+rgb[1]+', '+rgb[2]+', '+alpha+')';
}

/**
 * Removes the given element from an array
 * @param {object} element Element to remove
 */
Array.prototype.remove = function (element) {
    let idx = this.indexOf(element);
    if (idx > -1) {
        this.splice(idx, 1)
    }
}

/**
 * Pushes the given element to an array, provided the array doesn't already include it
 * @param {object} element Element to push
 */
Array.prototype.pushUnique = function (element) {
    if (this.includes(element)) return;
    this.push(element);
}

/**
 * Reads cookie value by name
 * @param {String} name Name of the cookie to get value from
 * @returns {String} Returns the value of the cookie
 */
function getCookieValue(name) {
    let b = document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)');
    return b ? b.pop() : '';
}

/**
 * Sets cookie of name to a value
 * @param {String} name Name of the cookie
 * @param {String} value Value of the cookie
 * @param {Date} expires Expiration date
 * @param {Boolean} secure Whether the cookie is secure
 * @param {String} sameSite SameSite setting
 */
function setCookie(name, value, expires = null, secure = false, sameSite = null) {
    let cookie = name + '=' + value;
    if (expires) cookie += '; expires=' + expires.toUTCString();
    if (secure) cookie += '; secure=' + String(secure);
    if (sameSite) cookie += '; samesite=' + sameSite;
    document.cookie = cookie;
}

/**
 * Normalizes a number within a given [min, max] range to a [0, 1] range
 * @param min The minimum value the given number can take
 * @param max The maximum value the given number can take
 * @returns {number} The given number normalized into [0, 1] range
 */
Number.prototype.normalize = function (min, max) {
    return (this - min) / (max - min);
}

Number.prototype.clamp = function (min = 0, max = 1) {
    if (this < min) return min;
    if (this > max) return max;
    return this;
}