export namespace Parallel {
	export function forEach<TData, TOut>(entries: TData[], func: (data: TData) => Promise<TOut>): Promise<TOut[]> {
		return Promise.all(entries.map(func));
	}
}
