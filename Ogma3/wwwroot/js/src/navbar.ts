(() => {
	let last_known_scroll_position = 0;
	let ticking = false;
	const nav: HTMLElement = document.getElementById('top-nav');

	let lastPos = 0;
	function changeNav(pos) {
		nav.classList.toggle('compact', pos - lastPos > 0);
		lastPos = pos;
	}

	window.addEventListener('scroll', () => {
		last_known_scroll_position = window.scrollY;

		if (!ticking) {
			window.requestAnimationFrame(() => {
				changeNav(last_known_scroll_position);
				ticking = false;
			});
			ticking = true;
		}
	});
})();