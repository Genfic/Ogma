(async () => {
	const route = document.querySelector("[data-reads]") as HTMLElement;
	const story = document.querySelector("[data-story-id]") as HTMLElement;

	const buttons = [
		...document.querySelectorAll("button.read-status"),
	] as HTMLButtonElement[];
	const csrf = (
		document.querySelector(
			"input[name=__RequestVerificationToken]"
		) as HTMLInputElement
	).value;

	route.remove();
	story.remove();

	let reads: Array<number> = [];

	await _getStatus();

	for (const b of buttons) {
		b.addEventListener("click", () => _changeState(Number(b.dataset.id)));
	}

	const _changeState = async (id: number) => {
		const method = reads.includes(id) ? "delete" : "post";
		const res = await fetch(route.dataset.reads, {
			method: method,
			headers: {
				RequestVerificationToken: csrf,
				"Content-Type": "application/json",
			},
			body: JSON.stringify({
				story: Number(story.dataset.storyId),
				chapter: id,
			})
		});
		reads = await res.json();
		_update();
	};

	function _update() {
		for (const btn of buttons) {
			const read = reads?.includes(Number(btn.dataset.id)) ?? false;

			btn.classList.toggle("active", read);
			btn.querySelector("i").innerText = read
				? "visibility"
				: "visibility_off";
		}
	}

	async function _getStatus() {
		const res = await fetch(
			`${route.dataset.reads}/${story.dataset.storyId}`
		);

		reads = await res.json();
		_update();
	}
})()