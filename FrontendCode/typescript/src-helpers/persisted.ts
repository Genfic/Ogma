import * as storage from "@solid-primitives/storage";
import type { PersistenceOptions } from "@solid-primitives/storage";
import type { Signal } from "solid-js";
import type { SetStoreFunction, Store } from "solid-js/store";

/** Mirrors the library's `PersistedState`, which it defines but never re-exports. */
export type PersistedState<S> = S & {
	2: Promise<string> | string | null;
};

/**
 * Wrapper around `makePersisted` that restores inference of the persisted value's type.
 *
 * Upstream binds `T` solely through the constraint `S extends Signal<T> | [Store<T>, SetStoreFunction<T>]`,
 * which leaves no inference site for `T`, so it collapses to `unknown`. A `Signal<boolean>` then fails the
 * invariant `Signal<unknown>` constraint, forcing every caller to spell out explicit generics. Binding `T`
 * straight to the parameter gives it an inference site, so the value type is inferred from the signal or store.
 */
export function makePersisted<T>(
	signal: Signal<T>,
	options?: PersistenceOptions<T, undefined>,
): PersistedState<Signal<T>>;
export function makePersisted<T, O extends Record<string, unknown>>(
	signal: Signal<T>,
	options: PersistenceOptions<T, O>,
): PersistedState<Signal<T>>;
export function makePersisted<T, O extends Record<string, unknown>>(
	store: [Store<T>, SetStoreFunction<T>],
	options: PersistenceOptions<T, O>,
): PersistedState<[Store<T>, SetStoreFunction<T>]>;
export function makePersisted<
	T,
	S extends Signal<T> | [Store<T>, SetStoreFunction<T>],
	O extends Record<string, unknown>,
>(state: S, options: PersistenceOptions<T, O>): PersistedState<S> {
	return storage.makePersisted<T, S, O>(state, options);
}
