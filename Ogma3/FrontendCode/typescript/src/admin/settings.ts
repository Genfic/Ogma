import { log } from "@h/logger";
import { $queryAll } from "@h/dom";

const inputs = $queryAll<HTMLInputElement>("input.o-form-control");

for (const i of inputs) {
	i.dataset.init = i.value;

	i.addEventListener("input", (e: Event) => {
		const element = e.target as HTMLInputElement;

		log.log(element.value !== element.dataset.init);

		if (element.value !== element.dataset.init) {
			element.classList.add("changed");
		} else {
			element.classList.remove("changed");
		}
	});
}
