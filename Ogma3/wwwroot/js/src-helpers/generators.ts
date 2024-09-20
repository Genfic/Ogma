/**
 * @description Produces a string that consists of the current UNIX timestamp and a random value, encoded to Base36
 */
export const timedString = () => {
	const stamp = Date.now().toString(36);
	const nonce = Math.random().toString(36).slice(2);
	return `${stamp}-${nonce}`;
}