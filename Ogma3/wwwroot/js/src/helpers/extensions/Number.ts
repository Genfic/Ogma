declare global {
	interface Number {
		normalize(min: number, max: number): number,

		clamp(min?: number, max?: number): number,
	}
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

export{};