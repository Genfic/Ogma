/**
 * Normalizes a number within a given [min, max] range to a [0, 1] range
 * @param num The number to normalize
 * @param min The minimum value the given number can take
 * @param max The maximum value the given number can take
 * @returns {number} The given number normalized into [0, 1] range
 */
export function normalize(num: number, min: number, max: number): number {
	return (num - min) / (max - min);
}

/**
 * Clamps a given number to a given [min, max] range
 * @param num The number to clamp
 * @param min The lower edge to clamp to, by default 0
 * @param max The upper edge to clamp to, by default 1
 */
export function clamp(num: number, min = 0, max = 1): number {
	if (max < min) throw `Max (${max}) cannot be less than min (${min})`;
	if (num < min) return min;
	if (num > max) return max;
	return num;
}
