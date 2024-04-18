export const Telemetry_GetTableInfo = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/admin/api/telemetry/gettableinfo", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Telemetry_GetImportantItemCounts = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/admin/api/telemetry/getimportantitemcounts", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Cache_GetCache = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/admin/api/cache", { 
    method: 'GET', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });

export const Cache_DeleteCache = async (headers?: HeadersInit, options?: RequestInit) => await fetch ("/admin/api/cache", { 
    method: 'DELETE', 
    headers: { 
      'Content-Type': 'application/json', 
      ...headers 
    },
    ...options 
  });