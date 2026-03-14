import { DELETE, GET, POST, typedFetch } from "./typed-fetch";
import type { CreateInfractionCommand, GetUserDataUserDetailsDto, GetUserInfractionsResult, InfractionDto } from "./types-internal";

const _enc = <T>(p: T): T extends string ? string : T => (typeof p === 'string' ? encodeURIComponent(p) : p) as any;


export const DeleteAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 401: undefined; 500: string }, undefined>("/admin/api/cache",
    DELETE,
    undefined,
    headers,
    options,
);

export const DeleteAdminApiInfractions = async (infractionId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 401: undefined; 404: undefined }, undefined>(`/admin/api/infractions/${infractionId}`,
    DELETE,
    undefined,
    headers,
    options,
);

export const GetAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 401: undefined; 500: string }, undefined>("/admin/api/cache",
    GET,
    undefined,
    headers,
    options,
);

export const GetAdminApiInfractionsUser = async (userId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetUserInfractionsResult[]; 401: undefined }, undefined>(`/admin/api/infractions/user/${userId}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetAdminApiTelemetryGetImportantItemCounts = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 401: undefined }, undefined>("/admin/api/telemetry/GetImportantItemCounts",
    GET,
    undefined,
    headers,
    options,
);

export const GetAdminApiTelemetryGetTableInfo = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: number; 401: undefined }, undefined>("/admin/api/telemetry/GetTableInfo",
    GET,
    undefined,
    headers,
    options,
);

export const GetAdminApiUsers = async (name: string, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: GetUserDataUserDetailsDto; 401: undefined; 404: undefined }, undefined>(`/admin/api/users/${name}`,
    GET,
    undefined,
    headers,
    options,
);

export const GetInfractionDetails = async (infractionId: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: InfractionDto; 401: undefined; 404: undefined }, undefined>(`/admin/api/infractions/${infractionId}`,
    GET,
    undefined,
    headers,
    options,
);

export const PostAdminApiInfractions = async (body: CreateInfractionCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<{ 200: undefined; 401: undefined }, CreateInfractionCommand>("/admin/api/infractions",
    POST,
    body,
    headers,
    options,
);
