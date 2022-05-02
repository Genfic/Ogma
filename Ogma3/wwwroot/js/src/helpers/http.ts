import { Result } from "./result";

export class http {
	static get = <T>(url: string, headers: object = {}, config: object = {}) =>
		http.request<T>(url, "GET", null, headers, config);

	static request = async <TResponse>(
		url: string,
		method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE" | "HEAD",
		payload: object | null = null,
		headers: object = {},
		config: object = {}
	): Promise<Result<TResponse>> => {
		try {
			const response = await fetch(url, {
				method: method,
				body: payload === null ? null : JSON.stringify(payload),
				headers: {
					"Content-Type": "application/json",
					...headers,
				},
				...config,
			});

			if (response.ok) {
				const data: TResponse = await response.json();
				return Result.ok<TResponse>(data);
			} else {
				return Result.fail<TResponse>(response.statusText);
			}
		} catch (e) {
			const msg = (e as TypeError).message;
			return Result.fail<TResponse>(msg);
		}
	};
}