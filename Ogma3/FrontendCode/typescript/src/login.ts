import { GetApiPasskeysRequestOptions, GetApiSignin as getSignInData, PostApiPasskeysSignin } from "@g/paths-public";
import { $id } from "@h/dom";

const avatar = $id<HTMLImageElement>("user-avatar");
const title = $id("user-title");
const name = $id<HTMLInputElement>("Input_Name");
const passkeyButton = $id<HTMLButtonElement>("passkey-login");

name.addEventListener("focusout", async (e) => {
	const target = e.target as HTMLInputElement;

	const res = await getSignInData(target.value);
	if (!res.ok) return;

	avatar.src = res.data.avatar;
	title.innerText = res.data.title ?? "";
});

passkeyButton.addEventListener("click", async () => {
	try {
		const res = await GetApiPasskeysRequestOptions(name.value);
		if (!res.ok) {
			return;
		}

		const options = PublicKeyCredential.parseRequestOptionsFromJSON(
			res.data as unknown as PublicKeyCredentialRequestOptionsJSON,
		);

		const assertion = (await navigator.credentials.get({ publicKey: options })) as PublicKeyCredential;

		if (!assertion) {
			console.error("No assertion received");
			return;
		}

		const json = JSON.stringify(assertion.toJSON());

		const loginRes = await PostApiPasskeysSignin({ credentials: json });

		if (loginRes.ok) {
			window.location.href = "/";
		}
	} catch (e) {
		console.error("Passkey login failed", e);
	}
});
