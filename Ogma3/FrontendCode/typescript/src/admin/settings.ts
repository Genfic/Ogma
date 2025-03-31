import { log } from "../../src-helpers/logger";

const inputs = [...document.querySelectorAll("input.o-form-control")] as HTMLInputElement[];

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
