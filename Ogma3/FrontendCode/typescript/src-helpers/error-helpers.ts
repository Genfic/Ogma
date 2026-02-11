export type AttemptResult<T, E = Error> = { success: true; value: T } | { success: false; error: E };

export const attempt = <T, E>(func: () => T): AttemptResult<T, E> => {
	try {
		const res = func();
		return { success: true as const, value: res };
	} catch (e) {
		return { success: false as const, error: e as E };
	}
};

export const attemptAsync = async <T, E>(func: () => Promise<T>): Promise<AttemptResult<T, E>> => {
	try {
		const res = await func();
		return { success: true as const, value: res };
	} catch (e) {
		return { success: false as const, error: e as E };
	}
};
