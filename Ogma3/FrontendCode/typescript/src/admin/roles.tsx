import { DeleteApiRoles, GetApiRoles, PostApiRoles, PutApiRoles } from "@g/paths-public";
import type { RoleDto } from "@g/types-public";
import { $id } from "@h/dom";
import { createTypeGuard, makeEmpty } from "@h/type-helpers";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import { LucidePencil } from "../icons/LucidePencil";
import { LucideShieldHalf } from "../icons/LucideShieldHalf";
import { LucideTrash2 } from "../icons/LucideTrash2";

const parent = $id("roles-app");
const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const formGuard = createTypeGuard<RoleDto>("id", "name", "color", "isStaff", "order");

const Roles = () => {
	const [form, setForm] = createStore<Partial<RoleDto>>({});

	const [roles, { refetch: refetchRoles }] = createResource(async () => {
		const res = await GetApiRoles();
		if (!res.ok) throw res.error;
		return res.data;
	});

	const createRole = async (e: Event) => {
		e.preventDefault();

		if (!formGuard(form)) return;
		const { id, name, color, isStaff, order } = form;

		const data = {
			name,
			color,
			isStaff,
			order,
		};

		if (id) {
			const res = await PutApiRoles({ id, ...data }, headers);
			if (res.ok) {
				await refetchRoles();
				cancelEdit();
			}
		} else {
			const res = await PostApiRoles(data, headers);
			if (res.ok) {
				await refetchRoles();
			}
		}
	};

	const deleteRole = async (role: RoleDto) => {
		if (confirm("Delete permanently?")) {
			const res = await DeleteApiRoles(role.id, headers);
			if (res.ok) {
				await refetchRoles();
			}
		}
	};

	const editRole = (role: RoleDto) => {
		setForm(role);
	};

	const cancelEdit = () => {
		setForm(makeEmpty);
	};

	return (
		<>
			<form id="namespace" class="auto" method="post" onsubmit={createRole}>
				<label for="role-name">Name</label>
				<input
					id="role-name"
					type="text"
					class="o-form-control"
					value={form.name ?? ""}
					oninput={({ target }) => setForm("name", target.value)}
				/>

				<label for="role-color">Color</label>
				<input
					id="role-color"
					type="color"
					class="o-form-control"
					value={form.color ?? ""}
					oninput={({ target }) => setForm("color", target.value)}
				/>

				<label for="role-isStaff">Is Staff</label>
				<input
					id="role-isStaff"
					type="checkbox"
					class="o-form-control"
					checked={form.isStaff ?? false}
					oninput={({ target }) => setForm("isStaff", target.checked)}
				/>

				<label for="role-order">Order</label>
				<input
					id="role-order"
					type="number"
					class="o-form-control"
					min="0"
					value={form.order ?? ""}
					oninput={({ target }) => setForm("order", Number(target.value))}
				/>

				<input type="hidden" value={form.id ?? ""} />

				<div class="form-row">
					<button type="submit" class="btn btn-primary">
						{form.id ? "Edit" : "Add"}
					</button>
					<Show when={form.id}>
						<button class="btn btn-secondary" type="button" onclick={cancelEdit}>
							Cancel
						</button>
					</Show>
				</div>
			</form>

			<br />

			<Switch>
				<Match when={roles.loading}>
					<button class="btn btn-primary" type="button" disabled>
						<span class="spinner-grow spinner-grow-sm" aria-hidden="true" />
						Loading...
					</button>
				</Match>
				<Match when={roles.error}>{roles.error}</Match>
				<Match when={roles}>
					<ul class="items-list">
						<For each={roles()}>
							{(role) => (
								<li>
									<div class="deco" style={{ background: role.color || "" }}>
										<span class="text">{role.order}</span>
									</div>
									<div class="main">
										<h3 class="name">
											<span>{role.id}</span>|<span>{role.name}</span>
											<Show when={role.isStaff}>
												<LucideShieldHalf />
											</Show>
										</h3>
									</div>
									<div class="actions">
										<button type="button" class="action" onclick={[deleteRole, role]}>
											<LucideTrash2 />
										</button>
										<button type="button" class="action" onclick={[editRole, role]}>
											<LucidePencil />
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

render(() => <Roles />, parent);
