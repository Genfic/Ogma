import { $queryAll } from "@h/dom";

const inputs = $queryAll<HTMLInputElement>("input.o-form-control");

const initValues = new Map<string, string>();

for (const i of inputs) {
	initValues.set(i.name, i.value);

	i.addEventListener("input", (e: Event) => {
		const element = e.target as HTMLInputElement;

		if (element.value !== initValues.get(element.name)) {
			element.classList.add("changed");
		} else {
			element.classList.remove("changed");
		}
	});
}
