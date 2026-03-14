import { DeleteApiRoles, GetApiRoles, PostApiRoles, PutApiRoles } from "@g/paths-public";
import type { RoleDto } from "@g/types-public";
import { $id } from "@h/dom";
import { getFormData } from "@h/form-helpers";
import { checkboxBool, strippedHexColor } from "@h/valibot-schemas";
import { createResource, For, Match, Show, Switch } from "solid-js";
import { createStore } from "solid-js/store";
import { render } from "solid-js/web";
import * as v from "valibot";
import { LucidePencil } from "../icons/LucidePencil";
import { LucideShieldHalf } from "../icons/LucideShieldHalf";
import { LucideTrash2 } from "../icons/LucideTrash2";

const parent = $id("roles-app");
const headers = { RequestVerificationToken: parent.dataset.csrf ?? "" };

const RoleSchema = v.object({
	name: v.string(),
	color: v.nullable(strippedHexColor),
	isStaff: checkboxBool,
	order: v.pipe(v.string(), v.transform(Number), v.minValue(0), v.maxValue(255), v.integer()),
	id: v.optional(v.pipe(v.string(), v.transform(Number), v.integer())),
});

type Role = v.InferOutput<typeof RoleSchema>;

const EmptyRole = {
	id: undefined,
	name: "",
	color: "",
	isStaff: false,
	order: 0,
} satisfies Role;

const Roles = () => {
	const [form, setForm] = createStore<Role>(EmptyRole);

	const [roles, { refetch: refetchRoles }] = createResource(async () => {
		const res = await GetApiRoles();
		return res.data.map((r) => ({ ...r, color: r.color ? `#${r.color.replace("#", "")}` : "" }));
	});

	const createRole = async (e: SubmitEvent) => {
		e.preventDefault();

		const [error, data] = getFormData(e, RoleSchema);

		if (error) {
			console.error(error);
			return;
		}

		if (data.id) {
			const res = await PutApiRoles({ ...data, id: data.id }, headers);
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
		setForm(EmptyRole);
	};

	return (
		<>
			<form id="namespace" class="auto" method="post" onsubmit={createRole}>
				<label for="role-name">Name</label>
				<input id="role-name" type="text" name="name" class="o-form-control" prop:value={form.name} />

				<label for="role-color">Color</label>
				<input id="role-color" type="color" name="color" class="o-form-control" prop:value={form.color} />

				<label for="role-isStaff">Is Staff</label>
				<input
					id="role-isStaff"
					type="checkbox"
					name="isStaff"
					class="o-form-control"
					prop:checked={form.isStaff}
				/>

				<label for="role-order">Order</label>
				<input
					id="role-order"
					type="number"
					name="order"
					class="o-form-control"
					min="0"
					prop:value={form.order}
				/>

				<Show when={form.id}>
					<input type="hidden" name="id" prop:value={form.id} />
				</Show>

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
									<div class="deco" style={{ background: role.color }}>
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
