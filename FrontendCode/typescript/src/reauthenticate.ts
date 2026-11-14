import { $id } from "@h/dom";

const button = $id<HTMLButtonElement>("reauth-passkey-button", true);
const form = $id<HTMLFormElement>("reauth-passkey-form", true);
const credentialsInput = $id<HTMLInputElement>("reauth-credentials", true);

// The passkey method is only rendered when the account can use it
if (button && form && credentialsInput) {
	const transaction = form.querySelector<HTMLInputElement>("input[name=transaction]")?.value ?? "";

	button.addEventListener("click", async () => {
		button.classList.add("loading");
		try {
			const optionsRes = await fetch(
				`/reauthenticate?handler=PasskeyOptions&transaction=${encodeURIComponent(transaction)}`,
			);
			if (!optionsRes.ok) {
				throw new Error(`Failed to load passkey options (${optionsRes.status})`);
			}

			const options = PublicKeyCredential.parseRequestOptionsFromJSON(await optionsRes.json());

			const assertion = (await navigator.credentials.get({ publicKey: options })) as PublicKeyCredential;

			if (!assertion) {
				throw new Error("No assertion received");
			}

			credentialsInput.value = JSON.stringify(assertion.toJSON());
			form.submit();
		} catch (error) {
			console.error("Passkey verification failed", error);
			button.classList.remove("loading");
			const alert = document.createElement("div");
			alert.className = "alert alert danger";
			alert.setAttribute("role", "alert");
			alert.textContent = "Verification failed. Please try again.";
			form.before(alert);
		}
	});
}
