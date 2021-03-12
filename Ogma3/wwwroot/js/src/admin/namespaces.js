let anamespaces_vue = new Vue({
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
			maxNameLength: 10,
		},
		err: [],
		namespaces: [],
		route: null,
		color: null
	},
	methods: {

		// Contrary to its name, it also modifies a namespace if needed.
		// It was simply easier to slap both functionalities into a single function.
		createNamespace: function (e) {
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
					axios.post(this.route,
						{
							name: this.form.name,
							color: this.form.color,
							order: Number(this.form.order)
						})
						.then(() => {
							this.getNamespaces();
						})
						.catch(error => {
							console.log(error);
						});

					// If the ID is set, that means it's an existing namespace.
					// Thus, we PUT it.
				} else {
					axios.put(this.route + '/' + this.form.id,
						{
							id: this.form.id,
							name: this.form.name,
							color: this.form.color,
							order: Number(this.form.order)
						})
						.then(() => {
							this.getNamespaces();
						})
						.catch(error => {
							console.log(error);
						})
					// Clear the form too
						.then(() => {
							this.form.name =
                                this.form.id = null;
						});
				}

			}
		},

		// Gets all existing namespaces
		getNamespaces: function () {
			axios.get(this.route)
				.then(response => {
					this.namespaces = response.data;
				})
				.catch(error => {
					console.log(error);
				});
		},

		// Deletes a selected namespace
		deleteNamespace: function (t) {
			if(confirm("Delete permanently?")) {
				axios.delete(this.route + '/' + t.id)
					.then(() => {
						this.getNamespaces();
					})
					.catch(error => {
						console.log(error);
					});
			}
		},

		// Throws a namespace from the list into the editor
		editNamespace: function (t) {
			this.form.name = t.name;
			this.form.color = t.color;
			this.form.id = t.id;
			this.form.order = t.order;
		},

		// Clears the editor
		cancelEdit: function () {
			this.form.name =
                this.form.color =
                    this.form.id = 
                        this.form.order = null;
		},
	}, 

	mounted() {
		// Grab the route from route helper
		this.route = document.getElementById('route').dataset.route;
		// Get validation data
		axios.get(this.route + '/validation')
			.then(r => {
				this.lens = r.data;
			})
			.catch(e => console.error(e));
		// Grab the initial set of namespaces
		this.getNamespaces();
	}
});