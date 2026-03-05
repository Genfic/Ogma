import { DeleteAdminApiInfractions, GetAdminApiUsers } from "@g/paths-internal";
import { PostApiUsersRoles } from "@g/paths-public";
import type { InfractionType } from "@g/types-internal";
import { toCurrentTimezone } from "@h/date-helpers";
import { $id } from "@h/dom";
import { EU, iso8601 } from "@h/tinytime-templates";
import { compact } from "es-toolkit";
import { createResource, For } from "solid-js";
import { Portal, render } from "solid-js/web";
import { ManageInfraction, type ManageInfractionApi } from "./components/manage-infraction-component";

const parent = $id("users-app");
const csrf = parent.dataset.csrf;
const headers = { RequestVerificationToken: csrf ?? "" };
const infractions = JSON.parse(parent.dataset.infractions ?? "[]") as InfractionType[];
const roles = JSON.parse(parent.dataset.roles ?? "[]") as { Id: number; Name: string }[];
const name = parent.dataset.name as string | null;

const date = (dt: Date) => iso8601.render(dt);
const dateEu = (dt: Date) => EU.render(toCurrentTimezone(dt));

const Users = () => {
	const [userResource, { refetch }] = createResource(async () => {
		if (name === null) {
			return null;
		}

		const res = await GetAdminApiUsers(name, headers);

		if (res.ok) {
			return res.data;
		}

		throw res.error;
	});

	const user = $memo(userResource());

	let userRoles = $signal<number[]>([]);
	const userInfractions = $memo(user?.infractions ?? []);

	$effect(() => {
		userRoles = compact(
			user?.roleNames.map((r) => roles.find((rr) => rr.Name.toLowerCase() === r.toLowerCase())?.Id) ?? [],
		);
	});

	const imageRef = $signal<HTMLImageElement>();
	const infractionPopupRef = $signal<ManageInfractionApi>();

	const updateRoles = (role: number, checked: boolean) => {
		if (checked) {
			userRoles = [...userRoles, role];
		} else {
			userRoles = userRoles.filter((r) => r !== role);
		}
	};

	const showImage = () => {
		if (!imageRef) return;
		imageRef.style.display = "block";
	};

	const updateImage = (e: MouseEvent) => {
		if (!imageRef) return;
		imageRef.style.left = `${e.clientX}px`;
		imageRef.style.top = `${e.clientY}px`;
	};

	const hideImage = () => {
		if (!imageRef) return;
		imageRef.style.display = "none";
	};

	const removeInfraction = async (id: number) => {
		if (!window.confirm(`Are you sure you want to remove infraction #${id}?`)) {
			return;
		}

		const res = await DeleteAdminApiInfractions(id, headers);

		if (!res.ok) {
			console.error(res.error);
		}

		refetch();
	};

	const addInfraction = () => {
		infractionPopupRef?.open();
	};

	const saveRoles = async () => {
		if (!user) return;

		const res = await PostApiUsersRoles(
			{
				userId: user.id,
				roles: userRoles,
			},
			headers,
		);

		if (!res.ok) {
			console.error(res.error);
		}
	};

	return (
		<>
			{user ? (
				<>
					<h3>User Info</h3>

					<a href={`/user/${user.name.toLowerCase()}`}>Visit profile</a>

					<table class="o-table">
						<tbody>
							<tr>
								<td>Id</td>
								<td id="id">{user.id}</td>
							</tr>
							<tr>
								<td>Name</td>
								<td>{user.name}</td>
							</tr>
							<tr>
								<td>Email</td>
								<td>{user.email}</td>
							</tr>
							<tr>
								<td>Title</td>
								<td>{user.title}</td>
							</tr>
							<tr>
								<td>Avatar</td>
								<td>
									<a
										href={user.avatar ?? ""}
										target="_blank"
										onFocus={() => null}
										onMouseOver={showImage}
										onMouseMove={updateImage}
										onMouseLeave={hideImage}
									>
										{user.avatar ?? "No avatar"}
									</a>
								</td>
							</tr>
							<tr>
								<td class="nb">Registration date</td>
								<td>{dateEu(user.registrationDate)}</td>
							</tr>
							<tr>
								<td class="nb">Last active</td>
								<td>{dateEu(user.lastActive)}</td>
							</tr>
							<tr>
								<td class="nb">Stories count</td>
								<td>{user.storiesCount}</td>
							</tr>
							<tr>
								<td class="nb">Blogposts count</td>
								<td>{user.blogpostsCount}</td>
							</tr>
							<tr>
								<td>Roles</td>
								<td>
									<div class="select-group">
										<For each={roles}>
											{(role) => (
												<>
													<input
														id={role.Name}
														type="checkbox"
														name="roles"
														value={role.Id}
														checked={userRoles.includes(role.Id)}
														onChange={(e) => updateRoles(role.Id, e.currentTarget.checked)}
													/>
													<label for={role.Name}>{role.Name}</label>
												</>
											)}
										</For>
									</div>
									<button type="button" onClick={saveRoles}>
										Save
									</button>
								</td>
							</tr>
						</tbody>
					</table>

					<br />
					<h3>Infractions</h3>

					<button type="button" class="btn btn-primary" onClick={addInfraction}>
						Create new
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
										issued <time datetime={date(infra.issueDate)}>{dateEu(infra.issueDate)}</time>,
										expires{" "}
										<time datetime={date(infra.activeUntil)}>{dateEu(infra.activeUntil)}</time>
									</summary>
									{infra.removedAt && (
										<div class="time">
											Removed on{" "}
											<time datetime={date(infra.removedAt)}>{dateEu(infra.removedAt)}</time> by
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
						csrf={csrf ?? ""}
						types={infractions}
						userId={user.id}
						onSuccess={refetch}
					/>
				</>
			) : (
				<form class="form" method="get">
					<div class="form-row">
						<div class="o-form-group">
							<label for="name">Go to user</label>
							<input class="o-form-control active-border" type="text" name="name" id="name" />
						</div>

						<div class="o-form-group keep-size">
							<input class="o-form-control active-border" type="submit" value="Go" />
						</div>
					</div>
				</form>
			)}

			<Portal>
				<img
					src={user?.avatar ?? ""}
					alt="Avatar"
					ref={$set(imageRef)}
					style={{ display: "none", position: "absolute", "pointer-events": "none" }}
				/>
			</Portal>
		</>
	);
};

render(() => <Users />, parent);
