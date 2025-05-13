import { ico } from "@h/icon-path" with { type: "macro" };
import { $queryAll } from "@h/dom";

const passwordInputs = $queryAll<HTMLInputElement>("input[type=password]");

for (const pi of passwordInputs) {
	const buttons = pi.nextElementSibling;

	buttons?.querySelector(".show-password")?.addEventListener("click", (e: Event) => {
		e.preventDefault();

		const icon = (e.currentTarget as HTMLElement)?.querySelector("use");
		if (!icon) return;

		if (pi.type === "password") {
			pi.type = "text";
			icon.setAttribute("href", ico("lucide:eye"));
		} else {
			pi.type = "password";
			icon.setAttribute("href", ico("lucide:eye-closed"));
		}
	});
}
