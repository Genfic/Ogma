import { attemptOr } from "@h/error-helpers";

export namespace CSS {
	const canvas = document.createElement("canvas");
	canvas.width = canvas.height = 1;
	const ctx = canvas.getContext("2d");

	export const serialize = (object: Record<string, string>) =>
		Object.entries(object)
			.map(([key, value]) => `${key}:${encodeURIComponent(value)}`)
			.join(";");

	const isValidVar = <TKey extends string>(key: string, validVars: readonly TKey[]): key is TKey =>
		(validVars as readonly string[]).includes(key);

	export const deserialize = <TKey extends string>(
		str: string,
		allowedVars: readonly TKey[],
	): Partial<Record<TKey, string>> => {
		return str
			.split(";")
			.filter(Boolean)
			.reduce<Partial<Record<TKey, string>>>((acc, pair) => {
				const [key, ...value] = pair.split(":");

				const raw = value.join(":").trim();

				const decoded = attemptOr(() => decodeURIComponent(raw), raw);

				const k = key.trim();
				if (isValidVar(k, allowedVars)) {
					acc[k] = decoded;
				}

				return acc;
			}, {});
	};

	export const deserializer =
		<TKey extends string>(validVars: readonly TKey[]) =>
		(str: string) =>
			deserialize(str, validVars);

	export const toHex = (color: string | undefined) => {
		if (!ctx) {
			return color;
		}
		if (!color) {
			return undefined;
		}
		ctx.fillStyle = color;
		return ctx?.fillStyle;
	};
}
