import {
	GetApiChaptersread as getRead,
	PostApiChaptersread as markRead,
	DeleteApiChaptersread as markUnread,
} from "../generated/paths-public";

(async () => {
	const story = document.querySelector("[data-story-id]") as HTMLElement;
	const csrf = document.querySelector("[data-x-csrf]") as HTMLElement;

	const buttons = [...document.querySelectorAll("button.read-status")] as HTMLButtonElement[];

	story.remove();
	csrf.remove();

	let reads: Array<number> = [];

	await _getStatus();

	for (const b of buttons) {
		b.addEventListener("click", () => _changeState(Number.parseInt(b.dataset.id)));
	}

	const _changeState = async (id: number) => {
		const client = reads.includes(id) ? markUnread : markRead;
		const res = await client(
			{
				story: Number.parseInt(story.dataset.storyId),
				chapter: id,
			},
			{
				RequestVerificationToken: csrf.dataset["x-csrf"],
			},
		);

		if (!res.ok) return;

		reads = res.data;
		_update();
	};

	function _update() {
		for (const btn of buttons) {
			const read = reads?.includes(Number.parseInt(btn.dataset.id)) ?? false;

			btn.classList.toggle("active", read);
			btn.querySelector("i").innerText = read ? "visibility" : "visibility_off";
		}
	}

	async function _getStatus() {
		const res = await getRead(Number.parseInt(story.dataset.storyId));

		if (!res.ok) return;

		reads = res.data;
		_update();
	}
})();
