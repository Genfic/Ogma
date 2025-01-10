interface TypedResponse<T> {
    ok: boolean; 
    status: number; 
    statusText: string; 
    headers: Headers; 
    data: T;
}

export async function typedFetch<TOut>(
	url: string,
	method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE" | "HEAD" | string,
	body?: object,
	headers?: HeadersInit,
	options?: RequestInit,
): Promise<TypedResponse<TOut>> {
	const res = await fetch(url, {
		method: method,
		headers: {
			"Content-Type": "application/json",
			...headers,
		},
		body: body ? JSON.stringify(body) : null,
		...options,
	});
		
	return {
        ok: res.ok,
        status: res.status,
        statusText: res.statusText,
        headers: res.headers,
        data: await res.json() as TOut
    }
}