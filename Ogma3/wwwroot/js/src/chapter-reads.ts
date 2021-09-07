import { ChaptersReadClient, MarkChapterAsReadCommand, MarkChapterAsUnreadCommand } from "../../lib/openapi/OpenAPI";

(async () => {
	console.log('runs');
	const story = document.querySelector("[data-story-id]") as HTMLElement;
	const buttons = [...document.querySelectorAll("button.read-status")] as HTMLButtonElement[];
	const csrf = (document.querySelector("input[name=__RequestVerificationToken]") as HTMLInputElement).value;

	const client = new ChaptersReadClient();
	const storyId: number = Number(story.dataset.storyId);

	story.remove();
	
	const headers = {
		"RequestVerificationToken": csrf,
		"Content-Type": "application/json"
	};

	let reads: number[] = [];
	
	await _getStatus();

	async function _readOrUnread(id: number) {
		console.log("clicks");
		return reads.includes(id) ? await _markUnread(id) : await _markRead(id);
	}

	async function _markRead(id: number) {
		console.log('marks');
		const data = await client.postChaptersRead(new MarkChapterAsReadCommand({
			story: storyId,
			chapter: id
		}));
		reads = data.read;
		_update();
	}

	async function _markUnread(id: number) {
		console.log('unmarks');
		const data = await client.deleteChaptersRead(new MarkChapterAsUnreadCommand({
			story: storyId,
			chapter: id
		}));
		reads = data.read;
		_update();
	}

	function _update() {
		for (const btn of buttons) {
			const read = reads?.includes(Number(btn.dataset.id)) ?? false;

			btn.classList.toggle("active", read);
			btn.querySelector("i").innerText = read ? "visibility" : "visibility_off";
		}
	}

	async function _getStatus() {
		reads = await client.getChaptersRead(storyId);
	}

	for (let b of buttons) {
		console.log(b.dataset.id);
		b.addEventListener("click", async () => {
			console.log('listener clicks');
			return await _readOrUnread(Number(b.dataset.id));
		});
	}
	
	buttons[0].addEventListener('mouseover', () => console.log('hello'));

})();