import { createSignal } from "solid-js";

export type LocalStorageHook<T> = [getter: () => T | undefined, setter: (value: T) => void, remover: () => void];

export function useLocalStorage<T>(key: string, initialValue?: T): LocalStorageHook<T> {
	const readValueFromStorage = (): T | undefined => {
		try {
			const item = window.localStorage.getItem(key);
			return item ? JSON.parse(item) : initialValue;
		} catch (error) {
			console.error(`Error reading localStorage key "${key}":`, error);
			return initialValue;
		}
	};

	const [storedValue, setStoredValue] = createSignal<T | undefined>(readValueFromStorage());

	const setValue = (value: T) => {
		try {
			setStoredValue(() => value);

			window.localStorage.setItem(key, JSON.stringify(value));
		} catch (error) {
			console.error(`Error setting localStorage key "${key}":`, error);
		}
	};

	// The remove function that will be returned
	const removeValue = () => {
		try {
			setStoredValue(() => undefined);

			window.localStorage.removeItem(key);
		} catch (error) {
			console.error(`Error removing localStorage key "${key}":`, error);
		}
	};

	// Listen for changes in other tabs/windows
	window.addEventListener("storage", (e) => {
		if (e.key === key) {
			const newValue = e.newValue ? JSON.parse(e.newValue) : undefined;
			setStoredValue(() => newValue);
		}
	});

	return [() => storedValue(), setValue, removeValue];
}
