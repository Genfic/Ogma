import { ico } from "@h/icon-path" with { type: "macro" };

const passwordInputs = [...document.querySelectorAll("input[type=password]")] as HTMLInputElement[];

for (const pi of passwordInputs) {
	const buttons = pi.nextElementSibling;

	buttons?.querySelector(".show-password").addEventListener("click", (e: Event) => {
		e.preventDefault();
		if (pi.type === "password") {
			pi.type = "text";
			(e.currentTarget as HTMLElement).querySelector("use").setAttribute("href", ico("lucide:eye"));
		} else {
			pi.type = "password";
			(e.currentTarget as HTMLElement).querySelector("use").setAttribute("href", ico("lucide:eye-closed"));
		}
	});
}
