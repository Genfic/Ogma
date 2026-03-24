import { attemptOr } from "@h/error-helpers";

export namespace CSS {
	const canvas = document.createElement("canvas");
	canvas.width = canvas.height = 1;
	const ctx = canvas.getContext("2d");

	export const serialize = (object: Record<string, string>) =>
		Object.entries(object)
			.map(([key, value]) => `${key}:${encodeURIComponent(value)}`)
			.join(";");

	export const deserialize = <TKey extends string>(str: string) =>
		Object.fromEntries(
			(str ?? "")
				.split(";")
				.filter(Boolean)
				.map((pair) => {
					const [key, ...value] = pair.split(":");

					const raw = value.join(":").trim();

					const decoded = attemptOr(() => decodeURIComponent(raw), raw);
					return [key.trim(), decoded];
				}),
		) as Record<TKey, string>;

	export const toHex = (color: string) => {
		if (!ctx) {
			return color;
		}
		ctx.fillStyle = color;
		return ctx?.fillStyle;
	};
}
