import { PostApiInviteCodes, GetApiInviteCodes } from "@g/paths-public";
import type { InviteCodeDto } from "@g/types-public";
import { long } from "@h/tinytime-templates";
import { createResource, createSignal, For, Match, Show, Switch } from "solid-js";
import { $id } from "@h/dom";
import { render } from "solid-js/web";

const parent = $id("invite-codes-app");
const date = (dt: string | Date) => long.render(new Date(dt));

const InviteCodes = () => {
	const [csrf] = createSignal<string>(parent.dataset.csrf ?? "");
	const [max] = createSignal(Number.parseInt(parent.dataset.max ?? "0"));

	const [codes, { mutate }] = createResource<InviteCodeDto[]>(
		async () => {
			const res = await GetApiInviteCodes();
			if (!res.ok) {
				throw new Error(res.error);
			}
			return res.data;
		},
		{ initialValue: [] },
	);

	const createCode = async () => {
		const res = await PostApiInviteCodes({ RequestVerificationToken: csrf() });

		if (res.ok && Array.isArray(res.data)) {
			mutate((prev) => [...prev, res.data as InviteCodeDto]);
		} else {
			console.log(res.ok ? res.data : res.error);
		}
	};

	const copyCode = ({ code }: InviteCodeDto) => {
		navigator.clipboard.writeText(code).then(
			() => alert("Copied"),
			(e) => {
				alert("Could not copy");
				console.error(e);
			},
		);
	};

	return (
		<>
			<button type="button" class="btn btn-primary btn-block" onClick={createCode}>
				Issue code ({codes().length}/{max()})
			</button>

			<Switch>
				<Match when={codes.loading}>
					<span class="spinner-grow spinner-grow-sm" role="status" aria-hidden="true" />
					Loading...
				</Match>
				<Match when={codes.error}>
					<span class="error">{codes.error}</span>
				</Match>
				<Match when={codes()}>
					<ul class="items-list">
						<For each={codes()}>
							{(code) => (
								<li>
									<div class="deco" style={{ background: code.usedByUserName ? "green" : "gray" }} />
									<div class="main">
										<h3 class="name">
											<span class="monospace">{code.code}</span> : {date(code.issueDate)}
										</h3>
										<Show when={code.usedByUserName}>
											<span class="desc">
												Redeemed by <strong>{code.usedByUserName}</strong> on{" "}
												<strong>{date(code.usedDate)}</strong>
											</span>
										</Show>
									</div>
									<div class="actions">
										<button
											type="button"
											class="action"
											onClick={[copyCode, code]}
											disabled={!!code.usedByUserName}
										>
											<o-icon icon="lucide:clipboard-copy" />
										</button>
									</div>
								</li>
							)}
						</For>
					</ul>
				</Match>
			</Switch>
		</>
	);
};

render(() => <InviteCodes />, parent);
