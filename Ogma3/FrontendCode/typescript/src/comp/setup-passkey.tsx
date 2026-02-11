import { GetApiSigninPasskeyOptions as getOptions, PostApiSigninPasskeyRegister as setPasskey } from "@g/paths-public";
import { attempt, attemptAsync } from "@h/error-helpers";
import { log } from "@h/logger";
import { component } from "@h/web-components";
import { clsx } from "clsx";
import { noShadowDOM } from "solid-element";
import type { Component } from "solid-js";
import css from "./setup-passkey.css";

const makeRaw = (data: ArrayBuffer): string =>
	new Uint8Array(data).toBase64({
		alphabet: "base64url",
		omitPadding: true,
	});

const SetupPasskey: Component = () => {
	noShadowDOM();

	let loading = $signal(false);

	const setup = async () => {
		loading = true;
		const res = await getOptions();

		if (!res.ok) {
			throw res.error;
		}

		const optionsResult = attempt(() =>
			PublicKeyCredential.parseCreationOptionsFromJSON(
				res.data as unknown as PublicKeyCredentialCreationOptionsJSON,
			),
		);

		if (!optionsResult.success) {
			log.warn(`Failed to parse options: ${optionsResult.error}`);
			loading = false;
			return;
		}

		const options = optionsResult.value;

		const credentialsResult = await attemptAsync(async () => {
			return (await navigator.credentials.create({ publicKey: options })) as PublicKeyCredential;
		});

		if (!credentialsResult.success) {
			log.warn(`Failed to create credentials: ${credentialsResult.error}`);
			loading = false;
			return;
		}
		const credentials = credentialsResult.value;

		const response = credentials.response as AuthenticatorAttestationResponse;

		log.info(`Credentials created: ${credentials.id}, ${credentials.type}`);

		const payload = {
			id: credentials.id,
			rawId: makeRaw(credentials.rawId),
			type: credentials.type,
			response: {
				attestationObject: makeRaw(response.attestationObject),
				clientDataJSON: makeRaw(response.clientDataJSON),
			},
			clientExtensionResults: credentials.getClientExtensionResults(),
		};

		const keyRes = await setPasskey({
			credentials: JSON.stringify(payload),
		});

		if (!keyRes.ok) {
			throw keyRes.error;
		}

		loading = false;
	};

	return (
		<button class={clsx(["btn", loading && "loading"])} type="button" onClick={setup}>
			Add Passkey
		</button>
	);
};

component("setup-passkey", {}, SetupPasskey, css);
