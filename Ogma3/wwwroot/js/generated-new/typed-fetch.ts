export class TypedResponse<TData> {
	private readonly res: Response;
	
	public readonly ok: boolean;
	public readonly statusText: string;
	public readonly status: number;
	public readonly headers: Headers;
	constructor(res: Response) {
		this.res = res;
		this.ok = res.ok;
		this.statusText = res.statusText;
		this.status = res.status;
		this.headers = res.headers;
	}
	json() {
		return this.res.json() as Promise<TData>;
	}
	text() {
		return this.res.text();
	}
	clone() {
		return this.res.clone();
	}	
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
		
	return new TypedResponse<TOut>(res);
}