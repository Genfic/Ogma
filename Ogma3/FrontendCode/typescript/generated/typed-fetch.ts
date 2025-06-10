export type SuccessResponse<TData> = {
	readonly ok: true;
	readonly status: number;
	readonly statusText: string;
	readonly headers: Headers;
	readonly data: TData;
};

export type ErrorResponse<TError> = {
	readonly ok: false;
	readonly status: number;
	readonly statusText: string;
	readonly headers: Headers;
	readonly error: TError;
};

export type TypedResponse<TData, TError> =
	| SuccessResponse<TData>
	| ErrorResponse<TError>;

type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE" | "HEAD";

type ResponseType = {
	[K in keyof Response]: Response[K] extends (
		...args: unknown[]
	) => Promise<unknown> ? K
		: never;
}[keyof Response];

type CustomString = string & { ___?: never };

export type RequestOptions = Omit<RequestInit, "method" | "headers" | "body"> & {
	responseType?: ResponseType;
	ignoreErrors?: boolean;
};

export const get: HttpMethod = "GET";
export const post: HttpMethod = "POST";
export const put: HttpMethod = "PUT";
export const patch: HttpMethod = "PATCH";
export const del: HttpMethod = "DELETE";
export const head: HttpMethod = "HEAD";

export type KnownHeaders =
	| "Accept"
	| "Accept-Charset"
	| "Accept-Encoding"
	| "Accept-Language"
	| "Authorization"
	| "Cache-Control"
	| "Content-Length"
	| "Content-Type"
	| "Cookie"
	| "ETag"
	| "Forwarded"
	| "If-Match"
	| "If-Modified-Since"
	| "If-None-Match"
	| "If-Unmodified-Since"
	| "Origin"
	| "Referer"
	| "User-Agent";

export const isoDateRegex = /^(\d{4}-\d{2}-\d{2})[T\s](\d{2}:\d{2}(:\d{2}(\.\d{1,6})?)?)((Z)|([+-]\d{2}:\d{2}))?$/;

export const DateSafeJsonParse = <T>(text: string): T => JSON.parse(text, (_, value) => {
	if (typeof value === 'string' && isoDateRegex.test(value)) {
		const date = new Date(value);
		if (!Number.isNaN(date.getTime())) return date;
	}
	return value as T;
})

export async function typedFetch<TOut, TBody>(
	url: string,
	method: HttpMethod | CustomString,
	body?: TBody,
	headers?: Record<KnownHeaders | CustomString, string>,
	options?: RequestOptions,
): Promise<TypedResponse<TOut, string>> {
	try {
		const res = await fetch(url, {
			method: method,
			headers: {
				"content-type": "application/json",
				...headers,
			},
			body: body && JSON.stringify(body),
			...options,
		});

		if (res.status >= 400 && !options?.ignoreErrors) {
			return {
				ok: false,
				status: res.status,
				statusText: res.statusText,
				headers: res.headers,
				error: await res.text().catch(() => res.statusText),
			};
		}
		const contentType = res.headers.get("Content-Type")?.toLowerCase() ?? "";

		let data: unknown;

		if (options?.responseType) {
			data = await res[options.responseType]();
		} else if (contentType.includes("application/json")) {
			const text = await res.text();
			data = DateSafeJsonParse(text);
		} else if (/^(application|image|audio|video)\//.test(contentType)) {
			data = await res.blob();
		} else {
			data = await res.text();
		}

		return {
			ok: true,
			status: res.status,
			statusText: res.statusText,
			headers: res.headers,
			data: data as TOut,
		};
	} catch (e) {
		return {
			ok: false,
			status: 0,
			statusText: "",
			headers: new Headers(),
			error: e instanceof Error ? e.message : String(e),
		};
	}
}
