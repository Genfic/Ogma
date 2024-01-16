function sendForm() {
	const inputs = [document.getElementById("query"), document.getElementById("sort")];
	for (const e of inputs) {
		if (e.value === null || e.value === "") e.disabled = true;
	}
}

document.getElementById("search").addEventListener("submit", sendForm);
