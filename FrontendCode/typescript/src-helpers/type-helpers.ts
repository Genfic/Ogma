export function createTypeGuard<T>(...requiredKeys: (keyof T)[]) {
	return function isComplete(partial: Partial<T>): partial is T {
		return requiredKeys.every((key) => partial[key] !== undefined && partial[key] !== null);
	};
}

export const makeEmpty = <T extends object>(obj?: T): Partial<T> => {
	return Object.fromEntries(Object.keys(obj ?? {}).map((k) => [k, undefined])) as Partial<T>;
};
