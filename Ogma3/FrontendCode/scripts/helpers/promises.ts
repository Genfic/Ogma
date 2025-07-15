export namespace Parallel {
	export function forEach<TData, TOut>(entries: TData[], func: (data: TData) => Promise<TOut>): Promise<TOut[]>;
	export function forEach<TData, TOut>(
		entries: AsyncIterableIterator<TData>,
		func: (data: TData) => Promise<TOut>,
	): Promise<TOut[]>;

	export async function forEach<TData, TOut>(
		entries: TData[] | AsyncIterableIterator<TData>,
		func: (data: TData) => Promise<TOut>,
	): Promise<TOut[]> {
		const tasks: Promise<TOut>[] = [];

		if (Array.isArray(entries)) {
			for (const ent of entries) {
				tasks.push(func(ent));
			}
		} else {
			for await (const ent of entries) {
				tasks.push(func(ent));
			}
		}

		return await Promise.all(tasks);
	}
}
