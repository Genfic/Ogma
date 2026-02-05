import { GetApiSigninPasskeyOptions as getOptions, PostApiSigninPasskeyRegister as setPasskey } from "@g/paths-public";
import { log } from "@h/logger";
import { component } from "@h/web-components";
import { clsx } from "clsx";
import { attemptAsync } from "es-toolkit";
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

		const options = PublicKeyCredential.parseCreationOptionsFromJSON(
			res.data as unknown as PublicKeyCredentialCreationOptionsJSON,
		);

		const [error, credentials] = await attemptAsync(async () => {
			return (await navigator.credentials.create({ publicKey: options })) as PublicKeyCredential;
		});

		if (!credentials || error) {
			log.warn(`Failed to create credentials: ${error}`);
			loading = false;
			return;
		}

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
