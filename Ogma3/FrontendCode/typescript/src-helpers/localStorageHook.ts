type LocalStorageHook<TData> = [get: () => TData | null, set: (data: TData) => void, remove: () => void];

export const useLocalStorage = <TData>(key: string): LocalStorageHook<TData> => [
	() => JSON.parse(localStorage.getItem(key)) as TData | null,
	(data: TData) => localStorage.setItem(key, JSON.stringify(data)),
	() => localStorage.removeItem(key),
];
