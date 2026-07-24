export type Empty = Record<string, never>;

export type ProblemDetails = {
	title: string;
	status: number;
	errors: Record<string, string[]>;
};

export type Prettify<T> = {
	[K in keyof T]: T[K];
} & {};

export type RequiredExcept<T, K extends keyof T> = Prettify<
	Pick<T, K> & {
		[P in Exclude<keyof T, K>]-?: NonNullable<T[P]>;
	}
>;
