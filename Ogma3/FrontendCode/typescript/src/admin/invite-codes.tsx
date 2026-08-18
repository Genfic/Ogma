import { DeleteApiInviteCodes, GetApiInviteCodesPaginated, PostApiInviteCodesNoLimit } from "@g/paths-public";
import type { InviteCodeDto } from "@g/types-public";
import { $id } from "@h/dom";
import LucideTrash2 from "icon:lucide:trash-2";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { render } from "solid-js/web";
import { InviteCode } from "../comp/common/_invite-code";

const parent = $id("invite-codes-app");

const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const perPage = 50;

const DeleteButton = ({ code, onDelete }: { code: InviteCodeDto; onDelete: (id: number) => void }) => {
	const deleteCode = async (t: InviteCodeDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiInviteCodes(t.id, headers);
			if (!res.ok) return;
			onDelete(t.id);
		}
	};

	return (
		<button type="button" class="action" onClick={[deleteCode, code]}>
			<LucideTrash2 />
		</button>
	);
};

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

			throw new Error(res.data ?? res.statusText);
		},
	);

	const createCode = async () => {
		const res = await PostApiInviteCodesNoLimit(headers);

		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
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

	const deleteHandler = (id: number) => {
		mutate((prev) => prev?.filter((i: InviteCodeDto) => i.id !== id));
	};

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
						<For each={codes()}>
							{(code) => (
								<InviteCode
									code={code}
									class={code.id === newCode && "hl"}
									actions={<DeleteButton code={code} onDelete={deleteHandler} />}
								/>
							)}
						</For>

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
