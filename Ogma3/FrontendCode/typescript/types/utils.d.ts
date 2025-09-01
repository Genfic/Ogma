export type Empty = Record<string, never>;

export type ProblemDetails = {
	title: string;
	status: number;
	errors: Record<string, string[]>;
};
