import { GetApiInviteCodes, PostApiInviteCodes } from "@g/paths-public";
import type { InviteCodeDto } from "@g/types-public";
import { toCurrentTimezone } from "@h/date-helpers";
import { $id } from "@h/dom";
import { long } from "@h/tinytime-templates";
import { createResource, For, Match, Switch } from "solid-js";
import { render } from "solid-js/web";
import { LucideClipboardCopy } from "../icons/LucideClipboardCopy";

const parent = $id("invite-codes-app");
const date = (dt: string | Date) => long.render(toCurrentTimezone(new Date(dt)));

const csrf = parent.dataset.csrf ?? "";
const max = Number.parseInt(parent.dataset.max ?? "0", 10);

const InviteCodes = () => {
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
		const res = await PostApiInviteCodes({ RequestVerificationToken: csrf });

		if (res.ok && typeof res.data !== "string") {
			const d = res.data;
			mutate((prev) => [...prev, d]);
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
						<For each={codes()}>
							{(code) => (
								<li>
									<div class="deco" style={{ background: code.usedByUserName ? "green" : "gray" }} />
									<div class="main">
										<h3 class="name">
											<span class="monospace">{code.code}</span> : {date(code.issueDate)}
										</h3>
										{code.usedByUserName && code.usedDate ? (
											<span class="desc">
												Redeemed by <strong>{code.usedByUserName}</strong> on{" "}
												<strong>{date(code.usedDate)}</strong>
											</span>
										) : null}
									</div>
									<div class="actions">
										<button
											type="button"
											class="action"
											onClick={[copyCode, code]}
											disabled={!!code.usedByUserName}
										>
											<LucideClipboardCopy />
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
