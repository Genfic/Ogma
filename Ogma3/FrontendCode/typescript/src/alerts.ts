import { $queryAll } from "@h/dom";

const alerts = $queryAll(".alert-dismissible");

for (const a of alerts) {
	a.querySelector("button.close")?.addEventListener("click", () => {
		a.remove();
	});
}
