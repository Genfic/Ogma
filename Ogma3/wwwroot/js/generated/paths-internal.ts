import type { CreateInfractionCommand, GetUserInfractionsResult, InfractionDto } from "./types-internal";
import { typedFetch } from "./typed-fetch";

export const DeleteAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<void>("/admin/api/cache", "DELETE", undefined, headers, options);

export const DeleteAdminApiInfractions = async (infractionid: number, headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<void>(`/admin/api/infractions/${infractionid}`, "DELETE", undefined, headers, options);

export const GetAdminApiCache = async (headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<number>("/admin/api/cache", "GET", undefined, headers, options);

export const GetAdminApiInfractionsUser = async (userid: number, headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<GetUserInfractionsResult[]>(`/admin/api/infractions/user/${userid}`, "GET", undefined, headers, options);

export const GetAdminApiTelemetryGetImportantItemCounts = async (headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<object>("/admin/api/telemetry/getimportantitemcounts", "GET", undefined, headers, options);

export const GetAdminApiTelemetryGetTableInfo = async (headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<object>("/admin/api/telemetry/gettableinfo", "GET", undefined, headers, options);

export const GetInfractionDetails = async (infractionid: number, headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<InfractionDto>(`/admin/api/infractions/${infractionid}`, "GET", undefined, headers, options);

export const PostAdminApiInfractions = async (body: CreateInfractionCommand, headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<InfractionDto>("/admin/api/infractions", "POST", body, headers, options);
