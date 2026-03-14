import {
	DeleteApiPasskeysDelete as deletePasskey,
	GetApiPasskeysList as listPasskeys,
	GetApiPasskeysOptions as getOptions,
	PostApiPasskeysRegister as setPasskey,
} from "@g/paths-public";
import { attempt, attemptAsync } from "@h/error-helpers";
import { log } from "@h/logger";
import { iso8601 } from "@h/tinytime-templates";
import { component } from "@h/web-components";
import { clsx } from "clsx";
import { noShadowDOM } from "solid-element";
import { type Component, createResource, For, Show } from "solid-js";
import css from "./setup-passkey.css";

const makeRaw = (data: ArrayBuffer): string =>
	new Uint8Array(data).toBase64({
		alphabet: "base64url",
		omitPadding: true,
	});

const SetupPasskey: Component<{ csrf: string }> = (props) => {
	noShadowDOM();

	const headers = { RequestVerificationToken: props.csrf };

	const [passkeys, { mutate }] = createResource(async () => {
		const res = await listPasskeys();
		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
		}
		return res.data;
	});

	let loading = $signal(false);
	let name = $signal<string | null>(null);
	let errors = $signal<string[]>([]);

	const remove = async (id: string) => {
		if (!confirm("Are you sure you want to delete the passkey?")) {
			return;
		}

		const res = await deletePasskey(encodeURIComponent(id), headers);
		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
		}
		mutate((old) => old?.filter((p) => p.id !== id));
	};

	const setup = async () => {
		loading = true;
		const res = await getOptions();

		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
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

		const keyRes = await setPasskey(
			{
				credentials: JSON.stringify(payload),
				name,
			},
			headers,
		);

		if (keyRes.status === 400) {
			errors = keyRes.data ?? [];
			return;
		}

		if (!keyRes.ok) {
			return;
		}

		const newKey = keyRes.data;
		mutate((old) => [...(old ?? []), newKey]);

		loading = false;
	};

	return (
		<>
			<h3>Existing passkeys</h3>
			<br />
			<Show when={!passkeys.loading} fallback={<p>Loading passkeys...</p>}>
				<table class="o-table">
					<thead>
						<tr>
							<td>Name</td>
							<td>Created At</td>
						</tr>
					</thead>
					<tbody>
						<For each={passkeys()}>
							{(key) => (
								<tr>
									<td>{key.name ?? "[unnamed]"}</td>
									<td>
										<time datetime={key.creationDate.toISOString()}>
											{iso8601.render(key.creationDate)}
										</time>
									</td>
									<td>
										<button title={key.id} type="button" class="btn" onclick={[remove, key.id]}>
											Delete
										</button>
									</td>
								</tr>
							)}
						</For>
					</tbody>
				</table>
			</Show>

			<br />
			<hr />

			<h3>Add passkey</h3>
			<form class="form">
				<div class="o-form-group">
					<label for="passkey-name">Name (optional)</label>
					<input
						id="passkey-name"
						type="text"
						onChange={({ currentTarget }) => (name = currentTarget.value)}
					/>
				</div>

				<button class={clsx(["btn", "wide", loading && "loading"])} type="button" onClick={setup}>
					Add Passkey
				</button>
			</form>
		</>
	);
};

component("setup-passkey", { csrf: "" }, SetupPasskey, css);
