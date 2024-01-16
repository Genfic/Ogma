import { log } from "../../src-helpers/logger";

(() => {
	const inputs = document.querySelectorAll("input.o-form-control");

	for (const i of inputs) {
		i.dataset.init = i.value;

		i.addEventListener("input", (e) => {
			log.log(e.target.value !== e.target.dataset.init);

			if (e.target.value !== e.target.dataset.init) {
				e.target.classList.add("changed");
			} else {
				e.target.classList.remove("changed");
			}
		});
	}
})();
