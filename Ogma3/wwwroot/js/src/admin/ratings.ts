import { DeleteApiRatings as deleteRating, GetRatings as getRatings } from "../../generated/paths-public";

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
		ratings: [],
		route: null,
		xcsrf: null,
	},
	methods: {
		iconChanged: function (e) {
			this.form.icon = e.target.files[0];
		},

		// Contrary to its name, it also modifies a namespace if needed.
		// It was simply easier to slap both functionalities into a single function.
		createRating: async function (e) {
			e.preventDefault();

			if (this.form.name) {
				const data = new FormData();

				data.append("name", this.form.name);
				data.append("description", this.form.desc);
				data.append("blacklistedByDefault", (this.form.blacklist ?? false).toString());
				data.append("order", this.form.order);
				if (this.form.icon) data.append("icon", this.form.icon, this.form.icon.name);

				const options = {
					headers: { RequestVerificationToken: this.xcsrf },
				};

				// If no ID has been set, that means it's a new rating.
				// Thus, we POST it.
				if (this.form.id === null) {
					// TODO: Shit's blocked by https://github.com/RicoSuter/NSwag/issues/4626
					await axios.post(this.route, data, options);
					await this.getRatings();

					// If the ID is set, that means it's an existing namespace.
					// Thus, we PUT it.
				} else {
					data.append("id", this.form.id);
					// TODO: Shit's blocked by https://github.com/RicoSuter/NSwag/issues/4626
					await axios.put(this.route, data, options);
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
		deleteRating: async function (t) {
			if (confirm("Delete permanently?")) {
				const res = await deleteRating(t.id, { RequestVerificationToken: this.xcsrf });
				if (res.ok) {
					await this.getRatings();
				}
			}
		},

		// Throws a namespace from the list into the editor
		editRating: function (t) {
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
		this.xcsrf = document.querySelector("[name=__RequestVerificationToken]").value;
		await this.getRatings();
	},
});
