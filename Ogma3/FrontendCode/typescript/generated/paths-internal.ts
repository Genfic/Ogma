import { del, get, post, typedFetch } from "./typed-fetch";
import type { CreateInfractionCommand, GetUserDataUserDetailsDto, GetUserInfractionsResult, InfractionDto } from "./types-internal";


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


export const GetAdminApiTelemetryGetImportantItemCounts = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>("/admin/api/telemetry/GetImportantItemCounts",
	get,
	undefined,
	headers,
	options,
);


export const GetAdminApiTelemetryGetTableInfo = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number, undefined>("/admin/api/telemetry/GetTableInfo",
	get,
	undefined,
	headers,
	options,
);


export const GetAdminApiUsers = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetUserDataUserDetailsDto, undefined>(`/admin/api/users/${name}`,
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

