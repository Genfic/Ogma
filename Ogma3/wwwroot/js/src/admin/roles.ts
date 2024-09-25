// @ts-ignore
import { DeleteApiRoles, GetApiRoles, PostApiRoles, PutApiRoles } from "../../generated/paths-public";
import type { RoleDto } from "../../generated/types-public";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		form: {
			name: null,
			color: null,
			order: null,
			isStaff: null,
			id: null,
		} as RoleDto,
		roles: [] as RoleDto[],
		xcsrf: null,
	},
	methods: {
		// Contrary to its name, it also modifies a role if needed.
		// It was simply easier to slap both functionalities into a single function.
		createRole: async function (e: Event) {
			e.preventDefault();
			if (this.form.name) {
				const data = {
					name: this.form.name,
					color: this.form.color,
					isStaff: this.form.isStaff,
					order: Number(this.form.order),
				};

				const headers = { RequestVerificationToken: this.xcsrf };

				if (this.form.id === null) {
					// If no ID has been set, that means it's a new role.
					// Thus, we POST it.
					const res = await PostApiRoles(data, headers);
					if (res.ok) {
						await this.getRoles();
					}
				} else {
					// If the ID is set, that means it's an existing role.
					// Thus, we PUT it.
					const res = await PutApiRoles({ id: this.form.id, ...data }, headers);
					if (res.ok) {
						await this.getRoles();
						this.cancelEdit();
					}
				}
			}
		},

		// Gets all existing roles
		getRoles: async function () {
			const res = await GetApiRoles();
			if (res.ok) {
				this.roles = await res.json();
			}
		},

		// Deletes a selected role
		deleteRole: async function (t: RoleDto) {
			if (confirm("Delete permanently?")) {
				const res = await DeleteApiRoles(t.id, { RequestVerificationToken: this.xcsrf });
				if (res.ok) {
					await this.getRoles();
				}
			}
		},

		// Throws a role from the list into the editor
		editRole: function (t: RoleDto) {
			this.form.name = t.name;
			this.form.color = t.color;
			this.form.id = t.id;
			this.form.order = t.order;
			this.form.isStaff = t.isStaff;
		},

		// Clears the editor
		cancelEdit: function () {
			this.form.name = this.form.color = this.form.id = this.form.isStaff = this.form.order = null;
		},
	},

	async mounted() {
		// Grab the XCSRF token
		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
		// Grab the initial set of roles
		await this.getRoles();
	},
});
