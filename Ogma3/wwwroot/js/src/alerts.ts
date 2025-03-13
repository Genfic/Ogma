const alerts = [...document.getElementsByClassName("alert-dismissible")] as HTMLElement[];

for (const a of alerts) {
	a.querySelector("button.close").addEventListener("click", () => {
		a.remove();
	});
}
