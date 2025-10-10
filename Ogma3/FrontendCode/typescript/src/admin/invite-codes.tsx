import { DeleteApiInviteCodes, GetApiInviteCodesPaginated, PostApiInviteCodesNoLimit } from "@g/paths-public";
import type { InviteCodeDto } from "@g/types-public";
import { $id } from "@h/dom";
import { log } from "@h/logger";
import { long } from "@h/tinytime-templates";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { render } from "solid-js/web";
import { LucideClipboardCopy } from "../icons/LucideClipboardCopy";
import { LucideTrash2 } from "../icons/LucideTrash2";

const parent = $id("invite-codes-app");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const perPage = 50;

const InviteCodes = () => {
	let isAnyMore = $signal(true);
	let newCode = $signal<number | null>(null);
	let page = $signal(1);

	const [codes, { mutate }] = createResource(
		() => page,
		async (p) => {
			const res = await GetApiInviteCodesPaginated(p, perPage);

			if (res.ok) {
				if (res.data.length < perPage) {
					isAnyMore = false;
				}
				return res.data;
			}

			throw res.error;
		},
	);

	const createCode = async () => {
		const res = await PostApiInviteCodesNoLimit(headers);

		if (!res.ok) {
			throw res.error;
		}

		newCode = res.data.id;

		setTimeout(() => {
			newCode = null;
		}, 5000);

		mutate((old) => (old ? [res.data, ...old] : [res.data]));
	};

	const loadMore = () => {
		page++;
	};

	const copyCode = (t: InviteCodeDto) => {
		navigator.clipboard.writeText(t.code).then(
			() => alert("Copied"),
			(e) => {
				alert("Could not copy");
				log.error(e);
			},
		);
	};

	const deleteCode = async (t: InviteCodeDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiInviteCodes(t.id, headers);

			if (!res.ok) return;

			mutate((prev) => prev?.filter((i: InviteCodeDto) => i.id !== t.id));
		}
	};

	const date = (dt: Date) => long.render(new Date(dt));

	const Code = ({ c }: { c: InviteCodeDto }) => (
		<li classList={{ hl: c.id === newCode }}>
			<div class="deco" style={{ background: c.usedDate ? "green" : "gray" }} />

			<div class="main">
				<h3 class="name">
					<span class="monospace">{c.code}</span>
				</h3>
				<span class="desc">
					<span>
						Issued by <strong>{c.issuedByUserName}</strong> on <strong>{date(c.issueDate)}</strong>
					</span>
					<br />
					<Show when={c.usedDate}>
						<span>
							Redeemed by <strong>{c.usedByUserName}</strong> on{" "}
							<strong>{c.usedDate && date(c.usedDate)}</strong>
						</span>
					</Show>
				</span>
			</div>

			<div class="actions">
				<button type="button" class="action" onClick={[deleteCode, c]}>
					<LucideTrash2 />
				</button>
				<button type="button" class="action" onClick={[copyCode, c]}>
					<LucideClipboardCopy />
				</button>
			</div>
		</li>
	);

	return (
		<>
			<button type="button" class="btn btn-primary btn-block" onclick={createCode}>
				Issue code
			</button>

			<Switch>
				<Match when={codes.loading}>
					<span class="spinner-grow spinner-grow-sm" aria-hidden="true" />
					Loading...
				</Match>
				<Match when={codes}>
					<ul class="items-list">
						<For each={codes()}>{(code) => <Code c={code} />}</For>

						<Show when={!isAnyMore}>
							<li>
								<span class="ph">That's it!</span>
							</li>
						</Show>
					</ul>
				</Match>
			</Switch>

			<Show when={isAnyMore}>
				<button type="button" class="btn" onclick={loadMore}>
					{codes.loading ? "Loading..." : "Load More"}
				</button>
			</Show>
		</>
	);
};

render(() => <InviteCodes />, parent);
