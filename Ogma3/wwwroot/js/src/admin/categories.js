new Vue({
	el: "#app",
	data: {
		form: {
			name: null,
			desc: null,
			id: null,
		},
		lens: {
			minNameLength: 5,
			maxNameLength: 20,
			minDescLength: 10,
			maxDescLength: 100,
		},
		err: [],
		categories: [],
		route: null,
	},
	methods: {
		// Contrary to its name, it also modifies a category if needed.
		// It was simply easier to slap both functionalities into a single function.
		createCategory: async function (e) {
			e.preventDefault();

			// Validation
			this.err = [];
			if (
				this.form.name.length > this.lens.maxNameLength ||
				this.form.name.length < this.lens.minNameLength
			)
				this.err.push(
					`Name has to be between ${this.lens.minNameLength} and ${this.lens.maxNameLength} characters long.`,
				);
			if (
				this.form.desc.length > this.lens.maxDescLength ||
				this.form.desc.length < this.lens.minDescLength
			)
				this.err.push(
					`Description has to be between ${this.lens.minDescLength} and ${this.lens.maxDescLength} characters long.`,
				);
			if (this.err.length > 0) return;

			if (this.form.name && this.form.desc) {
				// If no ID has been set, that means it's a new category.
				// Thus, we POST it.
				if (this.form.id === null) {
					await axios.post(this.route, {
						name: this.form.name,
						description: this.form.desc,
					});
					await this.getCategories();

					// If the ID is set, that means it's an existing category.
					// Thus, we PUT it.
				} else {
					await axios.put(`${this.route}/${this.form.id}`, {
						id: this.form.id,
						name: this.form.name,
						description: this.form.desc,
					});
					await this.getCategories();
					this.form.name = this.form.desc = this.form.id = null;
				}
			}
		},

		// Gets all existing categories
		getCategories: async function () {
			const { data } = await axios.get(this.route);
			this.categories = data;
		},

		// Deletes a selected category
		deleteCategory: async function (t) {
			await axios.delete(`${this.route}/${t.id}`);
			await this.getCategories();
		},

		// Throws a category from the list into the editor
		editCategory: function (t) {
			this.form.name = t.name;
			this.form.desc = t.description;
			this.form.id = t.id;
		},

		// Clears the editor
		cancelEdit: function () {
			this.form.name = this.form.desc = this.form.id = null;
		},
	},

	async mounted() {
		// Grab the route from route helper
		this.route = document.getElementById("route").dataset.route;
		// Get validation
		const { data } = await axios.get(`${this.route}/validation`);
		this.lens = data;
		// Grab the initial set of categories
		await this.getCategories();
	},
});
