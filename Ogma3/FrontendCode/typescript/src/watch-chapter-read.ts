import { PostApiChaptersread } from "../generated/paths-public";

const progress = document.getElementById("chapter-progress");
progress.addEventListener("read", async ({ currentTarget: t }) => {
	const chapterId = Number.parseInt(t.dataset.chapter);
	const storyId = Number.parseInt(t.dataset.story);
	const csrf = t.dataset.csrf;

	const res = await PostApiChaptersread({ story: storyId, chapter: chapterId }, { RequestVerificationToken: csrf });

	if (!res.ok && res.status !== 401) {
		alert(`Could not mark chapter as read: ${res.status}`);
	}
});
