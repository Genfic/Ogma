import { GetApiChaptersread } from "@g/paths-public";
import { createResource } from "solid-js";

export const useChaptersRead = (storyId: number) =>
	createResource(
		() => storyId,
		async (id: number) => {
			const res = await GetApiChaptersread(id);
			if (!res.ok) {
				return [];
			}
			return res.data;
		},
	);
