import { PostApiChaptersread } from "../generated/paths-public";

const progress = document.getElementById("chapter-progress");
progress.addEventListener("read", async () => {
	const chapterId = Number.parseInt(progress.dataset.chapter);
	const storyId = Number.parseInt(progress.dataset.story);
	const csrf = progress.dataset.csrf;

	const res = await PostApiChaptersread({ story: storyId, chapter: chapterId }, { RequestVerificationToken: csrf });

	if (!res.ok && res.status !== 401) {
		alert(`Could not mark chapter as read: ${res.status}`);
	}
});
