import type {
	Command23,
	InfractionDto,
	Result7,
} from './types-internal';
import { typedFetch } from './typed-fetch';


export const  = async (body: Command23, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InfractionDto>("/admin/api/infractions",
	"POST",
	body,
	headers,
	options,
);


export const  = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number|string>("/admin/api/cache",
	"GET",
	undefined,
	headers,
	options,
);


export const  = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<object>("/admin/api/telemetry/getimportantitemcounts",
	"GET",
	undefined,
	headers,
	options,
);


export const  = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<object>("/admin/api/telemetry/gettableinfo",
	"GET",
	undefined,
	headers,
	options,
);


export const  = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>("/admin/api/cache",
	"DELETE",
	undefined,
	headers,
	options,
);


export const  = async (infractionid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>(`/admin/api/infractions/${infractionid}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const  = async (userid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<Result7[]>(`/admin/api/infractions/user/${userid}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetInfractionDetails = async (infractionid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InfractionDto>(`/admin/api/infractions/${infractionid}`,
	"GET",
	undefined,
	headers,
	options,
);

