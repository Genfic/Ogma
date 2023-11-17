import { Result } from "./result";

export const get = <T>(url: string, headers: object = {}, config: object = {}) => request<T>(url, "GET", null, headers, config);

export const post = <T>(url: string, payload: object, headers: object = {}, config: object = {}) =>
	request<T>(url, "POST", payload, headers, config);

export const httpDelete = <T>(url: string, payload: object, headers: object = {}, config: object = {}) =>
	request<T>(url, "DELETE", payload, headers, config);

export const request = async <TResponse>(
	url: string,
	method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE" | "HEAD",
	payload: object | null = null,
	headers: object = {},
	config: object = {},
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
