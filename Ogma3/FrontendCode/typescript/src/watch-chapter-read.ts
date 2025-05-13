import { PostApiChaptersread } from "@g/paths-public";
import { $id } from "@h/dom";

const progress = $id("chapter-progress");
progress.addEventListener("read", async ({ currentTarget: t }: CustomEvent) => {
	if (!t || !(t instanceof HTMLElement)) return;

	const chapterId = Number.parseInt(t.dataset.chapter ?? "");
	const storyId = Number.parseInt(t.dataset.story ?? "");

	if (Number.isNaN(chapterId) || Number.isNaN(storyId)) {
		throw new Error(`Incorrect IDs: ${chapterId}, ${storyId}`);
	}

	const csrf = t.dataset.csrf;

	if (!csrf) {
		throw new Error("Missing CSRF token");
	}

	const res = await PostApiChaptersread({ story: storyId, chapter: chapterId }, { RequestVerificationToken: csrf });

	if (!res.ok && res.status !== 401) {
		alert(`Could not mark chapter as read: ${res.status}`);
	}
});
