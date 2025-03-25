export function getData<T extends string>(selector: string, keys: T, root?: HTMLElement | undefined): string;

export function getData<T extends string>(
	selector: string,
	keys: T[],
	root?: HTMLElement | undefined,
): Record<T, string>;

export function getData<T extends string>(
	selector: string,
	keys: T | readonly T[],
	root?: HTMLElement | undefined,
): string | Record<T, string> {
	const { dataset } = (root ?? document).querySelector(selector) as HTMLElement;

	if (typeof keys === "string") {
		return dataset[keys] as string;
	}

	return Object.fromEntries(keys.map((k) => [k, dataset[k]])) as Record<T, string>;
}
