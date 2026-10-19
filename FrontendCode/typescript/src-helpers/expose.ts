import { createEffect } from "solid-js";

export const expose = (obj: Record<string, () => unknown>) => {
	if (!import.meta.env.DEV) {
		return;
	}

	createEffect(() => {
		const snapshot: Record<string, unknown> = {};
		for (const key in obj) {
			snapshot[key] = obj[key]();
		}
		console.log(snapshot);
	});
};
