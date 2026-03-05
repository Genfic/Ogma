import type { BaseIssue, BaseSchema, InferOutput } from "valibot";
import { safeParse } from "valibot";

export type FormDataResult<T, E = Error> = [E, null] | [null, T];

export const getFormData = <TSchema extends BaseSchema<unknown, unknown, BaseIssue<unknown>>>(
	event: SubmitEvent,
	schema: TSchema,
): FormDataResult<InferOutput<TSchema>> => {
	if (!(event.currentTarget instanceof HTMLFormElement)) {
		return [new Error("Invalid form element"), null];
	}

	const raw = new FormData(event.currentTarget);

	const { success, output, issues } = safeParse(schema, Object.fromEntries(raw.entries()));

	if (success) {
		return [null, output];
	}

	return [new Error(issues?.map((i) => i.message).join(", ")), null];
};
