import type {
	CreateInfractionCommand,
	GetUserInfractionsResult,
	InfractionDto,
} from './types-internal';
import { typedFetch, get, post, put, patch, del, head } from './typed-fetch';


export const DeleteAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<string, undefined>("/admin/api/cache",
	del,
	undefined,
	headers,
	options,
);


export const DeleteAdminApiInfractions = async (infractionId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void, undefined>(`/admin/api/infractions/${infractionId}`,
	del,
	undefined,
	headers,
	options,
);


export const GetAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number|string, undefined>("/admin/api/cache",
	get,
	undefined,
	headers,
	options,
);


export const GetAdminApiInfractionsUser = async (userId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetUserInfractionsResult[], undefined>(`/admin/api/infractions/user/${userId}`,
	get,
	undefined,
	headers,
	options,
);


export const GetAdminApiTelemetryGetImportantItemCounts = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<object, undefined>("/admin/api/telemetry/GetImportantItemCounts",
	get,
	undefined,
	headers,
	options,
);


export const GetAdminApiTelemetryGetTableInfo = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<object, undefined>("/admin/api/telemetry/GetTableInfo",
	get,
	undefined,
	headers,
	options,
);


export const GetInfractionDetails = async (infractionId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InfractionDto, undefined>(`/admin/api/infractions/${infractionId}`,
	get,
	undefined,
	headers,
	options,
);


export const PostAdminApiInfractions = async (body: CreateInfractionCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InfractionDto, CreateInfractionCommand>("/admin/api/infractions",
	post,
	body,
	headers,
	options,
);

