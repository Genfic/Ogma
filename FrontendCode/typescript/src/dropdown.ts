import { onClickOutside } from "@h/click-outside";
import { $queryAll } from "@h/dom";

const dropdowns = $queryAll("[o-dropdown]");
for (const dropdown of dropdowns) {
	const btn = dropdown.querySelector("[o-dd-btn]");
	const body = dropdown.querySelector("[o-dd-body]");

	if (!btn || !body) {
		console.error("No dropdown button or body in dropdown");
		continue;
	}

	btn.addEventListener("click", () => {
		body.classList.toggle("open");
		btn.classList.toggle("active");
	});

	onClickOutside(dropdown, () => {
		body.classList.remove("open");
		btn.classList.remove("active");
	});
}
