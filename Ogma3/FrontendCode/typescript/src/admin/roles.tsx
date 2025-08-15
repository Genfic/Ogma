// @ts-ignore
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
						<span class="spinner-grow spinner-grow-sm" role="status" aria-hidden="true" />
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
											{role.name}
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

// @ts-ignore
// new Vue({
// 	el: "#app",
// 	data: {
// 		form: {
// 			name: null,
// 			color: null,
// 			order: null,
// 			isStaff: null,
// 			id: null,
// 		} as RoleDto,
// 		roles: [] as RoleDto[],
// 		xcsrf: null,
// 	},
// 	methods: {
// 		// Contrary to its name, it also modifies a role if needed.
// 		// It was simply easier to slap both functionalities into a single function.
// 		createRole: async function (e: Event) {
// 			e.preventDefault();
// 			if (this.form.name) {
// 				const data = {
// 					name: this.form.name,
// 					color: this.form.color,
// 					isStaff: this.form.isStaff,
// 					order: Number(this.form.order),
// 				};
//
// 				const headers = { RequestVerificationToken: this.xcsrf };
//
// 				if (this.form.id === null) {
// 					// If no ID has been set, that means it's a new role.
// 					// Thus, we POST it.
// 					const res = await PostApiRoles(data, headers);
// 					if (res.ok) {
// 						await this.getRoles();
// 					}
// 				} else {
// 					// If the ID is set, that means it's an existing role.
// 					// Thus, we PUT it.
// 					const res = await PutApiRoles({ id: this.form.id, ...data }, headers);
// 					if (res.ok) {
// 						await this.getRoles();
// 						this.cancelEdit();
// 					}
// 				}
// 			}
// 		},
//
// 		// Gets all existing roles
// 		getRoles: async function () {
// 			const res = await GetApiRoles();
// 			if (res.ok) {
// 				this.roles = res.data;
// 			}
// 		},
//
// 		// Deletes a selected role
// 		deleteRole: async function (t: RoleDto) {
// 			if (confirm("Delete permanently?")) {
// 				const res = await DeleteApiRoles(t.id, { RequestVerificationToken: this.xcsrf });
// 				if (res.ok) {
// 					await this.getRoles();
// 				}
// 			}
// 		},
//
// 		// Throws a role from the list into the editor
// 		editRole: function (t: RoleDto) {
// 			this.form.name = t.name;
// 			this.form.color = t.color;
// 			this.form.id = t.id;
// 			this.form.order = t.order;
// 			this.form.isStaff = t.isStaff;
// 		},
//
// 		// Clears the editor
// 		cancelEdit: function () {
// 			this.form.name = this.form.color = this.form.id = this.form.isStaff = this.form.order = null;
// 		},
// 	},
//
// 	async mounted() {
// 		// Grab the XCSRF token
// 		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
// 		// Grab the initial set of roles
// 		await this.getRoles();
// 	},
// });
