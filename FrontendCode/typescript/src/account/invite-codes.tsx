import { GetApiInviteCodes, PostApiInviteCodes } from "@g/paths-public";
import type { InviteCodeDto } from "@g/types-public";
import { $id } from "@h/dom";
import { createResource, For, Match, Switch } from "solid-js";
import { render } from "solid-js/web";
import { InviteCode } from "../comp/common/_invite-code";

const parent = $id("invite-codes-app");

const csrf = parent.dataset.csrf ?? "";
const max = Number.parseInt(parent.dataset.max ?? "0", 10);

const InviteCodes = () => {
	const [codes, { mutate }] = createResource<InviteCodeDto[]>(
		async () => {
			const res = await GetApiInviteCodes();
			if (!res.ok) {
				throw new Error(res.statusText);
			}
			return res.data;
		},
		{ initialValue: [] },
	);

	const createCode = async () => {
		const res = await PostApiInviteCodes({ RequestVerificationToken: csrf });

		if (res.ok) {
			mutate((prev) => [res.data, ...prev]);
		} else {
			console.error(res.data ?? res.statusText);
		}
	};

	return (
		<>
			<button type="button" class="btn btn-primary btn-block" onClick={createCode}>
				Issue code ({codes().length}/{max})
			</button>

			<Switch>
				<Match when={codes.loading}>
					<span class="spinner-grow spinner-grow-sm" aria-hidden="true" />
					Loading...
				</Match>
				<Match when={codes.error}>
					<span class="error">{codes.error}</span>
				</Match>
				<Match when={codes()}>
					<ul class="items-list">
						<For each={codes()}>{(code) => <InviteCode code={code} />}</For>
					</ul>
				</Match>
			</Switch>
		</>
	);
};

render(() => <InviteCodes />, parent);
