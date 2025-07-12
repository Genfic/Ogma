export function $try<T>(
	fn: () => Promise<T>,
	handler: (error: unknown) => void,
	defaultReturn?: T,
): Promise<T | undefined>;

export function $try<T>(fn: () => T, handler: (error: unknown) => void, defaultReturn?: T): T | undefined;

export function $try<T>(
	fn: () => T | Promise<T>,
	handler: (error: unknown) => void,
	defaultReturn?: T,
): (T | undefined) | Promise<T | undefined> {
	try {
		const result = fn();

		if (result instanceof Promise) {
			return result.catch((error) => {
				handler(error);
				return defaultReturn ?? undefined;
			});
		}

		return result;
	} catch (error) {
		handler(error);
		return defaultReturn ?? undefined;
	}
}
