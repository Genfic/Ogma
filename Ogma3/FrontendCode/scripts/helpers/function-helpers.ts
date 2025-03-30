export async function attempt<T>(
	fn: () => Promise<T>,
	handler: (error: unknown) => void,
	defaultReturn?: T,
): Promise<T | undefined> {
	try {
		return await fn();
	} catch (error) {
		handler(error);
		return defaultReturn ?? undefined;
	}
}

export function attemptSync<T>(fn: () => T, handler: (error: unknown) => void, defaultReturn?: T): T | undefined {
	try {
		return fn();
	} catch (error) {
		handler(error);
		return defaultReturn ?? undefined;
	}
}
