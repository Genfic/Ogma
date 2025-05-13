import { getCookieValue } from "@h/cookies";
import { $query, $queryAll } from "@h/dom";

const classname = "visible";
const formInputs = $queryAll("input[id]");

for (const input of formInputs) {
	const info = $query(`[data-for="${input.id}"]`, true);
	if (!info) continue;

	input.addEventListener("focusin", () => {
		info.classList.add(classname);
	});
	input.addEventListener("focusout", () => {
		info.classList.remove(classname);
	});
}

const turnstile = $query(".cf-turnstile");
const theme = getCookieValue("theme");
turnstile.dataset.theme = theme;
