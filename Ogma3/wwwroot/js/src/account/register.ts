const classname = "visible";
const formInputs = [...document.querySelectorAll("input[id]")] as HTMLInputElement[];

for (const input of formInputs) {
	const info = document.querySelector(`[data-for="${input.id}"]`);
	if (!info) continue;

	input.addEventListener("focusin", () => {
		info.classList.add(classname);
	});
	input.addEventListener("focusout", () => {
		info.classList.remove(classname);
	});
}
