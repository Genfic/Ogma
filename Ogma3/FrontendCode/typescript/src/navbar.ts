import { $id } from "@h/dom";

const nav = $id("top-nav");
const btn = $id<HTMLButtonElement>("burger");

let lastScrollY = window.scrollY;
let ticking = false;
const scrollThreshold = 50;

window.addEventListener("scroll", () => {
	const currentScrollY = window.scrollY;

	if (!ticking) {
		window.requestAnimationFrame(() => {
			const delta = currentScrollY - lastScrollY;
			if (Math.abs(delta) > scrollThreshold) {
				nav.classList.toggle("compact", delta > 0);
				lastScrollY = currentScrollY;
			}
			ticking = false;
		});
		ticking = true;
	}
});

btn.addEventListener("click", () => {
	nav.classList.toggle("visible");
});
