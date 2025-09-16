import { GetApiChaptersRead } from "@g/paths-public";
import { createResource, type InitializedResourceReturn } from "solid-js";

let resource: InitializedResourceReturn<Set<number>, number>;

export const useChaptersRead = (storyId: number) => {
	if (resource) {
		return resource;
	}
	resource = createResource(
		() => storyId,
		async (id: number) => {
			const res = await GetApiChaptersRead(id);
			if (!res.ok) {
				return new Set<number>();
			}
			return new Set(res.data);
		},
		{ initialValue: new Set<number>() },
	);
	return resource;
};
