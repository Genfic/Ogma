(() => {
	const route = document.querySelector('[data-reads]') as HTMLElement;
	const story = document.querySelector('[data-story-id]') as HTMLElement;
	const buttons = [...document.querySelectorAll('button.read-status')] as HTMLButtonElement[];
	const csrf = (document.querySelector('input[name=__RequestVerificationToken]') as HTMLInputElement).value;
    
	route.remove();
	story.remove();
    
	let reads: Array<number> = [];
    
	_getStatus();
    
	for (const b of buttons) {
		b.addEventListener('click', () => _markRead(Number(b.dataset.id)));
	}
    
	function _markRead(id: number) {
		fetch(route.dataset.reads, {
			method: 'post',
			headers: {
				"RequestVerificationToken" : csrf,
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				story: Number(story.dataset.storyId),
				chapter: id
			})
		})
			.then(res => res.json())
			.then(data => {
				reads = data.read;
				_update();
			})
			.catch(console.error);
	}
    
	function _update() {
		for (const btn of buttons) {
			const read = reads?.includes(Number(btn.dataset.id)) ?? false;

			btn.classList.toggle('active', read);
			btn.querySelector('i').innerText = read ? 'visibility' : 'visibility_off';
		}
	}
    
	function _getStatus() {
		fetch(`${route.dataset.reads}/${story.dataset.storyId}`)
			.then(res => {
				if (res.status == 204) return [];
				else return res.json();
			})
			.then(data => {
				reads = data.read;
				_update();
			})
			.catch(console.error);
	}
    
})();