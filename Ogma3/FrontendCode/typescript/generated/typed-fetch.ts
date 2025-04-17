interface TypedResponse<T> {
	readonly ok: boolean;
	readonly status: number;
	readonly statusText: string;
	readonly headers: Headers;
	readonly data: T;
}

type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE" | "HEAD";

export const get: HttpMethod = "GET";
export const post: HttpMethod = "POST";
export const put: HttpMethod = "PUT";
export const patch: HttpMethod = "PATCH";
export const del: HttpMethod = "DELETE";
export const head: HttpMethod = "HEAD";

export async function typedFetch<TOut, TBody>(
	url: string,
	method: HttpMethod | (string & { ___?: never }),
	body?: TBody,
	headers?: HeadersInit,
	options?: RequestInit,
): Promise<Readonly<TypedResponse<TOut>>> {
	const res = await fetch(url, {
		method: method,
		headers: {
			"content-type": "application/json",
			...headers,
		},
		body: body && JSON.stringify(body),
		...options,
	});

	const contentType = res.headers.get("content-type");

	const data: TOut = contentType?.includes("application/json")
		? await res.json()
		: await res.text();

	return {
		ok: res.ok,
		status: res.status,
		statusText: res.statusText,
		headers: res.headers,
		data: data,
	};
}
