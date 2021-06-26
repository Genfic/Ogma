let ratings_vue = new Vue({
	el: "#app",
	data: {
		form: {
			name: null,
			desc: null,
			icon: null,
			blacklist: null,
			id: null,
			order: null
		},
		err: [],
		ratings: [],
		route: null,
		cdn: null,
		xcsrf: null
	},
	methods: {
		iconChanged: function(e) {
			this.form.icon = e.target.files[0];
		},

		// Contrary to its name, it also modifies a namespace if needed.
		// It was simply easier to slap both functionalities into a single function.
		createRating: async function(e) {
			e.preventDefault();

			if (this.form.name) {

				const data = new FormData();

				data.append("name", this.form.name);
				data.append("description", this.form.desc);
				data.append("blacklistedByDefault", (this.form.blacklist ?? false).toString());
				data.append("order", this.form.order);
				if (this.form.icon)
					data.append("icon", this.form.icon, this.form.icon.name);

				// If no ID has been set, that means it's a new rating.
				// Thus, we POST it.
				if (this.form.id === null) {
					await axios.post(this.route, data, { 
						headers: { "RequestVerificationToken": this.xcsrf } 
					});
					await this.getRatings();

					// If the ID is set, that means it's an existing namespace.
					// Thus, we PUT it.
				} else {
					await axios.put(`${this.route}/${this.form.id}`, data, {
						headers: { "RequestVerificationToken": this.xcsrf } 
					});
					await this.getRatings();
					this.cancelEdit();
				}

			}
		},

		// Gets all existing namespaces
		getRatings: async function() {
			const { data } = await axios.get(this.route);
			this.ratings = data;
		},

		// Deletes a selected namespace
		deleteRating: async function(t) {
			if (confirm("Delete permanently?")) {
				await axios.delete(this.route + "/" + t.id);
				await this.getRatings();
			}
		},

		// Throws a namespace from the list into the editor
		editRating: function(t) {
			this.form.name = t.name;
			this.form.color = t.color;
			this.form.id = t.id;
			this.form.order = t.order;
			this.form.blacklist = t.blacklistedByDefault;
			this.form.desc = t.description;
		},

		// Clears the editor
		cancelEdit: function() {
			this.form.name =
				this.form.color =
					this.form.id =
						this.form.desc =
							this.form.order = null;
		}
	},

	async mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
		this.cdn = document.getElementById("cdn").dataset.cdn;
		this.xcsrf = document.querySelector("[name=\"__RequestVerificationToken\"]").value;
		// Grab the initial set of namespaces
		await this.getRatings();
	}
});