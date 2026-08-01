import { DeleteAdminApiInfractions, GetAdminApiUserInfractions } from "@g/paths-internal";
import { toCurrentTimezone } from "@h/date-helpers";
import { EU, iso8601 } from "@h/tinytime-templates";
import { component } from "@h/web-components";
import { noShadowDOM } from "solid-element";
import { createResource, For } from "solid-js";
import { ManageInfraction, type ManageInfractionApi } from "./components/manage-infraction-component";

const date = (dt: Date) => iso8601.render(dt);
const dateEu = (dt: Date) => EU.render(toCurrentTimezone(dt));

const ManageInfractions = (props: { userId: number; csrf: string }) => {
	noShadowDOM();

	const headers = { RequestVerificationToken: props.csrf ?? "" };

	const [infractionsResource, { refetch }] = createResource(async () => {
		const res = await GetAdminApiUserInfractions(props.userId, headers);

		if (res.ok) {
			return res.data;
		}

		throw new Error(res.data ?? res.statusText);
	});

	const infractions = $memo(infractionsResource());
	const userInfractions = $memo(infractions?.infractions ?? []);

	const infractionPopupRef = $signal<ManageInfractionApi>();

	const removeInfraction = async (id: number) => {
		if (!window.confirm(`Are you sure you want to remove infraction #${id}?`)) {
			return;
		}

		const res = await DeleteAdminApiInfractions(id, headers);

		if (!res.ok) {
			throw new Error(res.data ?? res.statusText);
		}

		await refetch();
	};

	const addInfraction = () => {
		infractionPopupRef?.open();
	};

	return (
		<>
			<button type="button" class="btn btn-primary" onClick={addInfraction}>
				Create new infraction
			</button>

			<For each={userInfractions}>
				{(infra) => {
					const isDone = infra.activeUntil < new Date() || infra.removedAt;
					return (
						<details class="infraction details">
							<summary class={isDone ? "passed" : ""}>
								<strong classList={{ type: true, [infra.type.toLowerCase()]: true }}>
									{infra.type}
								</strong>{" "}
								issued <time datetime={date(infra.issueDate)}>{dateEu(infra.issueDate)}</time>, expires{" "}
								<time datetime={date(infra.activeUntil)}>{dateEu(infra.activeUntil)}</time>
							</summary>
							{infra.removedAt && (
								<div class="time">
									Removed on <time datetime={date(infra.removedAt)}>{dateEu(infra.removedAt)}</time>{" "}
									by
									{infra.removedBy}
								</div>
							)}
							<div class="reason">
								<b>Reason:</b> {infra.reason}
							</div>
							{!infra.removedAt && (
								<>
									<br />
									<button type="button" class="btn" onClick={[removeInfraction, infra.id]}>
										Remove
									</button>
								</>
							)}
						</details>
					);
				}}
			</For>

			<ManageInfraction
				ref={$set(infractionPopupRef)}
				csrf={props.csrf}
				types={infractions?.infractionTypes ?? []}
				userId={props.userId}
				onSuccess={refetch}
			/>
		</>
	);
};

component("manage-infractions", { userId: 0, csrf: "" }, ManageInfractions);
