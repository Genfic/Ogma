/**
 * Replaces all instances of `oldCh` character with `newCh`
 * @param {String} oldCh Old character
 * @param {string} newCh New character
 * @returns {string} Resulting string
 */
String.prototype.replaceAll = function(oldCh, newCh) {
    let out = '';
    for (let c of this) {
        if (c === oldCh) out += newCh;
        else             out += c;
    }
    return out;
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