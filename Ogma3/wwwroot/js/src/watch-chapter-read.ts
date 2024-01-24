import { ChaptersRead_PostChaptersRead as markChapter } from "../generated/paths-public";

const progress = document.getElementById("chapter-progress");
progress.addEventListener("read", async () => {
	const chapterId = parseInt(progress.dataset.chapter);
	const storyId = parseInt(progress.dataset.story);
	const csrf = progress.dataset.csrf; 

	const res = await markChapter({ story: storyId, chapter: chapterId }, { RequestVerificationToken: csrf });
	
	if (!res.ok && res.status != 401) {
		alert(`Could not mark chapter as read: ${res.status}`);
	}
});
