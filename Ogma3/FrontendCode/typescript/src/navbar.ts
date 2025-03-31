let last_known_scroll_position = 0;
let ticking = false;

const nav = document.getElementById("top-nav");
const btn = document.getElementById("burger") as (HTMLButtonElement | null);

let lastPos = 0;

function changeNav(pos: number) {
	nav.classList.toggle("compact", pos - lastPos > 0);
	lastPos = pos;
}

window.addEventListener("scroll", () => {
	last_known_scroll_position = window.scrollY;

	if (!ticking) {
		window.requestAnimationFrame(() => {
			changeNav(last_known_scroll_position);
			ticking = false;
		});
		ticking = true;
	}
});

btn?.addEventListener("click", () => {
	nav.classList.toggle("visible");
});
