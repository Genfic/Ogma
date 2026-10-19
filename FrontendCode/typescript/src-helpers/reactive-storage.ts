import { getCookieValue, setCookie } from "@h/cookies";
import { createSignal, createEffect } from "solid-js";

type Storage = <T>() => {
	get: (key: string) => T | undefined;
	set: (key: string, value: T) => void;
};

export const useStorage = <T>(name: string, defaultValue: T, storage: Storage = localStorage) => {
	const store = storage();
	const [item, setItem] = createSignal(store.get(name) ?? defaultValue);

	createEffect(() => {
		const val = item();
		store.set(name, val);
	});

	return [item, setItem];
};

export const cookieStorage: Storage = <T>() => {
	return {
		get: (key) => {
			const val = getCookieValue(key);
			if (!val) {
				return undefined;
			}
			return JSON.parse(val) as T;
		},
		set: (key, value) => setCookie(key, JSON.stringify(value)),
	};
};

export const localStorage: Storage = <T>() => {
	const store = window.localStorage;

	return {
		get: (key) => {
			const val = store.getItem(key);
			if (!val) {
				return undefined;
			}
			return JSON.parse(val) as T;
		},
		set: (key, value) => store.setItem(key, JSON.stringify(value)),
	};
};

export const sessionStorage: Storage = <T>() => {
	const store = window.sessionStorage;

	return {
		get: (key) => {
			const val = store.getItem(key);
			if (!val) {
				return undefined;
			}
			return JSON.parse(val) as T;
		},
		set: (key, value) => store.setItem(key, JSON.stringify(value)),
	};
};
