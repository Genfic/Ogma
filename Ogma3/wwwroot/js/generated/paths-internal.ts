import type {
	CreateInfractionCommand,
	CreateInfractionResponse,
	DeactivateInfractionResponse,
	GetInfractionDetailsResult,
	GetTableInfoResponse,
	GetUserInfractionsResult,
	ProblemDetails,
} from "./types-internal";
import { typedFetch } from "./typed-fetch";

export const Telemetry_GetTableInfo = async (headers?: HeadersInit, options?: RequestInit) =>
	await typedFetch<GetTableInfoResponse[]>("/admin/api/telemetry/gettableinfo", "GET", undefined, headers, options);

export const Telemetry_GetImportantItemCounts = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<object>("/admin/api/telemetry/getimportantitemcounts", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Infractions_GetInfractions = async (userid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetUserInfractionsResult[]>(`/api/infractions?userid=${userid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Infractions_AddInfraction = async (body: CreateInfractionCommand, headers?: HeadersInit, options?: RequestInit) => await typedFetch<CreateInfractionResponse>("/api/infractions", 
    'POST', 
    body,
    headers,
    options,
  );

export const Infractions_GetInfractionDetails = async (infractionid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<GetInfractionDetailsResult>(`/api/infractions/details?infractionid=${infractionid}`, 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Infractions_DeactivateInfraction = async (infractionid: number, headers?: HeadersInit, options?: RequestInit) => await typedFetch<DeactivateInfractionResponse|ProblemDetails>(`/api/infractions/${infractionid}`, 
    'DELETE', 
    undefined,
    headers,
    options,
  );

export const Cache_GetCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<number>("/admin/api/cache", 
    'GET', 
    undefined,
    headers,
    options,
  );

export const Cache_DeleteCache = async (headers?: HeadersInit, options?: RequestInit) => await typedFetch<string>("/admin/api/cache", 
    'DELETE', 
    undefined,
    headers,
    options,
  );