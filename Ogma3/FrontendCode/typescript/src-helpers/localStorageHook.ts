type LocalStorageHook<TData> = [get: () => TData | null, set: (data: TData) => void, remove: () => void];

export const useLocalStorage = <TData>(key: string, raw = false): LocalStorageHook<TData> => [
	() => {
		const item = localStorage.getItem(key);
		if (raw) {
			return item as TData;
		}
		return item ? (JSON.parse(item) as TData) : null;
	},
	(data: TData) => localStorage.setItem(key, JSON.stringify(data)),
	() => localStorage.removeItem(key),
];
