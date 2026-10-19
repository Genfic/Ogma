export const alphaBy =
	<T>(fn: (object: T) => string) =>
	(a: T, b: T) =>
		fn(a).localeCompare(fn(b));
