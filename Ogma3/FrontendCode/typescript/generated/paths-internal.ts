import type {
	CreateInfractionCommand,
	GetUserInfractionsResult,
	InfractionDto,
} from './types-internal';
import { typedFetch } from './typed-fetch';


export const DeleteAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>("/admin/api/cache",
	"DELETE",
	undefined,
	headers,
	options,
);


export const DeleteAdminApiInfractions = async (infractionId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<void>(`/admin/api/infractions/${infractionId}`,
	"DELETE",
	undefined,
	headers,
	options,
);


export const GetAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number|string>("/admin/api/cache",
	"GET",
	undefined,
	headers,
	options,
);


export const GetAdminApiInfractionsUser = async (userId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetUserInfractionsResult[]>(`/admin/api/infractions/user/${userId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const GetAdminApiTelemetryGetImportantItemCounts = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<object>("/admin/api/telemetry/GetImportantItemCounts",
	"GET",
	undefined,
	headers,
	options,
);


export const GetAdminApiTelemetryGetTableInfo = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<object>("/admin/api/telemetry/GetTableInfo",
	"GET",
	undefined,
	headers,
	options,
);


export const GetInfractionDetails = async (infractionId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InfractionDto>(`/admin/api/infractions/${infractionId}`,
	"GET",
	undefined,
	headers,
	options,
);


export const PostAdminApiInfractions = async (body: CreateInfractionCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<InfractionDto>("/admin/api/infractions",
	"POST",
	body,
	headers,
	options,
);

