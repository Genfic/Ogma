new Vue({
	el: "#app",
	data: {
		form: {
			name: null,
			color: null,
			order: null,
			id: null
		},
		lens: {
			minNameLength: 5,
			maxNameLength: 10
		},
		err: [],
		namespaces: [],
		route: null,
		color: null
	},
	methods: {

		// Contrary to its name, it also modifies a namespace if needed.
		// It was simply easier to slap both functionalities into a single function.
		createNamespace: async function(e) {
			e.preventDefault();

			// Validation
			this.err = [];
			if (this.form.name.length > this.lens.maxNameLength || this.form.name.length < this.lens.minNameLength)
				this.err.push(`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`);
			if (this.err.length > 0) return;

			if (this.form.name) {

				// If no ID has been set, that means it's a new namespace.
				// Thus, we POST it.
				if (this.form.id === null) {
					await axios.post(this.route,
						{
							name: this.form.name,
							color: this.form.color,
							order: Number(this.form.order)
						});
					await this.getNamespaces();

					// If the ID is set, that means it's an existing namespace.
					// Thus, we PUT it.
				} else {
					await axios.put(`${this.route}/${this.form.id}`,
						{
							id: this.form.id,
							name: this.form.name,
							color: this.form.color,
							order: Number(this.form.order)
						});
					await this.getNamespaces();
					this.form.name =
						this.form.id = null;
				}

			}
		},

		// Gets all existing namespaces
		getNamespaces: async function() {
			const { data } = await axios.get(this.route);
			this.namespaces = data;
		},

		// Deletes a selected namespace
		deleteNamespace: async function(t) {
			if (confirm("Delete permanently?")) {
				await axios.delete(`${this.route}/${t.id}`);
				await this.getNamespaces();
			}
		},

		// Throws a namespace from the list into the editor
		editNamespace: function(t) {
			this.form.name = t.name;
			this.form.color = t.color;
			this.form.id = t.id;
			this.form.order = t.order;
		},

		// Clears the editor
		cancelEdit: function() {
			this.form.name =
				this.form.color =
					this.form.id =
						this.form.order = null;
		}
	},

	async mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
		// Get validation data
		const { data } = await axios.get(this.route + "/validation");
		this.lens = data;
		// Grab the initial set of namespaces
		await this.getNamespaces();
	}
});