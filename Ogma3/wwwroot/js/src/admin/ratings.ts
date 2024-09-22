import { DeleteApiRatings as deleteRating, GetRatings as getRatings } from "../../generated/paths-public";
import type { RatingApiDto } from "../../generated/types-public";

// @ts-ignore
new Vue({
	el: "#app",
	data: {
		form: {
			name: null,
			desc: null,
			icon: null,
			blacklist: null,
			id: null,
			order: null,
		},
		err: [],
		ratings: [] as (RatingApiDto & { color: string })[],
		route: null,
		xcsrf: null,
	},
	methods: {
		iconChanged: function (e: Event) {
			this.form.icon = (e.target as HTMLFormElement).files[0];
		},

		// Contrary to its name, it also modifies a namespace if needed.
		// It was simply easier to slap both functionalities into a single function.
		createRating: async function (e: Event) {
			e.preventDefault();

			if (this.form.name) {
				const data = new FormData();

				data.append("name", this.form.name);
				data.append("description", this.form.desc);
				data.append("blacklistedByDefault", (this.form.blacklist ?? false).toString());
				data.append("order", this.form.order);
				if (this.form.icon) data.append("icon", this.form.icon, this.form.icon.name);

				const headers = { RequestVerificationToken: this.xcsrf };

				if (this.form.id === null) {
					// If no ID has been set, that means it's a new rating.
					// Thus, we POST it.
					// TODO: Shit's blocked by https://github.com/RicoSuter/NSwag/issues/4626
					const res = await fetch(this.route, {
						method: "POST",
						body: data,
						headers: headers,
					});
					if (!res.ok) return;
					await this.getRatings();
				} else {
					// If the ID is set, that means it's an existing namespace.
					// Thus, we PUT it.
					data.append("id", this.form.id);
					// TODO: Shit's blocked by https://github.com/RicoSuter/NSwag/issues/4626
					const res = await fetch(this.route, {
						method: "PUT",
						body: data,
						headers: headers,
					});
					if (!res.ok) return;
					await this.getRatings();
				}

				this.cancelEdit();
			}
		},

		// Gets all existing namespaces
		getRatings: async function () {
			const res = await getRatings();
			this.ratings = await res.json();
		},

		// Deletes a selected namespace
		deleteRating: async function (t: RatingApiDto) {
			if (confirm("Delete permanently?")) {
				const res = await deleteRating(t.id, {
					RequestVerificationToken: this.xcsrf,
				});
				if (res.ok) {
					await this.getRatings();
				}
			}
		},

		// Throws a namespace from the list into the editor
		editRating: function (t: RatingApiDto & { color: string }) {
			this.form.name = t.name;
			this.form.color = t.color;
			this.form.id = t.id;
			this.form.order = t.order;
			this.form.blacklist = t.blacklistedByDefault;
			this.form.desc = t.description;
		},

		// Clears the editor
		cancelEdit: function () {
			for (const key of Object.keys(this.form)) {
				this.form[key] = null;
			}
			this.$refs.file.value = null;
		},
	},

	async mounted() {
		this.xcsrf = (document.querySelector("[name=__RequestVerificationToken]") as HTMLInputElement).value;
		await this.getRatings();
	},
});
