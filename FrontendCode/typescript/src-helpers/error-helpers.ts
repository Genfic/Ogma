import type { HttpValidationProblemDetails } from "@g/types-public";

export type AttemptResult<T, E = Error> = { success: true; value: T } | { success: false; error: E };

type FailableResponse = { data: unknown; statusText: string };

/**
 * Builds an error message from a failed API response. Handles 400 responses that carry
 * an {@link HttpValidationProblemDetails} body (RFC 7807), plain strings, and empty bodies.
 */
export const responseErrorMessage = ({ data, statusText }: FailableResponse): string => {
	if (data == null) {
		return statusText;
	}
	if (typeof data === "string" && data.length > 0) {
		return data;
	}
	if (typeof data !== "object") {
		return statusText;
	}

	const problem = data as Partial<HttpValidationProblemDetails>;
	const fieldErrors = strings(problem.errors);

	if (fieldErrors.length > 0) {
		return fieldErrors.join(", ");
	}
	return problem.detail ?? problem.title ?? statusText;
};

const strings = (value: unknown): string[] => {
	if (typeof value === "string") {
		return [value];
	}
	if (Array.isArray(value)) {
		return value.flatMap(strings);
	}
	if (value !== null && typeof value === "object") {
		return Object.values(value).flatMap(strings);
	}
	return [];
};

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

export const attemptOr = <T, E>(func: () => T, fallback: T): T => (attempt<T, E>(func).success ? func() : fallback);
export const attemptOrAsync = async <T, E>(func: () => Promise<T>, fallback: T): Promise<T> =>
	(await attemptAsync<T, E>(func)).success ? await func() : fallback;
